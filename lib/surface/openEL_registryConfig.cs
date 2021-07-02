using System;
using OpenEL;

namespace OpenEL {

	public class OpenELReg {

		public readonly Common.HAL_REG_T[] HalRegTbl =  {
			new Common.HAL_REG_T(0x000A, 0x00000007, 0x00000001, "SensorSCD30"),
			new Common.HAL_REG_T(0x000A, 0x00000008, 0x00000001, "SensorDiarkis"),
		};

		public readonly int hal_szRegTbl = 2;

	}

}

