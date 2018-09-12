using ebsco.svc.customerserviceportal.Controllers;
using ebsco.svc.customerserviceportal.Models;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.webapi.framework.Helpers;
using ebsco.svc.webapi.framework.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ControllerTests
{
    public class WorkItemFilesController_CreateWorkItemFile_should_
    {
        private Mock<IWorkItemTrackerServiceRepository> _workItemTrackerServiceRepositoryMock = new Mock<IWorkItemTrackerServiceRepository>();

        [Fact]
        public void create_a_work_item_file()
        {
            var workItemDocument = Any.WorkItemDocument();
            
            var workItemFile = new WorkItemFileCreateModel
            {
                WorkItemFileName = Any.String(),
                WorkItemFileContent = Any.ByteArray()
            };

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                           .CreateWorkItemFile(workItemDocument.WorkItemId, workItemDocument.DocumentId, workItemFile))
                           .Returns(new WorkItemDocumentResponse { WorkItemDocument = workItemDocument, Success = true });

            var expectedFileId = workItemDocument.DocumentId;
            var expectedFileName = workItemDocument.DocumentName;
            var expectedCreatedDate = workItemDocument.CreatedDate;
            var expectedDeleteFlag = workItemDocument.DeleteFlag;
            var expectedWorkItemId = workItemDocument.WorkItemId;

            var response = CreateWorkItemFile(workItemDocument, workItemFile) as CreatedResult;

            var actualWorkItemDocument = (response.Value as ResourceWrapper<WorkItemFileCreateInfoModel>).Value as WorkItemFileCreateInfoModel;

            Assert.Equal(201, response.StatusCode);
            Assert.Equal(expectedFileId, actualWorkItemDocument.WorkItemFile.FileId);
            Assert.Equal(expectedFileName, actualWorkItemDocument.WorkItemFile.FileName);
            Assert.Equal(expectedCreatedDate, actualWorkItemDocument.WorkItemFile.CreatedDate);
            Assert.Equal(expectedDeleteFlag, actualWorkItemDocument.WorkItemFile.DeleteFlag);
            Assert.Equal(expectedWorkItemId, actualWorkItemDocument.WorkItemId);
        }

        [Fact]
        public void return_a_400_Bad_Request_response_code_when_work_item_service_returns_an_error()
        {
            var workItemDocument = Any.WorkItemDocument();

            var workItemFile = new WorkItemFileCreateModel
            {
                WorkItemFileName = Any.String(),
                WorkItemFileContent = Any.ByteArray()
            };

            var workItemTrackerServiceErrorMessage = "WIT Service Error";

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                           .CreateWorkItemFile(workItemDocument.WorkItemId, workItemDocument.DocumentId, workItemFile))
                           .Returns(new WorkItemDocumentResponse { WorkItemDocument = workItemDocument, Success = false, ErrorMessage = workItemTrackerServiceErrorMessage });

            var response = (CreateWorkItemFile(workItemDocument, workItemFile) as BadRequestObjectResult).Value as ApiBadRequestResponse;

            Assert.Equal(400, response.StatusCode);
            Assert.Contains(workItemTrackerServiceErrorMessage, response.Errors);

        }

        [Fact]
        public void return_a_400_Bad_Request_response_code_for_invalid_request_message()
        {
            var workItemDocument = Any.WorkItemDocument();

            WorkItemFileCreateModel workItemFile = null;

            var expectedErrorMessage = "Not a valid request.";

            var response = (CreateWorkItemFile(workItemDocument, workItemFile) as BadRequestObjectResult).Value as ApiBadRequestResponse;

            Assert.Equal(400, response.StatusCode);
            Assert.Contains(expectedErrorMessage, response.Errors);
        }

        private IActionResult CreateWorkItemFile(WorkItemDocument workItemDocument, WorkItemFileCreateModel workItemFile)
        {
            var mediaServerServicerepositoryMock = new Mock<IMediaServerServiceRepository>();

            mediaServerServicerepositoryMock.Setup(repository => repository
                .AddWorkItemFile(workItemFile))
                .Returns(workItemDocument.DocumentId);

            var urlHelperMock = new Mock<IUrlHelper>();

            urlHelperMock.Setup(repository => repository
                .Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Any.String());

            return new WorkItemFilesController(
                _workItemTrackerServiceRepositoryMock.Object,
                mediaServerServicerepositoryMock.Object,
                urlHelperMock.Object)
                .CreateWorkItemFile(workItemDocument.WorkItemId, workItemFile);
        }
    }
}
