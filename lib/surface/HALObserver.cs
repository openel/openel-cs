using System;
using OpenEL;

namespace OpenEL {

	public interface HALObserver {

		void notify_error(int error_id);
		
		void notify_event(int event_id);

	}

}

