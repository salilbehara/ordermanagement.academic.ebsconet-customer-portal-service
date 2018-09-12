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
    [Route("workitems/{workItemId}/files")]
    [Produces("application/json")]
    public class WorkItemFilesController : BaseController
    {
        private readonly IWorkItemTrackerServiceRepository _workItemTrackerServiceRepository;
        private readonly IMediaServerServiceRepository _mediaServerServiceRepository;

        public WorkItemFilesController(IWorkItemTrackerServiceRepository workItemTrackerServiceRepository, IMediaServerServiceRepository mediaServerServiceRepository, IUrlHelper urlHelper)
        {
            _workItemTrackerServiceRepository = workItemTrackerServiceRepository;
            _mediaServerServiceRepository = mediaServerServiceRepository;
            Url = urlHelper;
        }

        /// <summary>
        /// Get metadata about all work item files for the supplied workitemId.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /workitems/12345/files
        ///     
        /// </remarks>
        /// <param name="workItemId">The workItemId of your desired work item files.</param>
        [HttpGet(Name = "GetAllWorkItemFiles")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(ResourceWrapper<WorkItemFileInfoModel>), "Operation successful. Returns the array of work item file metadata objects for the supplied workItemId.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "An unexpected fault happened.")]
        public IActionResult GetAllWorkItemFiles(int workItemId)
        {
            var getAllWorkItemDocumentsResponse = _workItemTrackerServiceRepository.GetAllWorkItemFiles(workItemId);

            List<WorkItemFileModel> workItemFileModels = new List<WorkItemFileModel>();

            workItemFileModels.AddRange(getAllWorkItemDocumentsResponse.WorkItemDocuments.Select(d => d.MapWorkItemDocumentToWorkItemFileModel()));

            workItemFileModels.ForEach(workItemFileModel => AddLinksToWorkItemFileModel(workItemFileModel));

            var workItemFileInfoModel = new WorkItemFileInfoModel 
            {
                WorkItemId = workItemId,
                WorkItemFiles = workItemFileModels.ToArray()
            };

            var response = AddLinksToWorkItemFileInfoModel(new ResourceWrapper<WorkItemFileInfoModel>(workItemFileInfoModel));

            return Ok(response);
        }

        /// <summary>
        /// Get the content of a work item file by workItemFileId.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /workitems/12345/files/67890
        ///     
        /// </remarks>
        /// <param name="workItemId">The workItemId of your desired work item file.</param>
        /// <param name="workItemFileId">The workItemFileId of your desired work item file.</param>
        [HttpGet]
        [Route("{workItemFileId}", Name = "GetWorkItemFile")]
        [SwaggerResponse((int)HttpStatusCode.OK, null, "Operation successful. Returns the content of the work item file for the supplied workItemFileId.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, null, "No work item file was found with the supplied workItemFileId.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "An unexpected fault happened.")]
        public IActionResult GetWorkItemFile(int workItemId, string workItemFileId)
        {
            var workItemExists = _workItemTrackerServiceRepository.GetWorkItemFile(workItemId, workItemFileId).WorkItemDocument != null;

            if (!workItemExists)
            {
                return NotFound();
            }

            var response = _mediaServerServiceRepository.DownloadWorkItemFile(workItemFileId);

            return File(response.Contents, response.MimeType, response.FileName);
        }

        /// <summary>
        /// Create a new work item file.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /workitems/12345/files
        ///     
        /// </remarks>
        /// <param name="workItemId">The workItemId of the work item file being created.</param>
        /// <param name="workItemFile">The name and contents of work item file being created.</param>
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.Created, typeof(ResourceWrapper<WorkItemFileCreateInfoModel>), "Operation successful.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, null, "Unable to process due to an invalid request.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "An unexpected fault happened.")]
        public IActionResult CreateWorkItemFile(int workItemId, [FromBody] WorkItemFileCreateModel workItemFile)
        {
            if (workItemFile == null)
            {
                return BadRequest("Not a valid request.");
            }

            var workItemFileId = _mediaServerServiceRepository.AddWorkItemFile(workItemFile);

            var createWorkItemFileResponse = _workItemTrackerServiceRepository.CreateWorkItemFile(workItemId, workItemFileId, workItemFile);

            if (createWorkItemFileResponse.Success == false)
            {
                return BadRequest(createWorkItemFileResponse.ErrorMessage);
            };

            var workItemFileModel = createWorkItemFileResponse.WorkItemDocument.MapWorkItemDocumentToWorkItemFileModel();

            var workItemFileCreateInfoModel = new WorkItemFileCreateInfoModel()
            {
                WorkItemId = workItemId,
                WorkItemFile = AddLinksToWorkItemFileModel(workItemFileModel)
            };

            var response = AddLinksToWorkItemFileCreateInfoModel(new ResourceWrapper<WorkItemFileCreateInfoModel>(workItemFileCreateInfoModel));

            return CreatedAtRoute("GetWorkItemFile", new {workItemFileId = workItemFileModel.FileId}, response);
        }

        #region Add Hypermedia links to the responses
        private WorkItemFileModel AddLinksToWorkItemFileModel(WorkItemFileModel workItemFileModel)
        {
            workItemFileModel.Links.Add(
                new LinkModel(Url.Link("GetWorkItemFile", new { workItemFileId = workItemFileModel.FileId })
                              , "workItems_Files_Uri"
                              , "GET"));

            return workItemFileModel;
        }
        
        private ResourceWrapper<WorkItemFileInfoModel> AddLinksToWorkItemFileInfoModel(ResourceWrapper<WorkItemFileInfoModel> workItemFileInfoModelWrapper)
        {
            workItemFileInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItemFiles", new { workItemFileInfoModelWrapper.Value.WorkItemId })
                              , "self"
                              , "GET"));

            workItemFileInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetWorkItemDetails", new { workItemFileInfoModelWrapper.Value.WorkItemId })
                              , "parent"
                              , "GET"));

            return workItemFileInfoModelWrapper;
        }
        
        private ResourceWrapper<WorkItemFileCreateInfoModel> AddLinksToWorkItemFileCreateInfoModel(ResourceWrapper<WorkItemFileCreateInfoModel> workItemFileCreateInfoModelWrapper)
        {
            workItemFileCreateInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetWorkItemFile", new { workItemFileId = workItemFileCreateInfoModelWrapper.Value.WorkItemFile.FileId })
                              , "self"
                              , "GET"));

            workItemFileCreateInfoModelWrapper.Links.Add(
                new LinkModel(Url.Link("GetAllWorkItemFiles", new { workItemFileCreateInfoModelWrapper.Value.WorkItemId })
                              , "parent"
                              , "GET"));

            return workItemFileCreateInfoModelWrapper;
        }
        #endregion
    }
}
