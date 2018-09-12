using ebsco.svc.customerserviceportal.Extensions;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ModelExtensionsTests
{
    public class MapWorkItemDocumentToWorkItemFileModel_should_
    {
        [Fact]
        public void convert_work_item_document_to_work_item_file_info()
        {
            var workItemDocument = Any.WorkItemDocument();
            var convertedWorkItemFileInfo = workItemDocument.MapWorkItemDocumentToWorkItemFileModel();

            Assert.Equal(workItemDocument.DocumentName, convertedWorkItemFileInfo.FileName);
            Assert.Equal(workItemDocument.DocumentId, convertedWorkItemFileInfo.FileId);
            Assert.Equal(workItemDocument.CreatedDate, convertedWorkItemFileInfo.CreatedDate);
            Assert.Equal(workItemDocument.DeleteFlag, convertedWorkItemFileInfo.DeleteFlag);
        }

        [Fact]
        public void convert_null_string_values_to_empty_string()
        {
            var workItemDocument = new WorkItemDocument();
            var convertedWorkItemFileInfo = workItemDocument.MapWorkItemDocumentToWorkItemFileModel();

            Assert.Equal(string.Empty, convertedWorkItemFileInfo.FileName);
            Assert.Equal(string.Empty, convertedWorkItemFileInfo.FileId);
        }
    }
}
