using ebsco.svc.customerserviceportal.Extensions;
using WorkItemTracker;
using Xunit;

namespace ebsco.svc.customerserviceportal.test.ModelExtensionsTests
{
    public class MapWorkItemDetailInfoToWorkItemDetailInfoModel_should_
    {
        [Fact]
        public void Convert_work_item_detail_info_to_work_item_detail_info_model()
        {
            var workItemDetailInfo = new WorkItemDetailInfo
            {
                NoteLabel = "Test Note Label",
                NoteText = "Test Note Text",
                NoteLabelTranslationKey = "Test Note Label Translation Key",
                Details = new WorkItemDetail[]
                {
                    new WorkItemDetail
                    {
                        DetailDescription = "Test Detail Description",
                        DetailValue = "Test Detail Value",
                        Order = 1,
                        RequestInformationEntity = new RequestInformation
                        {
                            TranslationKey = "Test Detail Translation Key"
                        }
                    }
                }
            };
            
            var actualWorkItemDetailInfoModel = workItemDetailInfo.MapWorkItemDetailInfoToWorkItemDetailInfoModel();

            Assert.Equal("Test Detail Description", actualWorkItemDetailInfoModel.WorkItemDetails[0].Name);
            Assert.Equal("Test Detail Value", actualWorkItemDetailInfoModel.WorkItemDetails[0].Value);
            Assert.Equal(1, actualWorkItemDetailInfoModel.WorkItemDetails[0].SortOrder);
            Assert.Equal("Test Detail Translation Key", actualWorkItemDetailInfoModel.WorkItemDetails[0].TranslationKey);
            Assert.Equal("Test Note Label", actualWorkItemDetailInfoModel.WorkItemDetails[1].Name);
            Assert.Equal("Test Note Text", actualWorkItemDetailInfoModel.WorkItemDetails[1].Value);
            Assert.Equal("Test Note Label Translation Key", actualWorkItemDetailInfoModel.WorkItemDetails[1].TranslationKey);
        }

        [Fact]
        public void Not_error_when_work_item_detail_info_is_null()
        {
            var workItemDetailInfo = new WorkItemDetailInfo
            {
                NoteLabel = null,
                NoteText = null,
                NoteLabelTranslationKey = null,
                Details = new WorkItemDetail[]
                {
                    new WorkItemDetail
                    {
                        DetailDescription = "Test Detail Description",
                        DetailValue = "Test Detail Value",
                        Order = 1,
                        RequestInformationEntity = new RequestInformation
                        {
                            TranslationKey = "Test Detail Translation Key"
                        }
                    }
                }
            };

            var actualWorkItemDetailInfoModel = workItemDetailInfo.MapWorkItemDetailInfoToWorkItemDetailInfoModel();

            Assert.Single(actualWorkItemDetailInfoModel.WorkItemDetails);
        }

        [Fact]
        public void Not_error_when_translation_key_is_null()
        {
            var workItemDetailInfo = new WorkItemDetailInfo
            {
                NoteLabel = null,
                NoteText = null,
                NoteLabelTranslationKey = null,
                Details = new WorkItemDetail[]
                {
                    new WorkItemDetail
                    {
                        DetailDescription = "Test Detail Description",
                        DetailValue = "Test Detail Value",
                        Order = 1,
                        RequestInformationEntity = new RequestInformation
                        {
                            TranslationKey = null
                        }
                    }
                }
            };

            var actualWorkItemDetailInfoModel = workItemDetailInfo.MapWorkItemDetailInfoToWorkItemDetailInfoModel();

            Assert.Equal(string.Empty, actualWorkItemDetailInfoModel.WorkItemDetails[0].TranslationKey);
        }

        [Fact]
        public void Not_error_when_work_item_details_is_null()
        {
            var workItemDetailInfo = new WorkItemDetailInfo
            {
                NoteLabel = null,
                NoteText = null,
                NoteLabelTranslationKey = null,
                Details = null
            };

            var actualWorkItemDetailInfoModel = workItemDetailInfo.MapWorkItemDetailInfoToWorkItemDetailInfoModel();

            Assert.Null(actualWorkItemDetailInfoModel.WorkItemDetails);
        }
    }
}
