using ebsco.svc.customerserviceportal.Models;
using WorkItemTracker;

namespace ebsco.svc.customerserviceportal.Repositories
{
    public interface IWorkItemTrackerServiceRepository
    {
        GetWorkItemsResponse GetAllWorkItems(string customerCode);
        GetWorkItemDetailsResponse GetWorkItemDetails(int workItemId);
        GetAllWorkItemDocumentsResponse GetAllWorkItemFiles(int workItemId);
        GetWorkItemDocumentResponse GetWorkItemFile(int workItemId, string workItemFileId);
        WorkItemDocumentResponse CreateWorkItemFile(int workItemId, string workItemFileId, WorkItemFileCreateModel workItemFile);
        GetWorkItemCommunicationsResponse GetAllWorkItemComments(int workItemId);
        AddWorkItemCommunicationResponse CreateWorkItemCommunication(int workItemId, WorkItemUserDetails workItemUserDetails, WorkItemCommentCreateModel commentModel);
    }
}
