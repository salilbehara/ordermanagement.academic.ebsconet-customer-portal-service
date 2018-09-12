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
    public class WorkItemCommentsController_GetWorkItemComments_should_
    {
        private Mock<IWorkItemTrackerServiceRepository> _workItemServiceRepositoryMock = new Mock<IWorkItemTrackerServiceRepository>();

        [Fact]
        public void return_empty_array_when_no_comments_exist_for_work_item()
        {
            var workItemId = Any.Int();

            _workItemServiceRepositoryMock.Setup(repository => repository
                 .GetAllWorkItemComments(workItemId))
                 .Returns(new GetWorkItemCommunicationsResponse { Communications = new Communication[0] });
            GetAllWorkItemComments(workItemId);

            var httpResponse = GetAllWorkItemComments(workItemId) as OkObjectResult;
            Assert.Equal(200, httpResponse.StatusCode);

            var actualWorkItemComments = (((httpResponse.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemCommentInfoModel>).Value as WorkItemCommentInfoModel).WorkItemComments;
            Assert.Empty(actualWorkItemComments);
        }

        [Fact]
        public void return_all_comments_for_a_workitemid()
        {
            var workItemId = Any.Int();
            var workItemCommentsFromWit = Any.Communications(length: 2, workItemId: workItemId);

            var expectedWorkItemComments = new List<WorkItemCommentModel>
            {
                workItemCommentsFromWit[0].MapCommunicationToWorkItemCommentModel(),
                workItemCommentsFromWit[1].MapCommunicationToWorkItemCommentModel()
            };

            _workItemServiceRepositoryMock.Setup(repository => repository
                .GetAllWorkItemComments(workItemId))
                .Returns(new GetWorkItemCommunicationsResponse { Communications = workItemCommentsFromWit });

            var httpResponse = GetAllWorkItemComments(workItemId) as OkObjectResult;
            Assert.Equal(200, httpResponse.StatusCode);

            var actualWorkItemComments = (((httpResponse.Value as ApiOkResponse).Result as ResourceWrapper<WorkItemCommentInfoModel>).Value as WorkItemCommentInfoModel).WorkItemComments;
            Assert.Equal(expectedWorkItemComments[0].WorkItemId, actualWorkItemComments[0].WorkItemId);
            Assert.Equal(expectedWorkItemComments[0].CommentId, actualWorkItemComments[0].CommentId);
            Assert.Equal(expectedWorkItemComments[0].CreatedByUserName, actualWorkItemComments[0].CreatedByUserName);
            Assert.Equal(expectedWorkItemComments[0].CreatedDate, actualWorkItemComments[0].CreatedDate);
            Assert.Equal(expectedWorkItemComments[0].Details, actualWorkItemComments[0].Details);

            Assert.Equal(expectedWorkItemComments[1].WorkItemId, actualWorkItemComments[1].WorkItemId);
            Assert.Equal(expectedWorkItemComments[1].CommentId, actualWorkItemComments[1].CommentId);
            Assert.Equal(expectedWorkItemComments[1].CreatedByUserName, actualWorkItemComments[1].CreatedByUserName);
            Assert.Equal(expectedWorkItemComments[1].CreatedDate, actualWorkItemComments[1].CreatedDate);
            Assert.Equal(expectedWorkItemComments[1].Details, actualWorkItemComments[1].Details);

        }

        private IActionResult GetAllWorkItemComments(int workItemId)
        {
            return new WorkItemCommentsController(
                                _workItemServiceRepositoryMock.Object,
                                new Mock<IUrlHelper>().Object)
                            .GetAllWorkItemComments(workItemId);
        }
    }
}
