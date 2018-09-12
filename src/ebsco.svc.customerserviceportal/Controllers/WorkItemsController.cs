using ebsco.svc.customerserviceportal.Extensions;
using ebsco.svc.customerserviceportal.Models;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.webapi.framework.Controllers;
using ebsco.svc.webapi.framework.Helpers;
using ebsco.svc.webapi.framework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ebsco.svc.customerserviceportal.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("workitems")]
    public class WorkItemsController : BaseController
    {
        private readonly IWorkItemTrackerServiceRepository _workItemTrackerServiceRepository;
        private readonly ITranslationsService _translationsService;

        public WorkItemsController(IWorkItemTrackerServiceRepository workItemTrackerServiceRepository, ITranslationsService translationsService, IUrlHelper urlHelper)
        {
            _workItemTrackerServiceRepository = workItemTrackerServiceRepository;
            _translationsService = translationsService;
            Url = urlHelper;
        }

        /// <summary>
        /// Get all work items for the supplied customerCode.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /workitems?customerCode=BR12345
        ///     
        /// </remarks>
        /// <param name="customerCode">The customerCode of your desired work items.</param>
        [HttpGet(Name = "GetAllWorkItems")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(ResourceWrapper<WorkItemHeaderInfoModel>), "Operation successful. Returns an array of work items for the supplied customerCode.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "An unexpected fault happened.")]
        public IActionResult GetAllWorkItems([FromQuery]string customerCode)
        {
            customerCode = User.FindFirst("customercode")?.Value ?? customerCode;

            var getWorkItemsResponse = _workItemTrackerServiceRepository.GetAllWorkItems(customerCode);

            var workItemHeaderModels = new List<WorkItemHeaderModel>();
            
            workItemHeaderModels.AddRange(getWorkItemsResponse.WorkItems.Select(w => w.MapWorkItemToWorkItemHeaderModel()));

            workItemHeaderModels.ForEach(workItemHeaderModel => AddLinksToWorkItemHeaderModel(workItemHeaderModel));

            var workItemHeaderInfoModel = new WorkItemHeaderInfoModel 
            {
                CustomerCode = customerCode,
                WorkItems = workItemHeaderModels.ToArray()
            };

            var response = AddLinksToWorkItemHeaderInfoModel(new ResourceWrapper<WorkItemHeaderInfoModel>(workItemHeaderInfoModel));

            return Ok(response);
        }

        /// <summary>
        /// Get work item details for the supplied workItemId .
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET workitems/{workItemId}
        ///     
        /// </remarks>
        /// <param name="workItemId">The workItemId of your desired work item details.</param>
        [HttpGet]
        [Route("{workItemId}", Name = "GetWorkItemDetails")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(ResourceWrapper<WorkItemDetailInfoModel>), "Operation successful. Returns the work item details for the supplied workItemId.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, null, "The supplied workItemId was not found.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "An unexpected fault happened.")]
        public IActionResult GetWorkItemDetails(int workItemId)
        {
            var getWorkItemDetailsResponse = _workItemTrackerServiceRepository.GetWorkItemDetails(workItemId);

            var workItemDetailInfoModel = getWorkItemDetailsResponse.WorkItemDetailInfo.MapWorkItemDetailInfoToWorkItemDetailInfoModel();

            workItemDetailInfoModel.WorkItemId = workItemId;

            if (workItemDetailInfoModel.WorkItemDetails.Length == 0)
            {
                return NotFound();
            }

            workItemDetailInfoModel.WorkItemDetails.ToList().ForEach(d =>
            {
                d.Name = string.IsNullOrEmpty(d.TranslationKey) ? d.Name : _translationsService.TranslateResource(d.TranslationKey);
            });

            workItemDetailInfoModel.TranslatedClaimDetailLink = _translationsService.TranslateResource("EN_ClaimDetailLink");

            var response = AddLinksToWorkItemDetailInfoModel(new ResourceWrapper<WorkItemDetailInfoModel>(workItemDetailInfoModel));

            return Ok(response);      
        }

        #region Add Hypermedia links to the responses
        private WorkItemHeaderModel AddLinksToWorkItemHeaderModel(WorkItemHeaderModel workItemHeaderModel)
        {
            workItemHeaderModel.Links.Add(
                new LinkModel(Url.Link("GetWorkItemDetails", new { workItemHeaderModel.WorkItemId })
                              , "workItems_Details_Uri"
                              , "GET"));

            workItemHeaderModel.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItemFiles", new { workItemHeaderModel.WorkItemId })
                              , "workItems_Files_Uri"
                              , "GET"));

            workItemHeaderModel.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItemComments", new { workItemHeaderModel.WorkItemId })
                              , "workItems_Comments_Uri"
                              , "GET"));

            return workItemHeaderModel;
        }

        private ResourceWrapper<WorkItemHeaderInfoModel> AddLinksToWorkItemHeaderInfoModel(ResourceWrapper<WorkItemHeaderInfoModel> workItemHeaderInfoModelWrapper)
        {
            workItemHeaderInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItems", new { customerCode = workItemHeaderInfoModelWrapper.Value.CustomerCode })
                              , "self"
                              , "GET"));

            return workItemHeaderInfoModelWrapper;
        }

        private ResourceWrapper<WorkItemDetailInfoModel> AddLinksToWorkItemDetailInfoModel(ResourceWrapper<WorkItemDetailInfoModel> workItemDetailInfoModelWrapper)
        {
            workItemDetailInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetWorkItemDetails", new { workItemDetailInfoModelWrapper.Value.WorkItemId })
                              , "self"
                              , "GET"));

            workItemDetailInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItemFiles", new { workItemDetailInfoModelWrapper.Value.WorkItemId })
                              , "workItems_Files_Uri"
                              , "GET"));

            workItemDetailInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItemComments", new { workItemDetailInfoModelWrapper.Value.WorkItemId })
                              , "workItems_Comments_Uri"
                              , "GET"));

            return workItemDetailInfoModelWrapper;
        }
        #endregion
    }
}