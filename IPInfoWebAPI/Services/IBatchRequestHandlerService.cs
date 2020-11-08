using IPInfoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public interface IBatchRequestHandlerService
    {
        Guid IpDetailsUpdateJob(IEnumerable<IpDetail> ipDetails);
        string GetUpdateJobProgress(Guid updateJobGuid);
    }
}
