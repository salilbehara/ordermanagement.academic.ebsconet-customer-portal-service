using ebsco.svc.customerserviceportal.Controllers;
using ebsco.svc.customerserviceportal.Extensions;
using ebsco.svc.customerserviceportal.Models;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.webapi.framework.Helpers;
using ebsco.svc.webapi.framework.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ControllerTests
{
    public class WorkItemFilesController_GetAllWorkItemFiles_should_
    {
        private Mock<IWorkItemTrackerServiceRepository> _workItemTrackerServiceRepositoryMock = new Mock<IWorkItemTrackerServiceRepository>();

        [Fact]
        public void return_all_files_for_work_item_id()
        {
            var workItemId = Any.Int();
            var workItemDocumentsFromWit = Any.WorkItemDocuments(length: 2, workItemId: workItemId);

            var expectedWorkItemFiles = new List<WorkItemFileModel>
            {
                workItemDocumentsFromWit[0].MapWorkItemDocumentToWorkItemFileModel(),
                workItemDocumentsFromWit[1].MapWorkItemDocumentToWorkItemFileModel()
            };

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                .GetAllWorkItemFiles(workItemId))
                .Returns(new GetAllWorkItemDocumentsResponse { WorkItemDocuments = workItemDocumentsFromWit });

            var httpResponse = GetAllWorkFiles(workItemId) as OkObjectResult;
            Assert.Equal(200, httpResponse.StatusCode);

            var actualWorkItemFiles = (((httpResponse.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemFileInfoModel>).Value as WorkItemFileInfoModel).WorkItemFiles;
            Assert.Equal(expectedWorkItemFiles[0].FileId, actualWorkItemFiles[0].FileId);
            Assert.Equal(expectedWorkItemFiles[0].FileName, actualWorkItemFiles[0].FileName);
            Assert.Equal(expectedWorkItemFiles[0].CreatedDate, actualWorkItemFiles[0].CreatedDate);
            Assert.Equal(expectedWorkItemFiles[0].DeleteFlag, actualWorkItemFiles[0].DeleteFlag);

            Assert.Equal(expectedWorkItemFiles[1].FileId, actualWorkItemFiles[1].FileId);
            Assert.Equal(expectedWorkItemFiles[1].FileName, actualWorkItemFiles[1].FileName);
            Assert.Equal(expectedWorkItemFiles[1].CreatedDate, actualWorkItemFiles[1].CreatedDate);
            Assert.Equal(expectedWorkItemFiles[1].DeleteFlag, actualWorkItemFiles[1].DeleteFlag);
        }

        [Fact]
        public void return_empty_array_when_no_files_exist_for_work_item()
        {
            var workItemId = Any.Int();

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                .GetAllWorkItemFiles(workItemId))
                .Returns(new GetAllWorkItemDocumentsResponse { WorkItemDocuments = new WorkItemDocument[0] });

            var httpResponse = GetAllWorkFiles(workItemId) as OkObjectResult;
            Assert.Equal(200, httpResponse.StatusCode);

            var actualWorkItemFiles = (((httpResponse.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemFileInfoModel>).Value as WorkItemFileInfoModel).WorkItemFiles;
            Assert.Empty(actualWorkItemFiles);
        }

        private IActionResult GetAllWorkFiles(int workItemId)
        {
            return new WorkItemFilesController(
                    _workItemTrackerServiceRepositoryMock.Object,
                    new Mock<IMediaServerServiceRepository>().Object,
                    new Mock<IUrlHelper>().Object)
                .GetAllWorkItemFiles(workItemId);
        }
    }
}
