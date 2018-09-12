using Xunit;
using Moq;
using ebsco.svc.customerserviceportal.Repositories;
using WorkItemTracker;
using System.Threading.Tasks;
using System;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class WorkItemTrackerServiceRepository_GetAllWorkItemComments_should_
    {
        [Fact]
        public void get_all_comments_for_work_item()
        {
            var workItemId = Any.Int();

            var expectedResponse = new GetWorkItemCommunicationsResponse { Communications = Any.Communications(length: Any.Int(), workItemId: workItemId) };

            var workItemServiceMock = new Mock<IWorkItemService>();
            workItemServiceMock.Setup(settings => settings
                .GetWorkItemCommunicationsAsync(It.Is<GetWorkItemCommunicationsRequest>(request => request.WorkItemId == workItemId)))
                .Returns(Task.FromResult(expectedResponse));

            var actualResponse = GetAllWorkItemComments(() => { return workItemServiceMock.Object; }, workItemId);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact]
        public void return_empty_array_when_no_comments_exist_for_work_item()
        {
            var workItemId = Any.Int();

            var expectedResponse = new GetWorkItemCommunicationsResponse { Communications = new Communication[0] };

            var workItemServiceMock = new Mock<IWorkItemService>();
            workItemServiceMock.Setup(settings => settings
                .GetWorkItemCommunicationsAsync(It.Is<GetWorkItemCommunicationsRequest>(request => request.WorkItemId == workItemId)))
                .Returns(Task.FromResult(expectedResponse));

            var actualResponse = GetAllWorkItemComments(() => { return workItemServiceMock.Object; }, workItemId);

            Assert.Empty(actualResponse.Communications);
        }

        private GetWorkItemCommunicationsResponse GetAllWorkItemComments(Func<IWorkItemService> service, int workItemId)
        {
            return new WorkItemTrackerServiceRepository(service).GetAllWorkItemComments(workItemId);
        }
    }
}
