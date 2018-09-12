using ebsco.svc.customerserviceportal.Helpers;
using ebsco.svc.customerserviceportal.Models;
using MediaServerService;
using System;

namespace ebsco.svc.customerserviceportal.Repositories
{
    public class MediaServerServiceRepository : IMediaServerServiceRepository
    {
        private readonly Func<IMediaServerService> _serviceFactory;

        public MediaServerServiceRepository(Func<IMediaServerService> serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public GetFileStreamResponse DownloadWorkItemFile(string workItemFileId)
        {            
            return ServiceHelper.ExecuteServiceCall(
                    _serviceFactory.Invoke(),
                    x => x.GetFileStreamAsync(new GetFileStreamRequest
                    {
                        Guid = workItemFileId
                    })
                );            
        }

        public string AddWorkItemFile(WorkItemFileCreateModel workItemFile)
        {
            return ServiceHelper.ExecuteServiceCall(
                    _serviceFactory.Invoke(),
                    x => x.AddFileStreamAsync(new AddFileStreamRequest
                    {
                        FileName = workItemFile.WorkItemFileName,
                        Contents = workItemFile.WorkItemFileContent
                    })
                ).Guid;
        }
    }
}
