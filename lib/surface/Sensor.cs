using System;
using System.Reflection;
using OpenEL;

namespace OpenEL {

	unsafe public class Sensor : HALComponent {

		public Sensor(HALId halId, bool exec = true) {
			hALId = halId;
			if (exec == true) {
				Common common = new Common();

				hALComponent = common.HalCreate(hALId.vendorId,
									   	    	hALId.productId,
												hALId.instanceId);
			}
			GC.SuppressFinalize(this);
		}

		~Sensor() {
			Common common = new Common();
			common.HalDestroy(ref hALComponent);
		}

		public ReturnCode GetValueList(float[] valueList, ref int num) {
			Common.HAL_HANDLER_T handler;
			ReturnCode retCode;
			object[] args = new object[] { valueList, null };
			object[] ctor = new object[] { hALId, false };

			handler = Common.HalHandlerTbl[hALComponent.handle];
			Type type = Type.GetType(handler.name);
			MethodInfo method = type.GetMethod("fncGetValLst", new Type[] { typeof(float[]), typeof(int).MakeByRefType() });
			retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), args);
			num = (int) args[1];

			return retCode;
		}

		public ReturnCode GetTimedValueList(float[] valueList, ref uint time, ref int num) {
			Common.HAL_HANDLER_T handler;
			ReturnCode retCode;
				object[] args = new object[] { valueList, null, null };
				object[] ctor = new object[] { hALId, false };

				handler = Common.HalHandlerTbl[hALComponent.handle];
				Type type = Type.GetType(handler.name);
				MethodInfo method = type.GetMethod("fncGetTimedValLst", new Type[] { typeof(float[]), typeof(uint).MakeByRefType(), typeof(int).MakeByRefType() });
				retCode = (ReturnCode)method.Invoke(Activator.CreateInstance(type, ctor), args);
				time = (uint) args[1];
				num = (int) args[2];

				return retCode;
		}

	}

}

