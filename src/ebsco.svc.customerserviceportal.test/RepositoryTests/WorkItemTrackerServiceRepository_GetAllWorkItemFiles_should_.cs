using ebsco.svc.customerserviceportal.Repositories;
using Moq;
using System;
using System.Threading.Tasks;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class WorkItemTrackerServiceRepository_GetAllWorkItemFiles_should_
    {
        [Fact]
        public void get_all_files_for_work_item()
        {
            var workItemId = Any.Int();

            var expectedResponse = new GetAllWorkItemDocumentsResponse { WorkItemDocuments = Any.WorkItemDocuments() };

            var workItemServiceMock = new Mock<IWorkItemService>();
            workItemServiceMock.Setup(settings => settings
                               .GetAllWorkItemDocumentsAsync(It.Is<GetAllWorkItemDocumentsRequest>(request => request.WorkItemId == workItemId)))
                               .Returns(Task.FromResult(expectedResponse));

            var actualResponse = GetAllWorkItemFiles(() => { return workItemServiceMock.Object; }, workItemId);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact]
        public void return_empty_array_when_no_files_exist_for_work_item()
        {
            var workItemId = Any.Int();

            var expectedResponse = new GetAllWorkItemDocumentsResponse { WorkItemDocuments = new WorkItemDocument[0] };

            var workItemServiceMock = new Mock<IWorkItemService>();
            workItemServiceMock.Setup(settings => settings
                               .GetAllWorkItemDocumentsAsync(It.Is<GetAllWorkItemDocumentsRequest>(request => request.WorkItemId == workItemId)))
                               .Returns(Task.FromResult(expectedResponse));

            var actualResponse = GetAllWorkItemFiles(() => { return workItemServiceMock.Object; }, workItemId);

            Assert.Empty(actualResponse.WorkItemDocuments);
        }

        private GetAllWorkItemDocumentsResponse GetAllWorkItemFiles(Func<IWorkItemService> service, int workItemId)
        {
            return new WorkItemTrackerServiceRepository(service).GetAllWorkItemFiles(workItemId);
        }
    }
}
