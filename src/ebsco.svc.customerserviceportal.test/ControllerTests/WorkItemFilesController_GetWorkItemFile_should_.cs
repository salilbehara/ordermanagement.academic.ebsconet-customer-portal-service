using ebsco.svc.customerserviceportal.Controllers;
using ebsco.svc.customerserviceportal.Repositories;
using MediaServerService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ControllerTests
{
    public class WorkItemFilesController_GetWorkItemFile_should_
    {
        private Mock<IWorkItemTrackerServiceRepository> _workItemTrackerServiceRepositoryMock = new Mock<IWorkItemTrackerServiceRepository>();
        private Mock<IMediaServerServiceRepository> _mediaServerServicerepositoryMock = new Mock<IMediaServerServiceRepository>();

        [Fact]
        public void return_a_work_item_file()
        {
            var workItemFileId = Guid.NewGuid().ToString();
            var expectedFileName = $"{ Any.String() }.pdf";
            const string expectedContentType = "application/pdf";
            var expectedContent = Any.ByteArray();

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                .GetWorkItemFile(It.IsAny<int>(), workItemFileId))
                .Returns(new GetWorkItemDocumentResponse
                {
                    WorkItemDocument = new WorkItemDocument { DocumentId = workItemFileId, DocumentName = expectedFileName }
                });

            _mediaServerServicerepositoryMock.Setup(repository => repository
                .DownloadWorkItemFile(workItemFileId))
                .Returns(new GetFileStreamResponse
                {
                    Contents = expectedContent,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    FileGuid = workItemFileId,
                    FileName = expectedFileName,
                    MimeType = expectedContentType
                });

            var fileResponse = GetWorkItemFile(workItemFileId) as FileContentResult;

            Assert.Equal(expectedFileName, fileResponse.FileDownloadName);
            Assert.Equal(expectedContentType, fileResponse.ContentType);
            Assert.Equal(expectedContent, fileResponse.FileContents);
        }

        [Fact]
        public void return_a_404_when_work_item_does_not_exist()
        {
            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                .GetWorkItemFile(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new GetWorkItemDocumentResponse { WorkItemDocument = null });

            var response = GetWorkItemFile(Guid.NewGuid().ToString());
            Assert.IsType<NotFoundObjectResult>(response);
        }

        private IActionResult GetWorkItemFile(string workItemFileId)
        {
            return new WorkItemFilesController(
                    _workItemTrackerServiceRepositoryMock.Object,
                    _mediaServerServicerepositoryMock.Object,
                    new Mock<IUrlHelper>().Object)
                .GetWorkItemFile(Any.Int(), workItemFileId);
        }
    }
}
