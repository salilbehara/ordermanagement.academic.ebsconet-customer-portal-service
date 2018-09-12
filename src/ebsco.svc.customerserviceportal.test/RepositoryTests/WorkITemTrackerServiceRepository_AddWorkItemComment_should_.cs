using Xunit;
using Moq;
using ebsco.svc.customerserviceportal.Repositories;
using ebsco.svc.customerserviceportal.Models;
using WorkItemTracker;
using System.Threading.Tasks;
using System;

namespace ebsco.svc.customerserviceportal.test.RepositoryTests
{
    public class WorkItemTrackerServiceRepository_AddWorkItemComment_should_
    {
        private int _workItemId = Any.Int();

        private WorkItemUserDetails _workItemUserDetails = Any.WorkItemUserDetails();

        private WorkItemCommentCreateModel _communicationModel = Any.WorkItemCommentCreateModel();

        [Fact]
        public void add_work_item_communication()
        {
            var communicationRequest = new AddWorkItemCommunicationRequest
            {
                Communication = new Communication
                {
                    WorkItemId = _workItemId,
                    ContactName = _workItemUserDetails.Name,
                    ContactEmail = _workItemUserDetails.Email,
                    ContactPhone = _workItemUserDetails.Phone,
                    ContactFax = _workItemUserDetails.Fax,
                    CommunicationFromContactTypeId = (int)_communicationModel.CommentFromContactTypeId,
                    CommunicationMethodId = (int)_communicationModel.CommentMethodId,
                    CommunicationToContactTypeId = (int)_communicationModel.CommentToContactTypeId,
                    CreatedBy = _workItemUserDetails.Id,
                    Details = _communicationModel.Details,
                    IsInCommunication = _communicationModel.IsInComment,
                    Organization = _communicationModel.Organization
                },
                CorrelationId = Any.String()
            };

            var expectedResponse = new AddWorkItemCommunicationResponse { Success = true };
            var workItemServiceMock = new Mock<IWorkItemService>();

            workItemServiceMock.Setup(settings => settings
                .AddWorkItemCommunicationAsync(It.IsAny<AddWorkItemCommunicationRequest>()))
                .Returns(Task.FromResult(expectedResponse));

            var actualResponse = AddWorkItemComment(() => { return workItemServiceMock.Object; }, _communicationModel);
            Assert.Equal(expectedResponse, actualResponse);
        }

        private AddWorkItemCommunicationResponse AddWorkItemComment(Func<IWorkItemService> service, WorkItemCommentCreateModel communicationRequest)
        {
            return new WorkItemTrackerServiceRepository(service).CreateWorkItemCommunication(_workItemId, _workItemUserDetails, communicationRequest);
        }
    }
}