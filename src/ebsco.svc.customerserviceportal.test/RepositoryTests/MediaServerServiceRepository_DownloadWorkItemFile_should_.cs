using ebsco.svc.customerserviceportal.Repositories;
using MediaServerService;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class MediaServerServiceRepository_DownloadWorkItemFile_should_
    {
        [Fact]
        public void retrieve_work_item_file()
        {
            var expectedResponse = new GetFileStreamResponse
            {
                Contents = Any.ByteArray(),
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                FileGuid = Guid.NewGuid().ToString(),
                FileName = $"{ Any.String() }.pdf",
                MimeType = "application/pdf"
            };

            var mediaServerServiceMock = new Mock<IMediaServerService>();
            mediaServerServiceMock.Setup(settings => settings
                               .GetFileStreamAsync(It.Is<GetFileStreamRequest>(request => request.Guid == expectedResponse.FileGuid)))
                               .Returns(Task.FromResult(expectedResponse));

            var actualResponse = DownloadWorkItemFile(() =>
                { return mediaServerServiceMock.Object; },
                expectedResponse.FileGuid);

            Assert.Equal(expectedResponse, actualResponse);
        }

        private GetFileStreamResponse DownloadWorkItemFile(Func<IMediaServerService> service, string workItemFileId)
        {
            return new MediaServerServiceRepository(service).DownloadWorkItemFile(workItemFileId);
        }
    }
}
