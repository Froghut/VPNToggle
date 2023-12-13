using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using VPNToggle.Properties;

namespace VPNToggle
{
	public partial class frmVPNToggle : Form
	{
		private Uri Url;
		private string _sid;
		private JArray _nonVpnMacs;
		private byte[] _salt = Encoding.Unicode.GetBytes("#/=W4EL-x.Ro");
		private string MacAddress => Cast(new { MAC = "", IP = "", Name = "" }, comboBox1.SelectedItem).MAC;

		public frmVPNToggle()
		{
			InitializeComponent();

			txt_RouterIP.Text = Settings.Default.RouterIP;
			txt_RouterPassword.Text = Decrypt(Settings.Default.Password);
			btn_GetStatus.Visible = false;

			var macs = NetworkInterface
				.GetAllNetworkInterfaces()
				.Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
				.Select(nic => new
				{
					MAC = ConvertMac(nic.GetPhysicalAddress().ToString()),
					IP = nic.GetIPProperties().UnicastAddresses.First(a => a.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString(),
					Name = nic.Name
				}).ToList();
			comboBox1.Items.AddRange(macs.ToArray());
			int loadIndex = macs.FindIndex(a => a.MAC == Settings.Default.Mac);
			if (loadIndex == -1)
				loadIndex = 0;
			comboBox1.SelectedIndex = loadIndex;


			LoadVPNStatusFromRouter();
		}

		private void LoadVPNStatusFromRouter()
		{
			Enabled = false;
			if (Settings.Default.RouterIP == "" || Settings.Default.Password == "")
			{
				btn_GetStatus.Visible = true;
				Enabled = true;
				return;
			}
			try
			{
				Url = new Uri($"http://{Settings.Default.RouterIP}/rpc");
				JObject jObject = new JObject();
				jObject["username"] = "root";
				JToken loginResult = InvokeMethod("challenge", true, jObject)["result"];

				string salt = loginResult["salt"].ToString();
				string password = Decrypt(Settings.Default.Password);

				Process p = new Process()
				{
					StartInfo = new ProcessStartInfo()
					{
						FileName = "openssl",
						Arguments = $"passwd -1 -salt \"{salt}\" \"{password}\"",
						UseShellExecute = false,
						RedirectStandardOutput = true,
						CreateNoWindow = true
					}
				};
				p.Start();
				string readToEnd = p.StandardOutput.ReadToEnd().Trim();
				p.WaitForExit();

				string loginHash = "root:" + readToEnd + ":" + loginResult["nonce"];
				MD5 md5 = MD5.Create();
				string loginString = Convert.ToHexString(md5.ComputeHash(Encoding.ASCII.GetBytes(loginHash))).ToLower();

				jObject["hash"] = loginString;

				_sid = InvokeMethod("login", true, jObject)["result"]["sid"].Value<string>();

				JToken? getMacPolicyResult = InvokeMethod("call", false, _sid, "vpn-policy", "get_mac_policy")["result"];
				if (getMacPolicyResult["default_policy"].Value<int>() == 0)
				{
					throw new Exception("VPN policy is set to Use VPN only for specified devices, this must be set to not use VPN!");
				}

				_nonVpnMacs = (JArray)getMacPolicyResult["mac_list"];

				bool vpnEnabled = _nonVpnMacs.All(v => v.Value<string>() != MacAddress);
				UpdateButtons(vpnEnabled);
				btn_GetStatus.Visible = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error while getting VPN Status from Router: {ex}");
				btn_GetStatus.Visible = true;
			}
			finally
			{
				Enabled = true;
			}
		}

		private string ConvertMac(string? mac)
		{
			for (int i = 2; i < mac.Length; i += 3)
			{
				mac = mac.Insert(i, ":");
			}

			return mac;
		}

		public JObject InvokeMethod(string method, bool login, params object[] parameters)
		{
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);


			webRequest.ContentType = "application/json-rpc";
			webRequest.Method = "POST";

			JObject joe = new JObject();
			joe["jsonrpc"] = "2.0";
			joe["method"] = method;

			if (parameters != null)
			{
				if (parameters.Length > 0)
				{
					if (login)
					{
						joe.Add("params", (JObject)parameters[0]);
					}
					else
					{
						JArray props = new JArray();
						foreach (var p in parameters)
						{
							props.Add(p);
						}

						joe.Add(new JProperty("params", props));
					}
				}
			}
			joe["id"] = 0;

			string s = JsonConvert.SerializeObject(joe);
			// serialize json for the request
			byte[] byteArray = Encoding.UTF8.GetBytes(s);
			webRequest.ContentLength = byteArray.Length;

			try
			{
				using (Stream dataStream = webRequest.GetRequestStream())
				{
					dataStream.Write(byteArray, 0, byteArray.Length);
				}
			}
			catch (WebException we)
			{
				//inner exception is socket
				//{"A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 23.23.246.5:8332"}
				throw;
			}
			WebResponse webResponse = null;
			try
			{
				using (webResponse = webRequest.GetResponse())
				{
					using (Stream str = webResponse.GetResponseStream())
					{
						using (StreamReader sr = new StreamReader(str))
						{
							string readToEnd = sr.ReadToEnd();
							return JsonConvert.DeserializeObject<JObject>(readToEnd);
						}
					}
				}
			}
			catch (WebException webex)
			{

				using (Stream str = webex.Response.GetResponseStream())
				{
					using (StreamReader sr = new StreamReader(str))
					{
						string readToEnd = sr.ReadToEnd();
						var tempRet = JsonConvert.DeserializeObject<JObject>(readToEnd);
						return tempRet;
					}
				}

			}
			catch (Exception)
			{

				throw;
			}
		}

		private void button_DisableVPN_Click(object sender, EventArgs e)
		{
			ChangeVPNStatus(false);
		}

		private void button_EnableVPN_Click(object sender, EventArgs e)
		{
			ChangeVPNStatus(true);
			button_DisableVPN.Enabled = true;
			button_EnableVPN.Enabled = false;
		}

		private void ChangeVPNStatus(bool enableVPN)
		{
			if (enableVPN)
			{
				int index = -1;
				for (var i = 0; i < _nonVpnMacs.Count; i++)
				{
					if (_nonVpnMacs[i].Value<string>() == MacAddress)
					{
						index = i;
						break;
					}
				}
				_nonVpnMacs.RemoveAt(index);
			}
			else
			{
				_nonVpnMacs.Add(MacAddress);
			}
			JObject jobj = new JObject();
			jobj["default_policy"] = 1;
			jobj["mac_list"] = _nonVpnMacs;
			JObject result = InvokeMethod("call", false, _sid, "vpn-policy", "set_mac_policy", jobj);
			Console.WriteLine(result);
			UpdateButtons(enableVPN);
		}

		private void UpdateButtons(bool vpnEnabled)
		{
			button_DisableVPN.Enabled = vpnEnabled;
			button_EnableVPN.Enabled = !vpnEnabled;
			button_DisableVPN.BackColor = vpnEnabled ? Color.Red : SystemColors.Control;
			button_EnableVPN.BackColor = !vpnEnabled ? Color.LimeGreen : SystemColors.Control;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			Settings.Default.Mac = MacAddress;
			Settings.Default.Save();

		}

		private static T Cast<T>(T typeHolder, Object x)
		{
			// typeHolder above is just for compiler magic
			// to infer the type to cast x to
			return (T)x;
		}

		public string Encrypt(string input)
		{
			byte[] encryptedData = ProtectedData.Protect(
				Encoding.Unicode.GetBytes(input),
				_salt,
				DataProtectionScope.CurrentUser);
			return Convert.ToBase64String(encryptedData);
		}

		public string Decrypt(string encryptedData)
		{
			try
			{
				byte[] decryptedData = ProtectedData.Unprotect(
					Convert.FromBase64String(encryptedData),
					_salt,
					DataProtectionScope.CurrentUser);
				return Encoding.Unicode.GetString(decryptedData);
			}
			catch
			{
				return "";
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			Settings.Default.RouterIP = txt_RouterIP.Text;
			Settings.Default.Save();
		}

		private void txt_RouterPassword_TextChanged(object sender, EventArgs e)
		{
			Settings.Default.Password = Encrypt(txt_RouterPassword.Text);
			Settings.Default.Save();
		}

		private void btn_GetStatus_Click(object sender, EventArgs e)
		{
			LoadVPNStatusFromRouter();
		}
	}
}
