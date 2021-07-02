using System;
using System.Timers;
using OpenEL;

namespace OpenEL {

	public interface TimerObserver {

		void notify_timer(object sender, ElapsedEventArgs e);

	}

}

