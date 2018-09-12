using ebsco.svc.customerserviceportal.Models;
using System.Linq;
using WorkItemTracker;

namespace ebsco.svc.customerserviceportal.Extensions
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Map the model object 'WorkItemHeaderInfoModel' to 'WorkItem'
        /// </summary>
        public static WorkItemHeaderModel MapWorkItemToWorkItemHeaderModel(this WorkItem workitem)
        {
            return new WorkItemHeaderModel()
            {
                WorkItemId = workitem.WorkItemId,
                WorkItemStatusDescription = workitem.WorkItemStatusDescription.NullSafeTrim(),
                TitleName = workitem.Title?.TitleName.NullSafeTrim(),
                EbscoOrderNumber = workitem.Order?.EbscoOrderNumber.NullSafeTrim(),
                WorkItemTypeDescription = workitem.WorkItemTypeDescription.NullSafeTrim(),
                CreatedDate = workitem.CreatedDate,                
                Owner = $"{workitem.WebUserInformation?.FirstName} {workitem.WebUserInformation?.LastName}".NullSafeTrim()              
            };
        }

        /// <summary>
        /// Map the model object 'WorkItemDetailInfoModel' to 'WorkItemDetailInfo'
        /// </summary>
        public static WorkItemDetailInfoModel MapWorkItemDetailInfoToWorkItemDetailInfoModel(this WorkItemDetailInfo workItemDetailInfo)
        {
            var workItemDetailInfoModel = new WorkItemDetailInfoModel()
            {
                WorkItemDetails = workItemDetailInfo.Details?
                    .Where(d => !string.IsNullOrEmpty(d.DetailValue))
                    .Select(d => new WorkItemDetailModel
                    {
                        Name = d.DetailDescription,
                        Value = d.DetailValue,
                        TranslationKey = (d.RequestInformationEntity?.TranslationKey ?? string.Empty),
                        SortOrder = d.Order
                    })
                    .OrderBy(d => d.SortOrder)
                    .ToArray()
            };

            if (!string.IsNullOrEmpty(workItemDetailInfo.NoteText))
            {
                var note = new WorkItemDetailModel
                {
                    Name = workItemDetailInfo.NoteLabel.NullSafeTrim(),
                    Value = workItemDetailInfo.NoteText.NullSafeTrim(),
                    TranslationKey = workItemDetailInfo.NoteLabelTranslationKey.NullSafeTrim(),
                };
                
                workItemDetailInfoModel.WorkItemDetails = workItemDetailInfoModel.WorkItemDetails.Concat(new WorkItemDetailModel[] { note }).ToArray();
            }

            return workItemDetailInfoModel;
        }

        public static WorkItemFileModel MapWorkItemDocumentToWorkItemFileModel(this WorkItemDocument workItemDocument)
        {
            return new WorkItemFileModel
            {
                FileName = workItemDocument.DocumentName ?? string.Empty,
                FileId = workItemDocument.DocumentId ?? string.Empty,
                CreatedDate = workItemDocument.CreatedDate,
                DeleteFlag = workItemDocument.DeleteFlag
            };
        }

        public static WorkItemCommentModel MapCommunicationToWorkItemCommentModel(this Communication workItemCommunication)
        {
            return new WorkItemCommentModel
            {
                WorkItemId = workItemCommunication.WorkItemId,
                CommentId = workItemCommunication.CommunicationId, 
                CreatedByUserName = workItemCommunication.CreatedByUserName ?? string.Empty,
                CreatedDate = workItemCommunication.CreatedDate,
                Details = workItemCommunication.Details ?? string.Empty
            };
        }
    }
}
