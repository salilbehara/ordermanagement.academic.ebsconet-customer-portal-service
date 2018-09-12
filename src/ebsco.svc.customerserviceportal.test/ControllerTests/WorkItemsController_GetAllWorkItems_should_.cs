using ebsco.svc.customerserviceportal.Controllers;
using ebsco.svc.customerserviceportal.Extensions;
using ebsco.svc.customerserviceportal.Models;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.webapi.framework.Helpers;
using ebsco.svc.webapi.framework.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Security.Claims;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ControllerTests
{
    public class WorkItemsController_GetAllWorkItems_should_
    {
        [Fact]
        public void return_all_work_items()
        {
            var workItemsFromRepository = Any.WorkItems(2);

            var expectedWorkItems = workItemsFromRepository.Select(w => w.MapWorkItemToWorkItemHeaderModel()).ToList();

            var response = GetAllWorkItems(workItemsFromRepository) as OkObjectResult;

            var actualWorkItems = (((response.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemHeaderInfoModel>).Value as WorkItemHeaderInfoModel).WorkItems;

            Assert.Equal(200, response.StatusCode);

            Assert.Equal(expectedWorkItems[0].CreatedDate, actualWorkItems[0].CreatedDate);
            Assert.Equal(expectedWorkItems[0].EbscoOrderNumber, actualWorkItems[0].EbscoOrderNumber);
            Assert.Equal(expectedWorkItems[0].Owner, actualWorkItems[0].Owner);
            Assert.Equal(expectedWorkItems[0].TitleName, actualWorkItems[0].TitleName);
            Assert.Equal(expectedWorkItems[0].WorkItemId, actualWorkItems[0].WorkItemId);
            Assert.Equal(expectedWorkItems[0].WorkItemStatusDescription, actualWorkItems[0].WorkItemStatusDescription);
            Assert.Equal(expectedWorkItems[0].WorkItemTypeDescription, actualWorkItems[0].WorkItemTypeDescription);

            Assert.Equal(expectedWorkItems[1].CreatedDate, actualWorkItems[1].CreatedDate);
            Assert.Equal(expectedWorkItems[1].EbscoOrderNumber, actualWorkItems[1].EbscoOrderNumber);
            Assert.Equal(expectedWorkItems[1].Owner, actualWorkItems[1].Owner);
            Assert.Equal(expectedWorkItems[1].TitleName, actualWorkItems[1].TitleName);
            Assert.Equal(expectedWorkItems[1].WorkItemId, actualWorkItems[1].WorkItemId);
            Assert.Equal(expectedWorkItems[1].WorkItemStatusDescription, actualWorkItems[1].WorkItemStatusDescription);
            Assert.Equal(expectedWorkItems[1].WorkItemTypeDescription, actualWorkItems[1].WorkItemTypeDescription);
        }

        [Fact]
        public void return_empty_array_when_no_work_items_exist_for_customer_code()
        {
            var workItemsFromRepository = new WorkItem[0];

            var expectedWorkItems = workItemsFromRepository.Select(w => w.MapWorkItemToWorkItemHeaderModel()).ToList();

            var response = GetAllWorkItems(workItemsFromRepository) as OkObjectResult;

            var actualWorkItems = (((response.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemHeaderInfoModel>).Value as WorkItemHeaderInfoModel).WorkItems;
            
            Assert.Equal(200, response.StatusCode);
            Assert.Empty(actualWorkItems);
        }

        private IActionResult GetAllWorkItems(WorkItem[] workItems)
        {
            var workItemTrackerServiceRepositoryMock = new Mock<IWorkItemTrackerServiceRepository>();

            workItemTrackerServiceRepositoryMock.Setup(repository => repository
                           .GetAllWorkItems(It.IsAny<string>()))
                           .Returns(new GetWorkItemsResponse { WorkItems = workItems });

            var translationsServiceMock = new Mock<ITranslationsService>();

            translationsServiceMock.Setup(repository => repository
                           .TranslateResource(It.IsAny<string>()))
                           .Returns(Any.String());

            var controller = new WorkItemsController(
                    workItemTrackerServiceRepositoryMock.Object,
                    translationsServiceMock.Object,
                    new Mock<IUrlHelper>().Object);

            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = Mock.Of<ClaimsPrincipal>(c => c.FindFirst("customercode") == Any.Claim())
            };

            return controller.GetAllWorkItems(Any.String());
        }
    }
}
