using System;
using Unity.Collections;
using Unity.Jobs;

namespace H1M4W4R1.LUNA.Utilities.Jobs
{
    /// <summary>
    /// Represents Job running in managed scripts.
    /// Makes it easier to handle them due to better support.
    /// </summary>
    /// <typeparam name="T">Job reference value type (for none use null)</typeparam>
    public class ManagedJob<T> : IManagedJob where T : class
    {
        public readonly IJob job;
        public JobHandle handle;
        public readonly T refValue;

        /// <summary>
        /// Execute this event when job is completed
        /// </summary>
        private Action _onJobCompleted = () =>
        {
            // Do nothing (wait for listeners)
        };
        
        public bool IsCompleted => handle.IsCompleted;
        
        public ManagedJob(IJob job, JobHandle handle, T refValue) 
        {
            this.job = job;
            this.handle = handle;
            this.refValue = refValue;
        }

        public ManagedJob<T> RegisterCompleteEvent<TSelf>(Action<TSelf> a) where TSelf : ManagedJob<T>
        {
            // INFO: this looks weird
            _onJobCompleted += () =>
            {
                a.Invoke((TSelf) this);
            };
            return this;
        }

        public void Kill()
        {
            handle.Complete();
        }

        public void Finish()
        {
            // Make sure job is complete to stop Unity from bitching about that.
            handle.Complete();
            
            // Process event
            _onJobCompleted?.Invoke();
        }

        public TJ GetJob<TJ>() where TJ : IJob => (TJ) job;
        
        public void Dispose()
        {
            // Dispose job if should be done so
            if (job is INativeDisposable disposable)
                disposable.Dispose();
        }

        public T GetReference() => refValue;
    }

    public interface IManagedJob : IDisposable
    {
        bool IsCompleted { get; }

        void Kill();
        
        void Finish();

        TJ GetJob<TJ>() where TJ : IJob;
    }
}