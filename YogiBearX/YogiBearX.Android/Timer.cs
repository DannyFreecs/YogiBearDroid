using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using YogiBearX.ViewModel;

[assembly: Xamarin.Forms.Dependency(typeof(Timer))]
namespace YogiBearX.Droid
{
    public sealed class Timer : System.Timers.Timer, ITimer
    {
        public new event EventHandler Elapsed;

        public Timer()
        {
            base.Elapsed += Base_Elapsed;
        }

        public Timer(double interval)
          : base(interval)
        {
            base.Elapsed += Base_Elapsed;
        }

        public void Reset()
        {
            Stop();
            Start();
        }

        private bool m_disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }

            base.Elapsed -= Base_Elapsed;
            m_disposed = true;

            base.Dispose(disposing);
        }

        private void Base_Elapsed(object sender, EventArgs e)
        {
            if (Elapsed != null)
            {
                Elapsed(this, e);
            }
        }
    }
}