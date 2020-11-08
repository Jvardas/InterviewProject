using System;
using System.Collections.Generic;

namespace IPInfoWebAPI.Services
{
    public class BatchJobHelperService : IBatchJobHelperService
    {
        public Dictionary<Guid, string> JobsProgress { get; set; }

        public BatchJobHelperService()
        {
            // ensure that my dictionary will be created
            JobsProgress = new Dictionary<Guid, string>();
        }

        public void UpdateJobProgress(Guid jobGuid, string progress)
        {
            // add the progress update if the guid is already in the dictionary, else add a new dictionary entry
            if(JobsProgress.ContainsKey(jobGuid))
            {
                JobsProgress[jobGuid] = progress;
            }
            else
            {
                JobsProgress.Add(jobGuid, progress);
            }
        }

        public string GetJobProgress(Guid jobGuid)
        {
            // return the progress or null if the guid is not in the dictionary
            if (JobsProgress.ContainsKey(jobGuid))
            {
                return JobsProgress[jobGuid];
            }
            else
            {
                return null;
            }
        }
    }
}
