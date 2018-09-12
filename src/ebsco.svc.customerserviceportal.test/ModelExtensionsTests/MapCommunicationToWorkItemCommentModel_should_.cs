using ebsco.svc.customerserviceportal.Extensions;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ModelExtensionsTests
{
    public class MapCommunicationToWorkItemCommentModel_should_
    {
        [Fact]
        public void convert_work_item_communication_to_work_item_comment_model()
        {
            var workItemCommunication = Any.Communication();
            var convertedWorkItemCommentModel = workItemCommunication.MapCommunicationToWorkItemCommentModel();

            Assert.Equal(workItemCommunication.WorkItemId, convertedWorkItemCommentModel.WorkItemId);
            Assert.Equal(workItemCommunication.CommunicationId, convertedWorkItemCommentModel.CommentId);
            Assert.Equal(workItemCommunication.CreatedByUserName, convertedWorkItemCommentModel.CreatedByUserName);
            Assert.Equal(workItemCommunication.CreatedDate, convertedWorkItemCommentModel.CreatedDate);
            Assert.Equal(workItemCommunication.Details, convertedWorkItemCommentModel.Details);
        }

        [Fact]
        public void convert_null_string_values_to_empty_string()
        {
            var workItemCommunication = new Communication
            {
                CreatedByUserName = null,
                Details = null
            };

            var convertedWorkItemCommentModel = workItemCommunication.MapCommunicationToWorkItemCommentModel();

            Assert.Equal(string.Empty, convertedWorkItemCommentModel.CreatedByUserName);
            Assert.Equal(string.Empty, convertedWorkItemCommentModel.Details);
        }
    }
}
