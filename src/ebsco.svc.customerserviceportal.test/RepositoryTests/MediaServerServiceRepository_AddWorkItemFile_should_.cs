using ebsco.svc.customerserviceportal.Models;
using ebsco.svc.customerserviceportal.Repositories;
using MediaServerService;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class MediaServerServiceRepository_AddWorkItemFile_should_
    {
        [Fact]
        public void add_work_item_file()
        {
            var expectedResponse = Any.String();

            var actualResponse = DownloadWorkItemFile(expectedResponse);

            Assert.Equal(expectedResponse, actualResponse);
        }

        private string DownloadWorkItemFile(string workItemFileId)
        {
            var mediaServerServiceMock = new Mock<IMediaServerService>();
            var mediaServerServiceMockDelegate = new Mock<Func<IMediaServerService>>();

            mediaServerServiceMock.Setup(settings => settings
                               .AddFileStreamAsync(It.IsAny<AddFileStreamRequest>()))
                               .Returns(Task.FromResult(new AddFileStreamResponse { Guid = workItemFileId }));

            mediaServerServiceMockDelegate.Setup(x => x()).Returns(mediaServerServiceMock.Object);

            return new MediaServerServiceRepository(mediaServerServiceMockDelegate.Object).AddWorkItemFile(
                new WorkItemFileCreateModel { 
                    WorkItemFileName = Any.String(), WorkItemFileContent = Any.ByteArray() });
        }
    }
}
