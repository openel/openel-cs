using System;
using System.Timers;
using System.Threading;
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

class TimeObs01 : TimerObserver {

	public void notify_timer(object sender, ElapsedEventArgs e) {
		DateTime dt = DateTime.Now;
		Console.WriteLine("{0} notify_timer : {1}", this.GetType().Name, dt.Millisecond);
		DiarkisTest.count++;
	}

}

class DiarkisTest {
	private static ReturnCode retCode;
	public float[] valuelist0 = new float[3];
	public float[] valuelist1 = new float[3];
	public float[] valuelist2 = new float[3];
	//uint time = 0;
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

	public DiarkisTest() {}

	public void DiarkisMain(CancellationToken token) {
		Console.WriteLine("Here1");
		DiarkisTest test = new DiarkisTest();
		Property property = new Property();
		HALId SCD30 = new HALId();
		SCD30.deviceKindId = 0x0000000A;
		SCD30.vendorId = 0x00000007;
		SCD30.productId = 0x00000001;
		SCD30.instanceId = 0x00000007; // Change to your Instance ID.

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

		Obs01 obs01 = new Obs01();

		EventTimer etimer = new EventTimer();
		TimeObs01 tobs01 = new TimeObs01();

		retCode = etimer.AddObserver(tobs01);
		Console.WriteLine("EventTimer AddObserver : {0}", retCode);
		retCode = etimer.SetEventPeriod(500);
		Console.WriteLine("EventTimer SetEventPeriod : {0}", retCode);
		retCode = etimer.StartTimer();
		Console.WriteLine("EventTimer StartTimer : {0}", retCode);

		retCode = etimer.StopTimer();
		Console.WriteLine("EventTimer StopTimer : {0}", retCode);

		Sensor sensor00 = new Sensor(SCD30);
		retCode = sensor00.AddObserver(obs01);
		Console.WriteLine("SCD30 AddObserver : {0}", retCode);
		retCode = sensor00.Init();
		Console.WriteLine("SCD30 Init : {0}", retCode);

		Sensor sensor01 = new Sensor(Diarkis00);
		retCode = sensor01.AddObserver(obs01);
		Console.WriteLine("Diarkis01 AddObserver : {0}", retCode);
		retCode = sensor01.Init();
		Console.WriteLine("Diarkis00 Init : {0}", retCode);

		Sensor sensor02 = new Sensor(Diarkis01);
		retCode = sensor02.AddObserver(obs01);
		Console.WriteLine("Diarkis01 AddObserver : {0}", retCode);
		retCode = sensor02.Init();
		Console.WriteLine("Diarkis01 Init : {0}", retCode);

		while (true) {

			retCode = sensor00.GetValueList(this.valuelist0, ref this.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("SCD30 GetValueList : {0}, valuelist0[0] = {1}, valuelist0[1] = {2}, valuelist0[2] = {3}, num = {4}",
								   retCode, this.valuelist0[0], this.valuelist0[1], this.valuelist0[2], this.num);
			} else {
				Console.WriteLine("SCD30 GetValueList : {0}", retCode);
			}

			retCode = sensor01.GetValueList(this.valuelist1, ref this.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis00 GetValueList : {0}, valuelist1[0] = {1}, valuelist1[1] = {2}, valuelist1[2] = {3}, num = {4}",
								   retCode, this.valuelist1[0], this.valuelist1[1], this.valuelist1[2], this.num);
			} else {
				Console.WriteLine("Diarkis00 GetValueList : {0}", retCode);
			}

			retCode = sensor02.GetValueList(this.valuelist2, ref this.num);
			if (retCode == ReturnCode.HAL_OK) {
				Console.WriteLine("Diarkis01 GetValueList : {0}, valuelist2[0] = {1}, valuelist2[1] = {2}, valuelist2[2] = {3}, num = {4}",
								   retCode, this.valuelist2[0], this.valuelist2[1], this.valuelist2[2], this.num);
			} else {
				Console.WriteLine("Diarkis01 GetValueList : {0}", retCode);
			}

			Task.Delay(1000);
			if (token.IsCancellationRequested) {
				Console.WriteLine("Cancelled");
				break;
			}
		}
	}
}

