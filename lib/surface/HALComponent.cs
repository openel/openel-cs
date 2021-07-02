using System;
using System.Collections.Generic;
using System.Reflection;
using OpenEL;

public enum ReturnCode {
	HAL_OK,
	HAL_ERROR
}

namespace OpenEL {

	unsafe public class HALComponent {

		public int handle;
		public HALId hALId;
		public HALComponent hALComponent;
		public Property property;
		public Common common;
		public delegate void eventDelegate(int id);
		public event eventDelegate handler_event;
		public event eventDelegate handler_error;
		private int eventId = 0;
		private int errorId = 0;

		public int EventId {
			get { return this.eventId; }
			set {
				if (value != 0) {
					for (int i = 0; i < Common.hal_sz_handler_tbl; i++) {
						if (Common.HalHandlerTbl[i].swUsed == 1) {
							if ((hALId.deviceKindId == Common.HalHandlerTbl[i].HalComponent.hALId.deviceKindId) &&
							    (hALId.vendorId == Common.HalHandlerTbl[i].HalComponent.hALId.vendorId) &&
							    (hALId.productId == Common.HalHandlerTbl[i].HalComponent.hALId.productId) &&
							    (hALId.instanceId == Common.HalHandlerTbl[i].HalComponent.hALId.instanceId)) {
								HALComponent halComponent = Common.HalHandlerTbl[i].HalComponent;
								halComponent.eventId = value;
								if (halComponent.handler_event != null) {
									halComponent.handler_event(halComponent.eventId);
								}
								break;
							}
						}
					}
				}
			}
		}

		public int ErrorId {
			get { return this.errorId; }
			set {
				if (value != 0) {
					for (int i = 0; i < Common.hal_sz_handler_tbl; i++) {
						if (Common.HalHandlerTbl[i].swUsed == 1) {
							if ((hALId.deviceKindId == Common.HalHandlerTbl[i].HalComponent.hALId.deviceKindId) &&
							    (hALId.vendorId == Common.HalHandlerTbl[i].HalComponent.hALId.vendorId) &&
							    (hALId.productId == Common.HalHandlerTbl[i].HalComponent.hALId.productId) &&
							    (hALId.instanceId == Common.HalHandlerTbl[i].HalComponent.hALId.instanceId)) {
								HALComponent halComponent = Common.HalHandlerTbl[i].HalComponent;
								halComponent.errorId = value;
								if (halComponent.handler_error != null) {
									halComponent.handler_error(halComponent.errorId);
								}
								break;
							}
						}
					}
				}
			}
		}

		public ReturnCode Init() {
			HALComponent halComponent = Common.HalHandlerTbl[Common.HalIdxHandlerTbl].HalComponent;
			Common.HAL_HANDLER_T Handler;
			ReturnCode retCode;
			object[] ctor = new object[] { hALId, false };

			Handler = Common.HalHandlerTbl[halComponent.handle];
			Type type = Type.GetType(Handler.name);
			MethodInfo method = type.GetMethod("fncInit", new Type[0]);
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), null);
			halComponent.EventId = 1;

			return retCode;
		}

		public ReturnCode ReInit() {
			HALComponent halComponent = Common.HalHandlerTbl[Common.HalIdxHandlerTbl].HalComponent;
			Common.HAL_HANDLER_T Handler;
			ReturnCode retCode;
			object[] ctor = new object[] { hALId, false };

			Handler = Common.HalHandlerTbl[halComponent.handle];
			Type type = Type.GetType(Handler.name);
			MethodInfo method = type.GetMethod("fncReInit", new Type[0]);
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), null);

			return retCode;
		}

		public ReturnCode Finalize() {
			HALComponent halComponent = Common.HalHandlerTbl[Common.HalIdxHandlerTbl].HalComponent;
			Common.HAL_HANDLER_T Handler;
			ReturnCode retCode;
			object[] ctor = new object[] { hALId, false };

			Handler = Common.HalHandlerTbl[halComponent.handle];
			Type type = Type.GetType(Handler.name);
			MethodInfo method = type.GetMethod("fncFinalize", new Type[0]);
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), null);

			return retCode;
		}

		public ReturnCode GetProperty(Property property) {
			HALComponent halComponent = Common.HalHandlerTbl[Common.HalIdxHandlerTbl].HalComponent;
			Common.HAL_HANDLER_T Handler;
			ReturnCode retCode;
			object[] ctor = new object[] { hALId, false };

			Handler = Common.HalHandlerTbl[halComponent.handle];
			Type type = Type.GetType(Handler.name);
			MethodInfo method = type.GetMethod("fncGetProperty", new Type[] { typeof(Property) });
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), new object[] { property });

			return retCode;
		}

		public ReturnCode GetTime(ref uint timeValue) {
			HALComponent halComponent = Common.HalHandlerTbl[Common.HalIdxHandlerTbl].HalComponent;
			Common.HAL_HANDLER_T Handler;
			ReturnCode retCode;
			object[] args = new object[] { null };
			object[] ctor = new object[] { hALId, false };

			Handler = Common.HalHandlerTbl[halComponent.handle];
			Type type = Type.GetType(Handler.name);
			MethodInfo method = type.GetMethod("fncGetTime", new Type[] { typeof(uint).MakeByRefType() });
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), args);
			timeValue = (uint) args[0];

			return retCode;
		}

		public ReturnCode AddObserver(HALObserver target) {
			HALComponent halComponent = Common.HalHandlerTbl[Common.HalIdxHandlerTbl].HalComponent;
			Common.HAL_HANDLER_T Handler;
			ReturnCode retCode;
			object[] ctor = new object[] { hALId, false };

			halComponent.handler_event += target.notify_event;
			halComponent.handler_error += target.notify_error;

			Handler = Common.HalHandlerTbl[halComponent.handle];
			Type type = Type.GetType(Handler.name);
			MethodInfo method = type.GetMethod("fncAddObserver", new Type[0]);
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), null);

			return retCode;
		}

		public ReturnCode RemoveObserver(HALObserver target) {
			HALComponent halComponent = Common.HalHandlerTbl[Common.HalIdxHandlerTbl].HalComponent;
			Common.HAL_HANDLER_T Handler;
			ReturnCode retCode;
			object[] ctor = new object[] { hALId, false };

			halComponent.handler_event -= target.notify_event;
			halComponent.handler_error -= target.notify_error;

			Handler = Common.HalHandlerTbl[halComponent.handle];
			Type type = Type.GetType(Handler.name);
			MethodInfo method = type.GetMethod("fncRemoveObserver", new Type[0]);
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), null);

			return retCode;
		}

  	}

}