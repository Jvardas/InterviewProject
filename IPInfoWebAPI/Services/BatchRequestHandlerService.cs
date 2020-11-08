using IPInfoWebAPI.Models;
using IPInfoWebAPI.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public class BatchRequestHandlerService : IBatchRequestHandlerService
    {
        private IBatchJobHelperService BatchJobHelperService { get; }
        private IIPRequestHandlerService IpRequestHandlerService { get; }
        private IServiceScopeFactory ServiceScopeFactory { get; }

        private const int batchSize = 10;

        public BatchRequestHandlerService(IBatchJobHelperService batchJobHelperService, IIPRequestHandlerService ipRequestHandlerService, IServiceScopeFactory serviceScopeFactory)
        {
            BatchJobHelperService = batchJobHelperService;
            IpRequestHandlerService = ipRequestHandlerService;
            ServiceScopeFactory = serviceScopeFactory;
        }

        public string GetUpdateJobProgress(Guid updateJobGuid)
        {
            return BatchJobHelperService.GetJobProgress(updateJobGuid);
        }

        public Guid IpDetailsUpdateJob(IEnumerable<IpDetail> ipDetails)
        {
            Guid guid = Guid.NewGuid();

            // initialize the progress
            BatchJobHelperService.UpdateJobProgress(guid, $"0/{ipDetails.Count()}");

            var t = Task.Run(async () =>
            {
                using var scope = ServiceScopeFactory.CreateScope();
                
                var _ipDetailsRepository = scope.ServiceProvider.GetRequiredService<IIPDetailsRepository>();

                var currentIds = ipDetails;

                int progress = 0;

                while (currentIds.Any())
                {
                    // update entries in DB
                    await _ipDetailsRepository.UpdateIpDetailsAsync(currentIds.Take(batchSize));

                    // if the request has less items that the batchsize then i want the progress to write it correctly
                    progress += batchSize > currentIds.Count() ? currentIds.Count() : batchSize;

                    currentIds = currentIds.Skip(batchSize);

                    // update progress
                    BatchJobHelperService.UpdateJobProgress(guid, $"{progress}/{ipDetails.Count()}");

                    // update cache
                    await IpRequestHandlerService.UpdateManyCacheAsync(currentIds.Take(batchSize));
                }

            });

            return guid;
        }
    }
}
