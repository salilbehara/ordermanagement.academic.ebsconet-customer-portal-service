using ebsco.svc.customerserviceportal.Repositories;
using Moq;
using System;
using System.Threading.Tasks;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class WorkItemTrackerServiceRepository_GetWorkItemFile_should_
    {
        [Fact]
        public void download_work_item_file_metadata_object()
        {
            var expectedResponse = new GetWorkItemDocumentResponse { WorkItemDocument = Any.WorkItemDocument() };

            var workItemServiceMock = new Mock<IWorkItemService>();
            workItemServiceMock.Setup(settings => settings
                               .GetWorkItemDocumentAsync(It.Is<GetWorkItemDocumentRequest>(request => request.DocumentId == expectedResponse.WorkItemDocument.DocumentId)))
                               .Returns(Task.FromResult(expectedResponse));

            var actualResponse = GetWorkItemFile(() => 
                { return workItemServiceMock.Object; }, 
                expectedResponse.WorkItemDocument.WorkItemId, 
                expectedResponse.WorkItemDocument.DocumentId);

            Assert.Equal(expectedResponse, actualResponse);
        }

        private GetWorkItemDocumentResponse GetWorkItemFile(Func<IWorkItemService> service, int workItemId, string workItemFileId)
        {
            return new WorkItemTrackerServiceRepository(service).GetWorkItemFile(workItemId, workItemFileId);
        }
    }
}
