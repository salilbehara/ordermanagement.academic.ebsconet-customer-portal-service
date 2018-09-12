using ebsco.svc.customerserviceportal.Repositories;
using Moq;
using System;
using System.Threading.Tasks;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class WorkItemTrackerServiceRepository_GetAllWorkItems_should_
    {
        [Fact]
        public void get_all_work_items()
        {
            var customerCode = Any.String();

            var expectedResponse = new GetWorkItemsResponse { WorkItems = Any.WorkItems() };

            var workItemServiceMock = new Mock<IWorkItemService>();
            workItemServiceMock.Setup(settings => settings
                               .GetWorkItemsAsync(It.Is<GetWorkItemsRequest>(request => request.Filter.CustomerCode == customerCode)))
                               .Returns(Task.FromResult(expectedResponse));

            var actualResponse = GetAllWorkItems(() => { return workItemServiceMock.Object; }, customerCode);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact]
        public void get_empty_list_of_work_items_when_none_exists()
        {
            var customerCode = Any.String();

            var expectedResponse = new GetWorkItemsResponse { WorkItems = new WorkItem[0] };

            var workItemServiceMock = new Mock<IWorkItemService>();
            workItemServiceMock.Setup(settings => settings
                               .GetWorkItemsAsync(It.Is<GetWorkItemsRequest>(request => request.Filter.CustomerCode == customerCode)))
                               .Returns(Task.FromResult(expectedResponse));

            var actualResponse = GetAllWorkItems(() => {return workItemServiceMock.Object;}, customerCode);

            Assert.Equal(expectedResponse, actualResponse);
        }

        private GetWorkItemsResponse GetAllWorkItems(Func<IWorkItemService> service, string customerCode)
        {          
            return new WorkItemTrackerServiceRepository(service).GetAllWorkItems(customerCode);
        }
    }
}
