using ebsco.svc.customerserviceportal.Extensions;
using ebsco.svc.customerserviceportal.Models;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ModelExtensionsTests
{
    public class MapWorkItemToWorkItemHeaderInfoModel_should_
    {
        private WorkItem _workItem = Any.WorkItem();
        private WorkItemHeaderModel _expectedWorkItemHeaderModel;

        [Fact]
        public void convert_work_item_to_work_item_header_info()
        {
            var actualWorkItemHeaderInfo = ConvertWorkItemToWorkItemHeaderInfo();

            Assert.Equal(_expectedWorkItemHeaderModel.CreatedDate, actualWorkItemHeaderInfo.CreatedDate);
            Assert.Equal(_expectedWorkItemHeaderModel.EbscoOrderNumber, actualWorkItemHeaderInfo.EbscoOrderNumber);
            Assert.Equal(_expectedWorkItemHeaderModel.Owner, actualWorkItemHeaderInfo.Owner);
            Assert.Equal(_expectedWorkItemHeaderModel.TitleName, actualWorkItemHeaderInfo.TitleName);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemId, actualWorkItemHeaderInfo.WorkItemId);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemStatusDescription, actualWorkItemHeaderInfo.WorkItemStatusDescription);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemTypeDescription, actualWorkItemHeaderInfo.WorkItemTypeDescription);
        }

        [Fact]
        public void not_error_when_order_is_null_on_work_item()
        {
            _workItem.Order = null;
            var actualWorkItemHeaderInfo = ConvertWorkItemToWorkItemHeaderInfo();

            Assert.Equal(_expectedWorkItemHeaderModel.CreatedDate, actualWorkItemHeaderInfo.CreatedDate);
            Assert.Null(actualWorkItemHeaderInfo.EbscoOrderNumber);
            Assert.Equal(_expectedWorkItemHeaderModel.Owner, actualWorkItemHeaderInfo.Owner);
            Assert.Equal(_expectedWorkItemHeaderModel.TitleName, actualWorkItemHeaderInfo.TitleName);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemId, actualWorkItemHeaderInfo.WorkItemId);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemStatusDescription, actualWorkItemHeaderInfo.WorkItemStatusDescription);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemTypeDescription, actualWorkItemHeaderInfo.WorkItemTypeDescription);
        }

        [Fact]
        public void not_error_when_webUserInformation_is_null_on_work_item()
        {
            _workItem.WebUserInformation = null;
            var actualWorkItemHeaderInfo = ConvertWorkItemToWorkItemHeaderInfo();

            Assert.Equal(_expectedWorkItemHeaderModel.CreatedDate, actualWorkItemHeaderInfo.CreatedDate);
            Assert.Equal(_expectedWorkItemHeaderModel.EbscoOrderNumber, actualWorkItemHeaderInfo.EbscoOrderNumber);
            Assert.Equal(string.Empty, actualWorkItemHeaderInfo.Owner);
            Assert.Equal(_expectedWorkItemHeaderModel.TitleName, actualWorkItemHeaderInfo.TitleName);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemId, actualWorkItemHeaderInfo.WorkItemId);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemStatusDescription, actualWorkItemHeaderInfo.WorkItemStatusDescription);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemTypeDescription, actualWorkItemHeaderInfo.WorkItemTypeDescription);
        }

        [Fact]
        public void not_error_when_title_is_null_on_work_item()
        {
            _workItem.Title = null;

            var actualWorkItemHeaderInfo = ConvertWorkItemToWorkItemHeaderInfo();

            Assert.Equal(_expectedWorkItemHeaderModel.CreatedDate, actualWorkItemHeaderInfo.CreatedDate);
            Assert.Equal(_expectedWorkItemHeaderModel.EbscoOrderNumber, actualWorkItemHeaderInfo.EbscoOrderNumber);
            Assert.Equal(_expectedWorkItemHeaderModel.Owner, actualWorkItemHeaderInfo.Owner);
            Assert.Null(actualWorkItemHeaderInfo.TitleName);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemId, actualWorkItemHeaderInfo.WorkItemId);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemStatusDescription, actualWorkItemHeaderInfo.WorkItemStatusDescription);
            Assert.Equal(_expectedWorkItemHeaderModel.WorkItemTypeDescription, actualWorkItemHeaderInfo.WorkItemTypeDescription);
        }

        private WorkItemHeaderModel ConvertWorkItemToWorkItemHeaderInfo()
        {
            _expectedWorkItemHeaderModel = new WorkItemHeaderModel
            {
                CreatedDate = _workItem.CreatedDate,
                EbscoOrderNumber = _workItem.Order?.EbscoOrderNumber.NullSafeTrim(),
                Owner = $"{_workItem.WebUserInformation?.FirstName} {_workItem.WebUserInformation?.LastName}".NullSafeTrim(),
                TitleName = _workItem.Title?.TitleName.NullSafeTrim(),
                WorkItemId = _workItem.WorkItemId,
                WorkItemStatusDescription = _workItem.WorkItemStatusDescription.NullSafeTrim(),
                WorkItemTypeDescription = _workItem.WorkItemTypeDescription.NullSafeTrim()
            };

            return _workItem.MapWorkItemToWorkItemHeaderModel();
        }
    }
}
