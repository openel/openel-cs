using System;
using System.Timers;
using OpenEL;

namespace OpenEL {

	unsafe public class EventTimer {

		public Timer timer = new Timer();

		public EventTimer(int period = 100) {
			this.timer.Interval = period;
			this.timer.Enabled = false;
		}

		public ReturnCode StartTimer() {
			this.timer.Enabled = true;

			return ReturnCode.HAL_OK;
		}

		public ReturnCode StopTimer() {
			this.timer.Enabled = false;

			return ReturnCode.HAL_OK;
		}

		public ReturnCode SetEventPeriod(int eventPeriod) {
			this.timer.Interval = eventPeriod;

			return ReturnCode.HAL_OK;
		}

		public ReturnCode AddObserver(TimerObserver timerObserver) {
			this.timer.Elapsed += timerObserver.notify_timer;

			return ReturnCode.HAL_OK;
		}

		public ReturnCode RemoveObserver(TimerObserver timerObserver) {
			this.timer.Elapsed -= timerObserver.notify_timer;

			return ReturnCode.HAL_OK;
		}

  	}

}

