//#define DEBUG

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenEL;

using Diarkis;
using Diarkis.Modules;

unsafe class SensorSCD30 : Sensor {
#if (DEBUG)
    [DllImport("SCD30Dll")]
    extern static int Add(int n1, int n2);
#endif
    [DllImport("SCD30Dll")]
    extern static void sensirion_i2c_init();
    [DllImport("SCD30Dll")]
    extern static short scd30_probe();
    [DllImport("SCD30Dll")]
    extern static short scd30_set_measurement_interval(ushort interval_sec);
    [DllImport("SCD30Dll")]
    extern static void sensirion_sleep_usec(uint useconds);
    [DllImport("SCD30Dll")]
    extern static short scd30_start_periodic_measurement(ushort ambient_pressure_mbar);
    [DllImport("SCD30Dll")]
    extern static short scd30_stop_periodic_measurement();
    [DllImport("SCD30Dll")]
    extern static short scd30_get_data_ready(ref ushort data_ready);
    [DllImport("SCD30Dll")]
    extern static short scd30_read_measurement(ref float co2_ppm, ref float temperature, ref float humidity);

	private static uint co2;
	private static uint temp;
	private static uint humid;
	private static string host = "xxx.xxx.xxx.xxx"; // Input your Diarkis server
	private static string uid;
	private static string client = "udp";
	private static string name;
	private static string place;
	private static string date;
	private static string time;

	private static byte[] message;
	private static string sensorValue = "";
	private static List<string> IDlist = new List<string>();
	private static string sid;
	private static Guid guidValue;
	private static string tcpAddr;
	private static string udpAddr;
	private static int tcpPort;
	private static int udpPort;
	private static Tcp tcp;
	private static Udp udp;
	private static Field tfield;
	private static Field ufield;
	private static long x = 0;
	private static long y = 0;
	private static long z = 1;
	private static bool tcpInit = false;
	private static bool udpInit = false;
	private static bool tsyncInit = false;
	private static bool usyncInit = false;
	private static bool sensorInit = false;
	private static short cntLimit = 10;
	private static short valueNum = 3;
	private static int deviceKindId; 
	private static int instanceId; 
	private static int productId; 
	private static int vendorId; 

	public SensorSCD30(HALId halId, bool exec) : base(halId, exec) { 
		deviceKindId = halId.deviceKindId;
		instanceId = halId.instanceId;
		productId = halId.productId;
		vendorId = halId.vendorId;
	}

	string strDevName = "SCD30";

	List<string> strFncList = new List<string>() {
		"Init",
		"ReInit",
		"Finalize",
		"AddObserver",
		"RemoveObserver",
		"GetProperty",
		"GetTime",
		"GetValueList",
		"GetTimedValueList"
	};

	enum Instance {
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

	float co2_ppm = 0, temperature = 0 , relative_humidity = 0;
	ushort interval_in_seconds = 2;

	private ReturnCode scd30_init()
	{
		short cnt = 0;

#if (DEBUG)
		int a = Add(1,2);
		Console.WriteLine(a);
#endif
	    /* Initialize I2C */
    	sensirion_i2c_init();

	    /* Busy loop for initialization, because the main loop does not work without
    	 * a sensor.
    	 */
	    while (scd30_probe() != 0) {
	    	cnt++;
			sensirion_sleep_usec(1000000u);
	    	if (cnt > cntLimit) {
				Console.WriteLine("SCD30 sensor probing failed");
				return ReturnCode.HAL_ERROR;
			}
	    }
    	Console.WriteLine("SCD30 sensor probing successful");

	    scd30_set_measurement_interval(interval_in_seconds);
    	sensirion_sleep_usec(20000u);
    	scd30_start_periodic_measurement(0);

		return ReturnCode.HAL_OK;
	}

	private ReturnCode scd30_getvalue()
	{
    	short err = 0;

//    while (true) {
        ushort data_ready = 0;
        ushort timeout = 0;
#if (DEBUG)
    	Console.WriteLine("Here_1");
#endif
        /* Poll data_ready flag until data is available. Allow 20% more than
         * the measurement interval to account for clock imprecision of the
         * sensor.
         */
        for (timeout = 0; (100000 * timeout) < (interval_in_seconds * 1200000); ++timeout) {
            err = scd30_get_data_ready(ref data_ready);
            if (err != 0) {
                Console.WriteLine("Error reading data_ready flag: %i", err);
            }
            if (data_ready == 1) {
                break;
            }
            sensirion_sleep_usec(100000);
        }
#if (DEBUG)
    	Console.WriteLine("Here_2");
#endif
        if (data_ready == 0) {
            Console.WriteLine("Timeout waiting for data_ready flag");
//            continue;
        }

        /* Measure co2, temperature and relative humidity and store into
         * variables.
         */
        err = scd30_read_measurement(ref co2_ppm, ref temperature, ref relative_humidity);
#if (DEBUG)
    	Console.WriteLine("Here_3");
#endif
        if (err != 0) {
            Console.WriteLine("error reading measurement");
			return ReturnCode.HAL_ERROR;
        } else {
            Console.WriteLine("measured co2 concentration: {0} ppm, measured temperature: {1} degreeCelsius, measured humidity: {2} %%RH",
                   co2_ppm, temperature, relative_humidity);
			return ReturnCode.HAL_OK;
        }
#if (DEBUG)
    	Console.WriteLine("Here_4");
#endif
//    }

	}

	public ReturnCode fncInit() {
		ReturnCode retCode = ReturnCode.HAL_OK;
		int initCount = 0;

#if (DEBUG)
		Console.WriteLine("{0} : funcInit() start", this.GetType().Name);
		Console.WriteLine("deviceKindId = {0}, vendorId = {1}, productId = {2}, instanceId = {3}",
				   deviceKindId,
				   vendorId,
				   productId,
				   instanceId);
#endif
		retCode = scd30_init();
		if (retCode == ReturnCode.HAL_ERROR) {
			return retCode;
		}
//		scd30_getvalue();

		guidValue = Guid.NewGuid();
		uid = guidValue.ToString();
		IDlist.Add(uid);

		string url = string.Format("https://{0}/auth/{0}", host, uid);
		Http http = new Http();
		http.Get();
		http.Url(url);
		http.Header("ClientKey", "xxxxxxxx"); // Input your ClientKey
		http.OnError += _OnHttpError;
		http.OnResponse += _OnHttpResponse;
		http.Send();

		if (client == "tcp") {
			while (tcpInit == false) {
				Task.Delay(1000);
			}
		} else if (client == "udp") {
			while (udpInit == false) {
				Task.Delay(1000);
			}
		}

		if (client == "tcp") {
			while (tsyncInit == false) {
				_Update();
				Task.Delay(100);
				initCount++;
				if (initCount > 10) {
					Console.WriteLine("Diarkis Connect Failed");
					return ReturnCode.HAL_ERROR;
				}
			}
		} else if (client == "udp") {
			while (usyncInit == false) {
				_Update();
				Task.Delay(100);
				initCount++;
				if (initCount > 10) {
					Console.WriteLine("Diarkis Connect Failed");
					return ReturnCode.HAL_ERROR;
				}
			}
		}
		sensorInit = true;

		return retCode;
	}

	public ReturnCode fncReInit() {
		ReturnCode retCode = ReturnCode.HAL_OK;
		int initCount = 0;

#if (DEBUG)
		Console.WriteLine("{0} : funcReInit() start", this.GetType().Name);
		Console.WriteLine("deviceKindId = {0}, vendorId = {1}, productId = {2}, instanceId = {3}",
				   deviceKindId,
				   vendorId,
				   productId,
				   instanceId);
#endif
		sensorInit = false;
		tcp = null;
		udp = null;
		tcpInit = false;
		udpInit = false;
		tsyncInit = false;
		usyncInit = false;
		IDlist.Clear();

		guidValue = Guid.NewGuid();
		uid = guidValue.ToString();
		IDlist.Add(uid);

		string url = string.Format("https://{0}/auth/{0}", host, uid);
		Http http = new Http();
		http.Get();
		http.Url(url);
		http.Header("ClientKey", "xxxxxxxx"); // Input your ClientKey
		http.OnError += _OnHttpError;
		http.OnResponse += _OnHttpResponse;
		http.Send();

		if (client == "tcp") {
			while (tcpInit == false) {
				Task.Delay(1000);
			}
		} else if (client == "udp") {
			while (udpInit == false) {
				Task.Delay(1000);
			}
		}

		if (client == "tcp") {
			while (tsyncInit == false) {
				_Update();
				Task.Delay(100);
				initCount++;
				if (initCount > 10) {
					Console.WriteLine("Diarkis Connect Failed");
					return ReturnCode.HAL_ERROR;
				}
			}
		} else if (client == "udp") {
			while (usyncInit == false) {
				_Update();
				Task.Delay(100);
				initCount++;
				if (initCount > 10) {
					Console.WriteLine("Diarkis Connect Failed");
					return ReturnCode.HAL_ERROR;
				}
			}
		}
		sensorInit = true;

		return retCode;
	}

	public ReturnCode fncFinalize() {
		scd30_stop_periodic_measurement();

		return ReturnCode.HAL_OK;
	}

	public ReturnCode fncAddObserver() {
		return ReturnCode.HAL_OK;
	}

	public ReturnCode fncRemoveObserver() {
		return ReturnCode.HAL_OK;
	}

	public ReturnCode fncGetProperty(Property property) {
#if (DEBUG)
		Console.WriteLine("{0} : funcGetProperty() start", this.GetType().Name);
		Console.WriteLine("deviceKindId = {0}, vendorId = {1}, productId = {2}, instanceId = {3}",
				   deviceKindId,
				   vendorId,
				   productId,
				   instanceId);
#endif
		property.deviceName = String.Copy(strDevName);
		property.functionList = new List<string>(strFncList);

		return ReturnCode.HAL_OK;
	}

	public ReturnCode fncGetTime(ref uint timeValue) {
#if (DEBUG)
		Console.WriteLine("{0} : funcGetTime() start", this.GetType().Name);
		Console.WriteLine("deviceKindId = {0}, vendorId = {1}, productId = {2}, instanceId = {3}",
				   deviceKindId,
				   vendorId,
				   productId,
				   instanceId);
#endif
		DateTime dt = DateTime.Now;
		timeValue = UnixTime(dt);

		return ReturnCode.HAL_OK;
	}

	public ReturnCode fncGetValLst(float[] valueList, ref int num) {
		ReturnCode retCode = ReturnCode.HAL_OK;

#if (DEBUG)
		Console.WriteLine("{0} : funcGetValLst() start", this.GetType().Name);
		Console.WriteLine("deviceKindId = {0}, vendorId = {1}, productId = {2}, instanceId = {3}",
				   deviceKindId,
				   vendorId,
				   productId,
				   instanceId);
#endif
		if (sensorInit == false) {
			Console.WriteLine("Init not complete");
			return ReturnCode.HAL_ERROR;
		}

		retCode = scd30_getvalue();
		if (retCode == ReturnCode.HAL_ERROR) {
			return retCode;
		}

		DateTime dt = DateTime.Now;
		name = "Upwind";
		place = Enum.GetName(typeof(Instance), instanceId);
		date = dt.ToString("yyyy/MM/dd");
		time = dt.ToString("HH:mm:ss");
		co2 = (uint) co2_ppm;
		temp = (uint) temperature;
		humid = (uint) relative_humidity;

		message = Encoding.UTF8.GetBytes(string.Format("snd{0},{1},{2},{3},{4},{5},{6},{7}", uid, name, place, date, time, co2.ToString(), temp.ToString(), humid.ToString()));
		_FieldSync(client, message);

		string str = Encoding.UTF8.GetString(message);
		sensorValue = str.Replace("snd", "");
		_Update();

#if (DEBUG)
		Console.WriteLine("{0}", sensorValue);
#endif

		for (int i = 0; i < (int) valueNum; i++) {
			switch (i) {
				case 0:
					valueList[i] = co2;
					break;
				case 1:
					valueList[i] = temp;
					break;
				case 2:
					valueList[i] = humid;
					break;
				default:
					break;
			}
		}

		num = (int) valueNum;

		return retCode;
	}

	public ReturnCode fncGetTimedValLst(float[] valueList, ref uint timeValue, ref int num) {
		ReturnCode retCode = ReturnCode.HAL_OK;

#if (DEBUG)
		Console.WriteLine("{0} : funcGetTimedValLst() start", this.GetType().Name);
		Console.WriteLine("deviceKindId = {0}, vendorId = {1}, productId = {2}, instanceId = {3}",
				   deviceKindId,
				   vendorId,
				   productId,
				   instanceId);
#endif
		if (sensorInit == false) {
			Console.WriteLine("Init not complete");
			return ReturnCode.HAL_ERROR;
		}

		retCode = scd30_getvalue();
		if (retCode == ReturnCode.HAL_ERROR) {
			return retCode;
		}

		DateTime dt = DateTime.Now;
		name = "Upwind";
		place = Enum.GetName(typeof(Instance), instanceId);
		date = dt.ToString("yyyy/MM/dd");
		time = dt.ToString("HH:mm:ss");
		co2 = (uint) co2_ppm;
		temp = (uint) temperature;
		humid = (uint) relative_humidity;

		message = Encoding.UTF8.GetBytes(string.Format("snd{0},{1},{2},{3},{4},{5},{6},{7}", uid, name, place, date, time, co2.ToString(), temp.ToString(), humid.ToString()));
		_FieldSync(client, message);

		string str = Encoding.UTF8.GetString(message);
		sensorValue = str.Replace("snd", "");
		_Update();

#if (DEBUG)
		Console.WriteLine("{0}", sensorValue);
#endif

		for (int i = 0; i < (int) valueNum; i++) {
			switch (i) {
				case 0:
					valueList[i] = co2;
					break;
				case 1:
					valueList[i] = temp;
					break;
				case 2:
					valueList[i] = humid;
					break;
				default:
					break;
			}
		}

		timeValue = UnixTime(dt);
		num = (int) valueNum;

		return retCode;
	}

	public ReturnCode fncNop(ref HALComponent pHalComponent, ref Common.HAL_ARGUMENT_T pCmd) {
		return ReturnCode.HAL_ERROR;
	}

	public ReturnCode fncDeviceVensorSpec(ref HALComponent pHalComponent, ref Common.HAL_ARGUMENT_T pCmdDev) {
		return ReturnCode.HAL_ERROR;
	}

	private static void _Update() {
		if (client == "tcp") {
			tcp.Update();
		} else if (client == "udp") {
			udp.Update();
		}
	}

	private static void _OnHttpError(string err) {
		Console.WriteLine("HTTP Error: {0}", err);
		Environment.Exit(1);
	}

	private static void _OnHttpResponse(string res) {
		string tcpAddress = res.Substring(res.IndexOf("TCP") + 6, (res.IndexOf("\"UDP") - 1) - (res.IndexOf("TCP") + 7));
		if (tcpAddress.Length > 0) {
	   		tcpAddr = tcpAddress.Substring(0, tcpAddress.IndexOf(":"));
			tcpPort = Int32.Parse(tcpAddress.Substring(tcpAddress.IndexOf(":") + 1, 4));
		}
		string udpAddress = res.Substring(res.IndexOf("UDP") + 6, ((res.IndexOf("\"sid") - 1) - (res.IndexOf("UDP") + 7)));
		if (udpAddress.Length > 0) {
			udpAddr = udpAddress.Substring(0, udpAddress.IndexOf(":"));
			udpPort = Int32.Parse(udpAddress.Substring(udpAddress.IndexOf(":") + 1, 4));
		}
		sid = res.Substring(res.IndexOf("sid") + 6, res.IndexOf("encryptionKey") - res.IndexOf("sid") - (6 + 3));
		string key = res.Substring(res.IndexOf("encryptionKey") + 16, res.IndexOf("encryptionIV") - res.IndexOf("encryptionKey") - (16 + 3));
		string iv = res.Substring(res.IndexOf("encryptionIV") + 15, res.IndexOf("encryptionMacKey") - res.IndexOf("encryptionIV") - (15 + 3));
		string mac = res.Substring(res.IndexOf("encryptionMacKey") + 19, res.IndexOf("\"}") - res.IndexOf("encryptionMacKey") - (19));

		if (udp == null && udpPort != 0) {
			int sendInterval = 100;
			int echoInterval = 5000;
			udp = new Udp(sendInterval, echoInterval);
			udp.SetClientKey("xxxxxxxx"); // Input your ClientKey
			udp.SetEncryptionKeys(_ConvertHexToBytes(sid), _ConvertHexToBytes(key), _ConvertHexToBytes(iv), _ConvertHexToBytes(mac));
			udp.OnException += _OnUdpException;
			udp.OnOffline += () => {
				udp.Disconnect();
			};
			udp.OnConnect += (bool reconnected) => {
				_FieldSync("udp", Encoding.UTF8.GetBytes(string.Format("OpenEL Init")));
			};
			udp.Connect(udpAddr, udpPort);
			ufield = new Field();
			ufield.SetupAsUdp(udp);
			udpInit = true;
		}

		if (tcp == null && tcpPort != 0) {
	   		int heartbeatInterval = 5000;
			tcp = new Tcp(heartbeatInterval);
			tcp.SetClientKey("xxxxxxxx"); // Input your ClientKey
			tcp.SetEncryptionKeys(_ConvertHexToBytes(sid), _ConvertHexToBytes(key), _ConvertHexToBytes(iv), _ConvertHexToBytes(mac));
			tcp.OnException += _OnTcpException;
			tcp.OnOffline += () => {
				tcp.Disconnect();
			};
			tcp.OnConnect += (bool reconnected) => {
				_FieldSync("tcp", Encoding.UTF8.GetBytes(string.Format("OpenEL Init")));
			};
			tcp.Connect(tcpAddr, tcpPort);
			tfield = new Field();
			tfield.SetupAsTcp(tcp);
			tcpInit = true;
		}
	}

	private static void _OnTcpException(int errCode, string errMsg) {
		tcp.Disconnect();
		Environment.Exit(0);
	}

	private static void _OnUdpException(int errCode, string errMsg) {
		udp.Disconnect();
		Environment.Exit(0);
	}

	private static byte[] _ConvertHexToBytes(string hex) {
		if (hex.Length == 0) {
			return null;
		}
		int index = 0;
		int count = 0;
		int len = hex.Length;
		byte[] res = new byte[len/2];
		while (count < len) {
			res[index] = Convert.ToByte(hex.Substring(count, 2), 16);
			count += 2;
			index++;
		}
		return res;
	}

	private static void _FieldSync(string client, byte[] msg) {
		switch (client) {
			case "tcp":
				if (tfield == null) {
					return;
				}
				if (!tsyncInit) {
					tfield.SyncInit(x, y, z, 65535, 0, msg);
					tsyncInit = true;
				} else {
					tfield.Sync(x, y, z, 65535, 0, msg, true);
				}
			break;
			case "udp":
				if (ufield == null) {
					return;
				}
				if (!usyncInit) {
					ufield.SyncInit(x, y, z, 50, 0, msg);
					usyncInit = true;
				} else {
					ufield.Sync(x, y, z, 50, 0, msg, false);
				}
			break;
		}
	}

	private static uint UnixTime(DateTime now) {
		string str = "1970/01/01 00:00:00";
		DateTime dt = DateTime.Parse(str);

		return (uint)((now.ToFileTimeUtc() - dt.ToFileTimeUtc())/ 10000000);
	}

}
