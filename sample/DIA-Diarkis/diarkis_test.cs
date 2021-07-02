using System;
using System.Timers;
using System.Threading.Tasks;
using OpenEL;

class Obs01 : HALObserver {

	public void notify_error(int error_id) {
		Console.WriteLine("{0} notify_error : {1}", this.GetType().Name, error_id);
	}

	public void notify_event(int event_id) {
		Console.WriteLine("{0} notify_event : {1}", this.GetType().Name, event_id);
	}

}

class Obs02 : HALObserver {

	public void notify_error(int error_id) {
		Console.WriteLine("{0} notify_error : {1}", this.GetType().Name, error_id);
	}

	public void notify_event(int event_id) {
		Console.WriteLine("{0} notify_event : {1}", this.GetType().Name, event_id);
	}

}

class TimeObs01 : TimerObserver {

	public void notify_timer(object sender, ElapsedEventArgs e) {
		DateTime dt = DateTime.Now;
		Console.WriteLine("{0} notify_timer : {1}", this.GetType().Name, dt.Millisecond);
		DiarkisTest.count++;
	}

}

class TimeObs02 : TimerObserver {

	public void notify_timer(object sender, ElapsedEventArgs e) {
		DateTime dt = DateTime.Now;
		Console.WriteLine("{0} notify_timer : {1}", this.GetType().Name, dt.Millisecond);
		DiarkisTest.count++;
	}

}

unsafe class DiarkisTest {

	private static ReturnCode retCode;
	float[] valuelist = new float[3];
	uint time = 0;
	int num = 0;
	public static int count = 0;

        private enum Instance {
                Adachi = 0x00000001,
                Setagaya,
                Shinjuku,
                Urayasu,
                Kawasaki,
                Misato,
                Fujimino,
                Osaka,
                Shibuya
        };

	public static void Main(string[] args) {
		DiarkisTest test = new DiarkisTest();
		Property property = new Property();
		HALId SCD30 = new HALId();
		SCD30.deviceKindId = 0x0000000A;
		SCD30.vendorId = 0x00000007;
		SCD30.productId = 0x00000001;
		SCD30.instanceId = 0x00000001; // Change to your Instance ID.

		HALId Diarkis00 = new HALId();
		Diarkis00.deviceKindId = 0x0000000A;
		Diarkis00.vendorId = 0x00000008;
		Diarkis00.productId = 0x00000001;
		Diarkis00.instanceId = 0x00000001;

		HALId Diarkis01 = new HALId();
		Diarkis01.deviceKindId = 0x0000000A;
		Diarkis01.vendorId = 0x00000008;
		Diarkis01.productId = 0x00000001;
		Diarkis01.instanceId = 0x00000002;

		HALId Diarkis02 = new HALId();
		Diarkis02.deviceKindId = 0x0000000A;
		Diarkis02.vendorId = 0x00000008;
		Diarkis02.productId = 0x00000001;
		Diarkis02.instanceId = 0x00000003;

		HALId Diarkis03 = new HALId();
		Diarkis03.deviceKindId = 0x0000000A;
		Diarkis03.vendorId = 0x00000008;
		Diarkis03.productId = 0x00000001;
		Diarkis03.instanceId = 0x00000004;

		HALId Diarkis04 = new HALId();
		Diarkis04.deviceKindId = 0x0000000A;
		Diarkis04.vendorId = 0x00000008;
		Diarkis04.productId = 0x00000001;
		Diarkis04.instanceId = 0x00000005;

		HALId Diarkis05 = new HALId();
		Diarkis05.deviceKindId = 0x0000000A;
		Diarkis05.vendorId = 0x00000008;
		Diarkis05.productId = 0x00000001;
		Diarkis05.instanceId = 0x00000006;

		HALId Diarkis06 = new HALId();
		Diarkis06.deviceKindId = 0x0000000A;
		Diarkis06.vendorId = 0x00000008;
		Diarkis06.productId = 0x00000001;
		Diarkis06.instanceId = 0x00000007;

		HALId Diarkis07 = new HALId();
		Diarkis07.deviceKindId = 0x0000000A;
		Diarkis07.vendorId = 0x00000008;
		Diarkis07.productId = 0x00000001;
		Diarkis07.instanceId = 0x00000008;

		HALId Diarkis08 = new HALId();
		Diarkis08.deviceKindId = 0x0000000A;
		Diarkis08.vendorId = 0x00000008;
		Diarkis08.productId = 0x00000001;
		Diarkis08.instanceId = 0x00000009;

		Obs01 obs01 = new Obs01();
		Obs02 obs02 = new Obs02();

		EventTimer etimer = new EventTimer();
		TimeObs01 tobs01 = new TimeObs01();
		TimeObs02 tobs02 = new TimeObs02();

		retCode = etimer.AddObserver(tobs01);
		Console.WriteLine("EventTimer AddObserver : {0}", retCode);
		retCode = etimer.AddObserver(tobs02);
		Console.WriteLine("EventTimer AddObserver : {0}", retCode);
		retCode = etimer.RemoveObserver(tobs02);
		Console.WriteLine("EventTimer RemoveObserver : {0}", retCode);
		retCode = etimer.StartTimer();
		Console.WriteLine("EventTimer StartTimer : {0}", retCode);

		while (count < 10) {			
		}
		count = 0;

		retCode = etimer.StopTimer();
		Console.WriteLine("EventTimer StopTimer : {0}", retCode);
		retCode = etimer.SetEventPeriod(500);
		Console.WriteLine("EventTimer SetEventPeriod : {0}", retCode);
		retCode = etimer.StartTimer();
		Console.WriteLine("EventTimer StartTimer : {0}", retCode);

		while (count < 10) {			
		}

		retCode = etimer.StopTimer();
		Console.WriteLine("EventTimer StopTimer : {0}", retCode);

		Sensor sensor00 = new Sensor(SCD30);
		retCode = sensor00.AddObserver(obs01);
		Console.WriteLine("SCD30 AddObserver : {0}", retCode);
		retCode = sensor00.AddObserver(obs02);
		Console.WriteLine("SCD30 AddObserver : {0}", retCode);
		retCode = sensor00.RemoveObserver(obs01);
		Console.WriteLine("SCD30 RemoveObserver : {0}", retCode);
		retCode = sensor00.Init();
		Console.WriteLine("SCD30 Init : {0}", retCode);
		retCode = sensor00.ReInit();
		Console.WriteLine("SCD30 ReInit : {0}", retCode);
		Sensor sensor01 = new Sensor(Diarkis00);
		retCode = sensor01.Init();
		Console.WriteLine("Diarkis00 Init : {0}", retCode);
		retCode = sensor01.ReInit();
		Console.WriteLine("Diarkis00 ReInit : {0}", retCode);
		Sensor sensor02 = new Sensor(Diarkis01);
		retCode = sensor02.AddObserver(obs01);
		Console.WriteLine("Diarkis01 AddObserver : {0}", retCode);
		retCode = sensor02.Init();
		Console.WriteLine("Diarkis01 Init : {0}", retCode);
		Sensor sensor03 = new Sensor(Diarkis02);
		retCode = sensor03.Init();
		Console.WriteLine("Diarkis02 Init : {0}", retCode);
		Sensor sensor04 = new Sensor(Diarkis03);
		retCode = sensor04.Init();
		Console.WriteLine("Diarkis03 Init : {0}", retCode);
		Sensor sensor05 = new Sensor(Diarkis04);
		retCode = sensor05.Init();
		Console.WriteLine("Diarkis04 Init : {0}", retCode);
		Sensor sensor06 = new Sensor(Diarkis05);
		retCode = sensor06.Init();
		Console.WriteLine("Diarkis05 Init : {0}", retCode);
		Sensor sensor07 = new Sensor(Diarkis06);
		retCode = sensor07.Init();
		Console.WriteLine("Diarkis06 Init : {0}", retCode);
		Sensor sensor08 = new Sensor(Diarkis07);
		retCode = sensor08.Init();
		Console.WriteLine("Diarkis07 Init : {0}", retCode);
		Sensor sensor09 = new Sensor(Diarkis08);
		retCode = sensor09.Init();
		Console.WriteLine("Diarkis08 Init : {0}", retCode);

		while (true) {

			retCode = sensor00.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("SCD30 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("SCD30 GetTime : {0}", retCode);
			}
/*
			retCode = sensor00.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor00.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("SCD30 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("SCD30 GetValueList : {0}", retCode);
			}
			retCode = sensor00.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("SCD30 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("SCD30 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor01.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis00 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis00 GetTime : {0}", retCode);
			}
/*
			retCode = sensor01.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor01.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis00 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis00 GetValueList : {0}", retCode);
			}
			retCode = sensor01.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis00 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis00 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor02.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis01 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis01 GetTime : {0}", retCode);
			}
/*
			retCode = sensor02.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor02.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis01 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis01 GetValueList : {0}", retCode);
			}
			retCode = sensor02.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis01 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis01 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor03.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis02 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis02 GetTime : {0}", retCode);
			}
/*
			retCode = sensor03.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor03.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis02 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis02 GetValueList : {0}", retCode);
			}
			retCode = sensor03.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis02 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis02 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor04.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis03 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis03 GetTime : {0}", retCode);
			}
/*
			retCode = sensor04.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor04.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis03 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis03 GetValueList : {0}", retCode);
			}
			retCode = sensor04.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis03 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis03 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor05.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis04 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis04 GetTime : {0}", retCode);
			}
/*
			retCode = sensor05.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor05.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis04 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis04 GetValueList : {0}", retCode);
			}
			retCode = sensor05.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis04 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis04 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor06.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis05 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis05 GetTime : {0}", retCode);
			}
/*
			retCode = sensor06.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor06.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis05 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis05 GetValueList : {0}", retCode);
			}
			retCode = sensor06.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis05 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis05 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor07.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis06 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis06 GetTime : {0}", retCode);
			}
/*
			retCode = sensor07.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor07.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis06 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis06 GetValueList : {0}", retCode);
			}
			retCode = sensor07.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis06 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis06 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor08.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis07 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis07 GetTime : {0}", retCode);
			}
/*
			retCode = sensor08.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor08.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis07 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis07 GetValueList : {0}", retCode);
			}
			retCode = sensor08.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis07 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis07 GetTimedValueList : {0}", retCode);
			}

			retCode = sensor09.GetTime(ref test.time);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis08 GetTime : {0}, time = {1}", retCode, test.time);
			} else {
				Console.WriteLine("Diarkis08 GetTime : {0}", retCode);
			}
/*
			retCode = sensor09.GetProperty(property);
			Console.WriteLine("Property deviceName = {0}", property.deviceName);
			foreach(string str in property.functionList) {
				Console.WriteLine("{0}", str);
			}
*/
			retCode = sensor09.GetValueList(test.valuelist, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis08 GetValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, num = {4}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.num);
			} else {
				Console.WriteLine("Diarkis08 GetValueList : {0}", retCode);
			}
			retCode = sensor09.GetTimedValueList(test.valuelist, ref test.time, ref test.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis08 GetTimedValueList : {0}, valuelist[0] = {1}, valuelist[1] = {2}, valuelist[2] = {3}, time = {4}, num = {5}",
								   retCode, test.valuelist[0], test.valuelist[1], test.valuelist[2], test.time, test.num);
			} else {
				Console.WriteLine("Diarkis08 GetTimedValueList : {0}", retCode);
			}

			Task.Delay(1000);
		}

	}

}
