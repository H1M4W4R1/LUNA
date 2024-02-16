using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

namespace H1M4W4R1.LUNA.Utilities.Jobs
{
    public class JobManager : MonoBehaviour
    {
        private static JobManager _instance;

        public static JobManager Instance
        {
            get
            {
                if (!_instance) _instance = FindAnyObjectByType<JobManager>();
                return _instance;
            }
        }
        
        /// <summary>
        /// List of all available jobs
        /// </summary>
        private readonly List<IManagedJob> _jobs = new List<IManagedJob>();

        /// <summary>
        /// Check if manager has specified job active
        /// </summary>
        /// <typeparam name="T">Job type</typeparam>
        /// <returns>True if job is running, false otherwise</returns>
        public static bool HasJob<T>() => Instance._jobs.Any(q => q is T);

        public static IManagedJob ScheduleJob<T, TJ, TSelf>(TJ job, T refObject = default,
            Action<TSelf> onCompleted = null) 
            where TJ : struct, IJob 
            where T : class 
            where TSelf : ManagedJob<T>
        {
            // Schedule job and create object
            var handle = job.Schedule();
            var mJob = new ManagedJob<T>(job, handle, refObject);

            // Register event if exists
            if (onCompleted != null)
                mJob.RegisterCompleteEvent(onCompleted);

            // Store job for execution
            Instance._jobs.Add(mJob);
            
            return mJob;
        }

        private void Awake()
        {
            _instance = this;
        }

        public void Update()
        {
            var jobsToRemove = new List<IManagedJob>();

            for (var index = 0; index < _jobs.Count; index++)
            {
                var job = _jobs[index];
                if (!job.IsCompleted) continue;

                // Complete the job
                job.Finish();
                jobsToRemove.Add(job);
            }

            foreach (var job in jobsToRemove)
                _jobs.Remove(job);
        }

        private void OnDestroy()
        {
            // Get rid of old jobs
            foreach (var job in _jobs)
            {
                try
                {
                    job.Kill();
                    job.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // Do nothing, we should be fine.
                }
            }
        }
    }
}