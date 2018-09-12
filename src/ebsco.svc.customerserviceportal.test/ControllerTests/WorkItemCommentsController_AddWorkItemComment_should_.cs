using ebsco.svc.customerserviceportal.Controllers;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.customerserviceportal.Models;
using MediaServerService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using WorkItemTracker;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ebsco.svc.customerserviceportal.test.ControllerTests
{
    public class WorkItemCommentsController_AddWorkItemComment_should_
    {
        private Mock<IWorkItemTrackerServiceRepository> _workItemTrackerServiceRepositoryMock = new Mock<IWorkItemTrackerServiceRepository>();
        

        [Fact]
        public void return_201_when_comment_added_successfully()
        {
            var workItemId = Any.Int();

            var workItemComment = Any.WorkItemCommentCreateModel();

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                    .CreateWorkItemCommunication(workItemId, It.IsAny<WorkItemUserDetails>(), workItemComment))
                    .Returns(new AddWorkItemCommunicationResponse { Success = true, CommunicationId = Any.Int() });

            var httpResponse = AddWorkItemCommunication(workItemId, workItemComment) as ObjectResult;
            Assert.Equal(201, httpResponse.StatusCode);
        }

        [Fact]
        public void return_400_when_comment_is_invalid()
        {
            var workItemId = Any.Int();

            var workItemComment = Any.WorkItemCommentCreateModel();

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                .CreateWorkItemCommunication(workItemId, It.IsAny<WorkItemUserDetails>(), workItemComment))
                .Returns(new AddWorkItemCommunicationResponse { Success = false, CommunicationId = Any.Int() });

            var response = AddWorkItemCommunication(workItemId, workItemComment) as BadRequestObjectResult;

            Assert.Equal(400, response.StatusCode);
        }

        [Fact]
        public void return_400_when_comment_is_null()
        {
            var workItemId = Any.Int();

            var workItemUserDetails = Any.WorkItemUserDetails();

            WorkItemCommentCreateModel workItemComment = null;

            _workItemTrackerServiceRepositoryMock.Setup(repository => repository
                .CreateWorkItemCommunication(workItemId, workItemUserDetails, null))
                .Returns(new AddWorkItemCommunicationResponse { Success = false, CommunicationId = Any.Int() });

            var response = AddWorkItemCommunication(workItemId, workItemComment) as BadRequestObjectResult;

            Assert.Equal(400, response.StatusCode);
        }

        private IActionResult AddWorkItemCommunication(int workItemId, WorkItemCommentCreateModel commentModel)
        {
            var workItemCommentsController = new WorkItemCommentsController(
                _workItemTrackerServiceRepositoryMock.Object,
                new Mock<IUrlHelper>().Object);

            workItemCommentsController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = Mock.Of<ClaimsPrincipal>(c => c.FindFirst(It.IsAny<string>()) == Any.Claim())
            };

            return workItemCommentsController.CreateWorkItemComment(workItemId, commentModel);
        }
    }
}
