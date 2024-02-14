using Unity.Jobs;

namespace H1M4W4R1.LUNA.Weapons.Jobs
{
    public class LinkedJob<T> where T : class
    {
        public readonly IJob job;
        public JobHandle handle;
        public readonly T refValue;

        public LinkedJob(IJob job, JobHandle handle, T refValue) 
        {
            this.job = job;
            this.handle = handle;
            this.refValue = refValue;
        }
    }
}