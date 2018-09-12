using ebsco.svc.customerserviceportal.Repositories;
using Moq;
using System;
using System.Threading.Tasks;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class WorkItemTrackerServiceRepository_GetWorkItemDetails_should_
    {
        [Fact]
        public void get_work_item_details()
        {
            var getWorkItemDetailsResponse = new GetWorkItemDetailsResponse
            {
                WorkItemDetailInfo = new WorkItemDetailInfo
                {
                    Details = Any.WorkItemDetails(),
                    NoteLabel = Any.String(),
                    NoteLabelTranslationKey = Any.String(),
                    NoteText = Any.String()
                }
            };

            var expectedResponse = getWorkItemDetailsResponse;

            var actualResponse = GetWorkItemDetails(getWorkItemDetailsResponse);

            Assert.Equal(expectedResponse, actualResponse);
        }

        [Fact]
        public void get_empty_list_of_work_items_when_none_exists()
        {
            var getWorkItemDetailsResponse = new GetWorkItemDetailsResponse { };
            
            var expectedResponse = getWorkItemDetailsResponse;

            var actualResponse = GetWorkItemDetails(getWorkItemDetailsResponse);

            Assert.Equal(expectedResponse, actualResponse);
        }

        private GetWorkItemDetailsResponse GetWorkItemDetails(GetWorkItemDetailsResponse getWorkItemDetailsResponse)
        {
            var workItemServiceMock = new Mock<IWorkItemService>();
            var workItemServiceMockDelegate = new Mock<Func<IWorkItemService>>();

            workItemServiceMock.Setup(settings => settings
                               .GetWorkItemDetailsAsync(It.IsAny<GetWorkItemDetailsRequest>()))
                               .Returns(Task.FromResult(getWorkItemDetailsResponse));
            workItemServiceMockDelegate.Setup(x => x()).Returns(workItemServiceMock.Object);

            return new WorkItemTrackerServiceRepository(workItemServiceMockDelegate.Object).GetWorkItemDetails(Any.Int());
        }
    }
}
