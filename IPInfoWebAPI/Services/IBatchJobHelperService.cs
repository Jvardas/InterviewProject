using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPInfoWebAPI.Services
{
    public interface IBatchJobHelperService
    {
        public Dictionary<Guid, string> JobsProgress { get; set; }

        void UpdateJobProgress(Guid jobGuid, string progress);

        string GetJobProgress(Guid jobGuid);
    }
}
