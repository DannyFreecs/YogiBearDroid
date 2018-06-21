using System;

namespace YogiBearX.ViewModel
{
    public interface ITimer
    {
        // unfortunately we have to use our own event args since we don't have access to the real one
        event EventHandler Elapsed;

        bool AutoReset { get; set; }
        bool Enabled { get; set; }
        double Interval { get; set; }

        void Start();
        void Stop();
        void Reset();
    }
}
