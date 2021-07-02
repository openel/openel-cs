//#define DEBUG

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenEL;

using Diarkis;
using Diarkis.Modules;

unsafe class SensorDiarkis : Sensor {

	private static uint co2;
	private static uint temp;
	private static uint humid;
	private static string host = "xxx.xxx.xxx.xxx"; // Input your Diarkis server
	private static string uid;
	private static string client = "udp";
	private static string date;
	private static string time;

	private static string[] sensorValue = new string[Enum.GetNames(typeof(Instance)).Length];
	private static bool[] rcvFlg = new bool[Enum.GetNames(typeof(Instance)).Length];
	private static string initValue = "Not Found Data";
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
	private static short valueNum = 3;
	private static int deviceKindId; 
	private static int instanceId; 
	private static int productId; 
	private static int vendorId; 

	public SensorDiarkis(HALId halId, bool exec) : base(halId, exec) {
		deviceKindId = halId.deviceKindId;
		instanceId = halId.instanceId;
		productId = halId.productId;
		vendorId = halId.vendorId;
	 }

	string strDevName = "Diarkis";

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

		for(int i = 0; i < Enum.GetNames(typeof(Instance)).Length; i++) {
			sensorValue[i] = initValue;
			rcvFlg[i] = false;
		}

		guidValue = Guid.NewGuid();
		uid = guidValue.ToString();
		IDlist.Add(uid);

		string url = string.Format("https://{0}/auth/{1}", host, uid);
		Http http = new Http();
		http.Get();
		http.Url(url);
		http.Header("ClientKey", "xxxxxxxx"); // Input your ClientKey.
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

		string url = string.Format("https://{0}/auth/{1}", host, uid);

		Http http = new Http();
		http.Get();
		http.Url(url);
		http.Header("ClientKey", "xxxxxxxx"); // Input your ClientKey.
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

		_Update();

		Task.Delay(100);

#if (DEBUG)
		Console.WriteLine("{0}", sensorValue[instanceId - 1]);
#endif
		if (rcvFlg[instanceId - 1] == false) {
			this.ErrorId = 1;
			return ReturnCode.HAL_ERROR;
		}

		string[] arr = sensorValue[instanceId - 1].Split(",");
		co2 = uint.Parse(arr[5]);
		temp = uint.Parse(arr[6]);
		humid = uint.Parse(arr[7]);

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

		rcvFlg[instanceId - 1] = false;

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

		_Update();

		Task.Delay(100);

#if (DEBUG)
		Console.WriteLine("{0}", sensorValue[instanceId - 1]);
#endif
		if (rcvFlg[instanceId - 1] == false) {
			this.ErrorId = 1;
			return ReturnCode.HAL_ERROR;
		}

		string[] arr = sensorValue[instanceId - 1].Split(",");
		date = arr[3];
		time = arr[4];
		co2 = uint.Parse(arr[5]);
		temp = uint.Parse(arr[6]);
		humid = uint.Parse(arr[7]);
		DateTime dt = DateTime.Parse(string.Format("{0} {1}", date, time), null, System.Globalization.DateTimeStyles.AssumeLocal);

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

		rcvFlg[instanceId - 1] = false;

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

	private void _OnHttpResponse(string res) {
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
			udp.OnDisconnect += _OnDisconnect;
			udp.Connect(udpAddr, udpPort);
			ufield = new Field();
			ufield.OnSync += _OnFieldSync;
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
			tcp.OnDisconnect += _OnDisconnect;
			tcp.Connect(tcpAddr, tcpPort);
			tfield = new Field();
			tfield.OnSync += _OnFieldSync;
			tfield.SetupAsTcp(tcp);
			tcpInit = true;
		}
	}

	private void _OnTcpException(int errCode, string errMsg) {
		tcp.Disconnect();
		Environment.Exit(0);
	}

	private void _OnUdpException(int errCode, string errMsg) {
		udp.Disconnect();
		Environment.Exit(0);
	}

	private byte[] _ConvertHexToBytes(string hex) {
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

	private void _FieldSync(string client, byte[] msg) {
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
					ufield.SyncInit(x, y, z, 65535, 0, msg);
					usyncInit = true;
				} else {
					ufield.Sync(x, y, z, 65535, 0, msg, false);
				}
			break;
		}
	}

	private void _OnFieldSync(byte[] msg) {
		int id;
		string str = Encoding.UTF8.GetString(msg);
		string type = str.Substring(0, 3);
		string[] arr = str.Split(",");
		switch (type) {
			case "snd":
				id = (int)Enum.Parse(typeof(Instance), arr[2], true) - 1;
				if (arr[0].Contains(initValue)) {
					sensorValue[id] = arr[0].Replace(type, "");
				} else {
					sensorValue[id] = str.Replace(type, "");
				}
				rcvFlg[id] = true;
				break;
			default:
				break;
		}
	}

	private void _OnDisconnect(bool reconnect) {
		this.ErrorId = 2;
	}

	private uint UnixTime(DateTime now) {
		string str = "1970/01/01 00:00:00";
		DateTime dt = DateTime.Parse(str);

		return (uint)((now.ToFileTimeUtc() - dt.ToFileTimeUtc())/ 10000000);
	}

}

