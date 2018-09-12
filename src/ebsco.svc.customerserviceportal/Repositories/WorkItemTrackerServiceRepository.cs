using System;
using ebsco.svc.customerserviceportal.Helpers;
using ebsco.svc.customerserviceportal.Models;
using WorkItemTracker;

namespace ebsco.svc.customerserviceportal.Repositories
{
    public class WorkItemTrackerServiceRepository : IWorkItemTrackerServiceRepository
    {
        private readonly Func<IWorkItemService> _serviceFactory;

        public WorkItemTrackerServiceRepository(Func<IWorkItemService> serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public GetWorkItemsResponse GetAllWorkItems(string customerCode)
        {
            #region Create 'GetWorkItemsRequest' request
            var request = CreateBasicRequest<GetWorkItemsRequest>();

            request.IgnoreIsSearchable = true;

            request.Filter = new WorkItemFilter
            {
                CustomerCode = customerCode,
                WorkItemStatuses = new[] { 1, 2, 3 },
                ExcludeWorkItemTypes = new string[]
                {
                    "CO - ENET Order Now Firm Inv.",
                    "CO - ENET Order Now Non firm"
                }
            };

            request.Sort = WorkItemSort.CreatedDate;
            #endregion

            return ServiceHelper.ExecuteServiceCall(
                _serviceFactory.Invoke(), 
                x => x.GetWorkItemsAsync(request));
        }

        public GetWorkItemDetailsResponse GetWorkItemDetails (int workItemId)
        {
            #region Create 'GetWorkItemDetailsRequest' request
            var request = CreateBasicRequest<GetWorkItemDetailsRequest>();

            request.WorkItemId = workItemId;
            #endregion

            return ServiceHelper.ExecuteServiceCall(
                _serviceFactory.Invoke(),
                x => x.GetWorkItemDetailsAsync(request));
        }

        public GetAllWorkItemDocumentsResponse GetAllWorkItemFiles(int workItemId)
        {
            return ServiceHelper.ExecuteServiceCall(
                _serviceFactory.Invoke(),
                x => x.GetAllWorkItemDocumentsAsync(new GetAllWorkItemDocumentsRequest
                {
                    WorkItemId = workItemId
                })
            );
        }

        public GetWorkItemDocumentResponse GetWorkItemFile(int workItemId, string workItemFileId)
        {
            return ServiceHelper.ExecuteServiceCall(
                _serviceFactory.Invoke(),
                x => x.GetWorkItemDocumentAsync(new GetWorkItemDocumentRequest
                {
                    WorkItemId = workItemId,
                    DocumentId = workItemFileId
                })
            );
        }

        public AddWorkItemCommunicationResponse CreateWorkItemCommunication(int workItemId, WorkItemUserDetails workItemUserDetails, WorkItemCommentCreateModel commentModel)
        {
            #region Create 'AddWorkItemCommunicationRequest' request
            var request = CreateBasicRequest<AddWorkItemCommunicationRequest>();

            const int commentMethodIsEBSCONET = 6;

            const int commentFromContactTypeIsCustomer = 1;

            const int commentToContactTypeIsInternal = 3;

            const string commentOrganization = "EBSCO, Inc.";

            request.Communication = new Communication
            {
                WorkItemId = workItemId,
                CommunicationMethodId = commentModel.CommentMethodId ?? commentMethodIsEBSCONET,
                CommunicationFromContactTypeId = commentModel.CommentFromContactTypeId ?? commentFromContactTypeIsCustomer,
                CommunicationToContactTypeId = commentModel.CommentToContactTypeId ?? commentToContactTypeIsInternal,
                Organization = commentModel.Organization ?? commentOrganization,
                ContactName = workItemUserDetails.Name,
                ContactPhone = workItemUserDetails.Phone,
                ContactEmail = workItemUserDetails.Email,
                ContactFax = workItemUserDetails.Fax,
                Details = commentModel.Details,
                IsInCommunication = commentModel.IsInComment,
                CreatedBy = workItemUserDetails.Id
            };

            request.CorrelationId = Guid.NewGuid().ToString();
            #endregion

            return ServiceHelper.ExecuteServiceCall(
                _serviceFactory.Invoke(),
                x => x.AddWorkItemCommunicationAsync(request)
            );
        }

        public WorkItemDocumentResponse CreateWorkItemFile(int workItemId, string workItemFileId, WorkItemFileCreateModel workItemFile)
        {
            const int workItemDocumentTypeOther = 4;

            return ServiceHelper.ExecuteServiceCall(
                _serviceFactory.Invoke(),
                x => x.AddWorkItemDocumentAsync(new WorkItemDocumentRequest
                {
                    WorkItemDocument = new WorkItemDocument
                    {
                        WorkItemId = workItemId,
                        DocumentId = workItemFileId,
                        WorkItemDocumentTypeId = workItemDocumentTypeOther,
                        DocumentName = workItemFile.WorkItemFileName,
                        DocumentDescription = workItemFile.WorkItemFileName,
                        DeleteFlag = false,
                        IsHyperlink = false,
                        CreatedBy = "System",
                        ModifiedBy = "System",
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    },
                    IsFromEbsconet = true
                })
            );
        }

        public GetWorkItemCommunicationsResponse GetAllWorkItemComments(int workItemId)
        {
            return ServiceHelper.ExecuteServiceCall(
                _serviceFactory.Invoke(),
                x => x.GetWorkItemCommunicationsAsync(new GetWorkItemCommunicationsRequest
                {
                    WorkItemId = workItemId
                })
            );
        }

        private static T CreateBasicRequest<T>() where T : BaseRequest, new()
        {
            return new T
            {
                CallerInfo = new CallerInfo()
                {
                    AppId = "EBSCONet",
                    CorrelationId = new Guid()
                },
                Paging = new Paging
                {
                    Take = 0
                }
            };
        }
    }
}
