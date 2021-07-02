using System;
using System.Reflection;
using OpenEL;

namespace OpenEL {

	unsafe public class Actuator : HALComponent {

		public Actuator(HALId halId, bool exec = true) {
			hALId = halId;
			if (exec == true) {
				Common common = new Common();

				hALComponent = common.HalCreate(hALId.vendorId,
									   	    	hALId.productId,
												hALId.instanceId);
			}
			GC.SuppressFinalize(this);
		}

		~Actuator() {
			Common common = new Common();
			common.HalDestroy(ref hALComponent);
		}

		public ReturnCode GetValue(ref float value, int command) {
			Common.HAL_HANDLER_T handler;
			ReturnCode retCode;
			object[] args = new object[] { null, command };
			object[] ctor = new object[] { hALId, false };

			handler = Common.HalHandlerTbl[hALComponent.handle];
			Type type = Type.GetType(handler.name);
			MethodInfo method = type.GetMethod("fncGetVal", new Type[] { typeof(float).MakeByRefType(), typeof(int) });
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), args);
			value = (float) args[0];

			return retCode;
		}

		public ReturnCode SetValue(float value, int command) {
			Common.HAL_HANDLER_T handler;
			ReturnCode retCode;
			object[] args = new object[] { value, command };
			object[] ctor = new object[] { hALId, false };

			handler = Common.HalHandlerTbl[hALComponent.handle];
			Type type = Type.GetType(handler.name);
			MethodInfo method = type.GetMethod("fncSetVal", new Type[] { typeof(float), typeof(int) });
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), args);

			return retCode;
		}

	}

}

