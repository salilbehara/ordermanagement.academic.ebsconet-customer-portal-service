using ebsco.svc.customerserviceportal.Extensions;
using ebsco.svc.customerserviceportal.Models;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.webapi.framework.Controllers;
using ebsco.svc.webapi.framework.Helpers;
using ebsco.svc.webapi.framework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Net;
using WorkItemTracker;

namespace ebsco.svc.customerserviceportal.Controllers
{
    [Authorize]
    [Route("workitems/{workItemId}/comments")]
    [Produces("application/json")]
    public class WorkItemCommentsController : BaseController
    {
        private readonly IWorkItemTrackerServiceRepository _workItemTrackerServiceRepository;
        
        public WorkItemCommentsController(IWorkItemTrackerServiceRepository workItemTrackerServiceRepository,  IUrlHelper urlHelper)
        {
            _workItemTrackerServiceRepository = workItemTrackerServiceRepository;
            Url = urlHelper;
        }

        /// <summary>
        /// Get all comments for the supplied workItemId .
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET workitems/{workItemId}/comments
        ///     
        /// </remarks>
        /// <param name="workItemId">The workItemId of your desired work item comments.</param>
        [HttpGet(Name = "GetAllWorkItemComments")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(ResourceWrapper<WorkItemCommentModel>), "Operation successful. Returns the array of work item comments for the supplied workItemId.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "An unexpected fault happened.")]
        public IActionResult GetAllWorkItemComments(int workItemId)
        {
            var getAllWorkItemCommentsResponse = _workItemTrackerServiceRepository.GetAllWorkItemComments(workItemId);
         
            var workItemCommentModels = getAllWorkItemCommentsResponse.Communications.Select(d => d.MapCommunicationToWorkItemCommentModel()).ToList();
            
            var workItemCommentsModel = new WorkItemCommentInfoModel
            {
                WorkItemId = workItemId,
                WorkItemComments = workItemCommentModels.ToArray()
            };

            var response = AddLinksToWorkItemCommentInfoModel(new ResourceWrapper<WorkItemCommentInfoModel>(workItemCommentsModel));

            return Ok(response);
        }

        /// <summary>
        /// Create a new work item comment.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /workitems/12345/comments
        ///     
        /// </remarks>
        /// <param name="workItemId">The workItemId of the work item comment being created.</param>
        /// <param name="workItemComment">The contents of work item comment being created.</param> 
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.Created, null, "Operation successful.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, null, "Unable to process due to an invalid request.")] 
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "An unexpected fault happened.")]
        public IActionResult CreateWorkItemComment(int workItemId, [FromBody] WorkItemCommentCreateModel workItemComment)
        {
            if (workItemComment == null)
            {
                return BadRequest("Not a valid request.");
            }

            var workItemUserDetails = new WorkItemUserDetails
            {
                Id = User.FindFirst("userid")?.Value.NullSafeTrim(),
                Name = $"{User.FindFirst("firstname")?.Value} {User.FindFirst("lastname")?.Value}".NullSafeTrim(),
                Phone = User.FindFirst("dayphone")?.Value.NullSafeTrim(),
                Email = User.FindFirst("emailaddress")?.Value.NullSafeTrim(),
                Fax = User.FindFirst("faxnumber")?.Value.NullSafeTrim()
            };
            
            var addWorkItemCommunicationResponse = _workItemTrackerServiceRepository.CreateWorkItemCommunication(workItemId, workItemUserDetails, workItemComment);

            if (addWorkItemCommunicationResponse.Success == false) 
            {
                return BadRequest(addWorkItemCommunicationResponse.ErrorMessage);
            }
            
            return CreatedAtRoute("GetAllWorkItemComments", new { CommunicationId = addWorkItemCommunicationResponse.CommunicationId });
        }

        private ResourceWrapper<WorkItemCommentInfoModel> AddLinksToWorkItemCommentInfoModel(ResourceWrapper<WorkItemCommentInfoModel> workItemCommentModelInfoWrapper)
        {
            workItemCommentModelInfoWrapper.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItemComments", new { workItemCommentModelInfoWrapper.Value.WorkItemId })
                              , "self"
                              , "GET"));

            workItemCommentModelInfoWrapper.Links.Add(
                new LinkModel(Url.Link("GetWorkItemDetails", new { workItemCommentModelInfoWrapper.Value.WorkItemId })
                              , "parent"
                              , "GET"));

            return workItemCommentModelInfoWrapper;
        }
    }
}
