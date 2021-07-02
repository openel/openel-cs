using System;
using System.Runtime.InteropServices;
using OpenEL;

namespace OpenEL {

	unsafe public class Common {

		public static readonly int hal_sz_handler_tbl = 256;
		public static readonly int hal_msk_handler_tbl = (hal_sz_handler_tbl - 1);
		public static HAL_HANDLER_T[] HalHandlerTbl = new HAL_HANDLER_T[hal_sz_handler_tbl];
		public static int HalIdxHandlerTbl;

		[StructLayout(LayoutKind.Explicit)]
		public struct HAL_ARGUMENT_T {
			[FieldOffset(0)]
			public long numI64;

			[FieldOffset(0)]
			public int num;
		}

		public struct HAL_HANDLER_T {
			public uint swUsed;
			public HALComponent HalComponent;
			public string name;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HAL_REG_T {
			public int deviceKindID;
			public int vendorID;
			public int productID;
			public string name;

			public HAL_REG_T(int device, int vendor, int product, string name) {
				this.deviceKindID = device;
				this.vendorID = vendor;
				this.productID = product;
				this.name = name;
			}
		}

		public struct HAL_FNCTBL_T {
			/* 0x00 */ public delegate ReturnCode FncInit();																		/**< Initialize */
			/* 0x01 */ public delegate ReturnCode FncReInit();																		/**< ReInit */
			/* 0x02 */ public delegate ReturnCode FncFinalize();																	/**< Finalize */
			/* 0x03 */ public delegate ReturnCode FncAddObserver(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);			/**< AddObserver */
			/* 0x04 */ public delegate ReturnCode FncRemoveObserver(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);		/**< RemoveObserver */
			/* 0x05 */ public delegate ReturnCode FncGetProperty(ref Property property);  											/**< GetProperty */
			/* 0x06 */ public delegate ReturnCode FncDummy06(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x07 */ public delegate ReturnCode FncDummy07(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x08 */ public delegate ReturnCode FncDummy08(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x09 */ public delegate ReturnCode FncDummy09(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x0A */ public delegate ReturnCode FncDummy0A(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x0B */ public delegate ReturnCode FncDummy0B(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x0C */ public delegate ReturnCode FncDummy0C(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x0D */ public delegate ReturnCode FncDummy0D(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x0E */ public delegate ReturnCode FncDummy0E(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x0F */ public delegate ReturnCode FncDummy0F(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */

			/* 0x10 */ public delegate ReturnCode FncDummy10(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x11 */ public delegate ReturnCode FncDummy11(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x12 */ public delegate ReturnCode FncDummy12(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x13 */ public delegate ReturnCode FncDummy13(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x14 */ public delegate ReturnCode FncDummy14(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x15 */ public delegate ReturnCode FncDummy15(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x16 */ public delegate ReturnCode FncDummy16(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x17 */ public delegate ReturnCode FncDummy17(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x18 */ public delegate ReturnCode FncDummy18(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x19 */ public delegate ReturnCode FncDummy19(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x1A */ public delegate ReturnCode FncDummy1A(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x1B */ public delegate ReturnCode FncGetTimedValueList(float** valuelist, uint** time, int** num);	/**< GetTimedValueList */
			/* 0x1C */ public delegate ReturnCode FncDummy1C(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x1D */ public delegate ReturnCode FncDummy1D(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x1E */ public delegate ReturnCode FncDummy1E(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
			/* 0x1F */ public delegate ReturnCode FncDummy1F(ref HALComponent pHalComponent, ref HAL_ARGUMENT_T pCmd);				/**< Reserved */
		}

		public HALComponent HalCreate(int vendorID, int productID, int instanceID) {
			int i;
			int idx = -1;
			HALComponent HalComponent;
			HAL_REG_T Reg;
			OpenELReg regconf = new OpenELReg();

			for (i = 0; i < hal_sz_handler_tbl; i++) {
				HalIdxHandlerTbl = (HalIdxHandlerTbl + 1) & hal_msk_handler_tbl;
				if (0 == HalHandlerTbl[HalIdxHandlerTbl].swUsed) {
					idx = HalIdxHandlerTbl;
					break;
				}
			}
			if (idx == -1) Environment.Exit(0);
			idx = -1;

			for (i = 0; i < regconf.hal_szRegTbl; i++) {
				if ((vendorID == regconf.HalRegTbl[i].vendorID) &&
				    (productID == regconf.HalRegTbl[i].productID)) {
					idx = i;
					break;
				}
			}
			if (idx == -1) Environment.Exit(0);
			Reg = regconf.HalRegTbl[idx];

			HalComponent = new HALComponent();
			HalComponent.hALId = new HALId();
			HalComponent.handle = HalIdxHandlerTbl;
			HalComponent.hALId.deviceKindId = Reg.deviceKindID;
			HalComponent.hALId.vendorId = vendorID;
			HalComponent.hALId.productId = productID;
			HalComponent.hALId.instanceId = instanceID;

			HalHandlerTbl[HalIdxHandlerTbl].swUsed = 1;
			HalHandlerTbl[HalIdxHandlerTbl].HalComponent = HalComponent;
			HalHandlerTbl[HalIdxHandlerTbl].name = Reg.name;

			return HalComponent;
		}

		public void HalDestroy(ref HALComponent halComponent) {
			HalHandlerTbl[halComponent.handle].swUsed = 0;
		}

	}

}
