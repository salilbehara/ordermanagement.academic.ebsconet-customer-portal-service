using ebsco.svc.customerserviceportal.Models;
using MediaServerService;

namespace ebsco.svc.customerserviceportal.Repositories
{
    public interface IMediaServerServiceRepository
    {
        GetFileStreamResponse DownloadWorkItemFile(string workItemFileId);
        string AddWorkItemFile(WorkItemFileCreateModel workItemFile);
    }
}
