using PRMasterServer.Data;
using Reality.Net.Extensions;
using Reality.Net.GameSpy.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PRMasterServer.Servers
{
	internal class ServerListRetrieve
	{
		private const string Category = "ServerRetrieve";

		public Action<string, string> Log = (x, y) => { };
		public Action<string, string> LogError = (x, y) => { };

        public Thread Thread;
        

		private static Socket _socket;
		private readonly ServerListReport _report;

		private readonly ManualResetEvent _reset = new ManualResetEvent(false);
		private AsyncCallback _socketSendCallback;
		private AsyncCallback _socketDataReceivedCallback;


        static public int tmp1=0;
        static public Socket tmpS1;
        static public byte[] resp11 = new byte[] {0x7F,0x00,0x00,0x01,0x19,0x64,0x0E,0x00,0x68,0x6F,0x73,0x74,0x6E,0x61,0x6D,0x65,0x00,0x00,0x67,0x61,0x6D,0x65,0x6D,0x6F,0x64,0x65,0x00,0x00,0x68,0x6F,0x73,0x74,0x6E,0x61,0x6D,0x65,0x00,0x00,0x67,0x61,0x6D,0x65,0x76,0x65,0x72,0x00,0x00,0x70,
0x61,0x73,0x73,0x77,0x64,0x00,0x00,0x68,0x6F,0x73,0x74,0x70,0x6F,0x72,0x74,0x00,0x00,0x73,0x74,0x61,0x67,0x69,0x6E,0x67,0x00,0x00,0x6E,0x65,0x77,0x67,0x61,0x6D,0x65,0x00,0x00,0x6D,0x61,0x70,0x6E,0x61,0x6D,0x65,0x00,0x00,0x67,0x61,
0x6D,0x65,0x74,0x79,0x70,0x65,0x00,0x00,0x6D,0x79,0x6E,0x75,0x6D,0x70,0x6C,0x61,0x79,0x65,0x72,0x73,0x00,0x00,0x6D,0x61,0x78,0x6E,0x75,0x6D,0x70,0x6C,0x61,0x79,0x65,0x72,0x73,0x00,0x00,0x6E,0x75,0x6D,0x6D,0x69,0x73,0x73,
0x69,0x6E,0x67,0x00,0x00,0x70,0x69,0x74,0x62,0x6F,0x73,0x73,0x00,0x00,0x73,0xC0,0xA8,0x89,0xCC,0x19,0x64,0xC0,0xA8,0x89,0xCC,0x19,0x64,0xFF,0x61,0x64,0x6D,0x69,0x6E,0x6F,0x73,0x74,0x72,0x61,0x74,0x65,0x72,0x27,0x73,
0x20,0x47,0x61,0x6D,0x65,0x00,0xFF,0x6F,0x70,0x65,0x6E,0x73,0x74,0x61,0x67,0x69,0x6E,0x67,0x00,0xFF,0x61,0x64,0x6D,0x69,0x6E,0x6F,0x73,0x74,0x72,0x61,0x74,0x65,0x72,0x27,0x73,0x20,0x47,0x61,0x6D,0x65,0x00,0xFF,0x30,0x00,0xFF,
0x30,0x00,0xFF,0x32,0x30,0x35,0x36,0x00,0xFF,0x31,0x00,0xFF,0x31,0x00,0xFF,0x4D,0x65,0x64,0x69,0x75,0x6D,0x5F,0x61,0x6E,0x64,0x5F,0x53,0x6D,0x61,0x6C,0x6C,0x00,0xFF,0x00,0xFF,0x31,0x00,0xFF,0x37,0x00,0xFF,0x36,0x00,0xFF,0x30,0x00,0x00,0xFF,0xFF,0xFF,0xFF};
            
            
            /*{0x7F,0x00,0x00,0x01,0x19,0x64,0x0E,0x00,0x68,0x6F,0x73,0x74,0x6E,0x61,0x6D,0x65,0x00,0x00,0x67,0x61,0x6D,0x65,0x6D,0x6F,
0x64,0x65,0x00,0x00,0x68,0x6F,0x73,0x74,0x6E,0x61,0x6D,0x65,0x00,0x00,0x67,0x61,0x6D,0x65,0x76,0x65,0x72,0x00,0x00,0x70,
0x61,0x73,0x73,0x77,0x64,0x00,0x00,0x68,0x6F,0x73,0x74,0x70,0x6F,0x72,0x74,0x00,0x00,0x73,0x74,0x61,0x67,0x69,
0x6E,0x67,0x00,0x00,0x6E,0x65,0x77,0x67,0x61,0x6D,0x65,0x00,0x00,0x6D,0x61,0x70,0x6E,0x61,0x6D,0x65,0x00,0x00,0x67,0x61,
0x6D,0x65,0x74,0x79,0x70,0x65,0x00,0x00,0x6D,0x79,0x6E,0x75,0x6D,0x70,0x6C,0x61,0x79,0x65,0x72,0x73,0x00,0x00,0x6D,
0x61,0x78,0x6E,0x75,0x6D,0x70,0x6C,0x61,0x79,0x65,0x72,0x73,0x00,0x00,0x6E,0x75,0x6D,0x6D,0x69,0x73,0x73,
0x69,0x6E,0x67,0x00,0x00,0x70,0x69,0x74,0x62,0x6F,0x73,0x73,0x00,0x00,0x51,0xC0,0xA8,0x89,0x82,0x19,0x64,0xFF,0x61,
0x64,0x6D,0x69,0x6E,0x6F,0x73,0x74,0x72,0x61,0x74,0x65,0x72,0x27,0x73,0x20,0x47,0x61,0x6D,0x65,0x00,0xFF,0x6F,
0x70,0x65,0x6E,0x73,0x74,0x61,0x67,0x69,0x6E,0x67,0x00,0xFF,0x61,0x64,0x6D,0x69,0x6E,0x6F,0x73,0x74,0x72,
0x61,0x74,0x65,0x72,0x27,0x73,0x20,0x47,0x61,0x6D,0x65,0x00,0xFF,0x30,0x00,0xFF,0x30,0x00,0xFF,0x32,0x30,0x35,0x36,0x00,0xFF,
0x31,0x00,0xFF,0x31,0x00,0xFF,0x4D,0x65,0x64,0x69,0x75,0x6D,0x5F,0x61,0x6E,0x64,0x5F,0x53,0x6D,0x61,0x6C,0x6C,0x00,
0xFF,0x00,0xFF,0x31,0x00,0xFF,0x37,0x00,0xFF,0x36,0x00,0xFF,0x30,0x00,0x00,0xFF,0xFF,0xFF,0xFF
    };*/
        static public byte[] resp12 = new byte[] {
            0x51,0x7F,0x00,0x00,0x01,0x19,0x64,0xFF,0x61,0x64,
0x6D,0x69,0x6E,0x27,0x73,0x20,0x47,0x61,0x6D,0x65,0x00,0xFF,0x6F,0x70,0x65,0x6E,0x73,0x74,0x61,0x67,0x69,0x6E,
0x67,0x00,0xFF,0x61,0x64,0x6D,0x69,0x6E,0x27,0x73,0x20,0x47,0x61,0x6D,0x65,0x00,0xFF,0x30,0x00,0xFF,0x30,0x00,0xFF,0x32,
0x30,0x35,0x36,0x00,0xFF,0x31,0x00,0xFF,0x31,0x00,0xFF,0x42,0x61,0x6C,0x61,0x6E,0x63,0x65,0x64,0x00,0xFF,0x00,0xFF,0x31,0x00,
0xFF,0x33,0x00,0xFF,0x32,0x00,0xFF,0x30,0x00,0x00,0xFF,0xFF,0xFF,0xFF
        };

        public static void SendToAllConnected(string validateX)
        {
            //byte[] unencryptedServerList = "POTATA";// PackServerList(state, servers, fields);
            //byte[] encryptedServerList = GSEncoding.Encode(key, DataFunctions.StringToBytes(validate), unencryptedServerList, unencryptedServerList.LongLength);
            //_socket
            //SendToClient(state, encryptedServerList);
            //;
            byte[] key = DataFunctions.StringToBytes(Program.getGKey(Program.gameName));
            Program.LogBlue(Encoding.ASCII.GetString(key));
            Program.LogBlue(tmpS1.Connected.ToString());
            byte[] resp13 = GSEncoding.Encode(key, DataFunctions.StringToBytes(validateX), resp11, resp11.LongLength);
            Program.LogBlue(Encoding.ASCII.GetString(resp13));
            tmpS1.Send(resp13);
           
            // ParseRequest.
            
        }
       

		public ServerListRetrieve(IPAddress listen, ushort port, ServerListReport report, Action<string, string> log, Action<string, string> logError)
		{
			Log = log;
			LogError = logError;
            //logError("public","serverlistretrieve");
			_report = report;
			//*
			/*_report.Servers.TryAdd("test", 
				new GameServer() {
					Valid = true,
                          
             newgame=false,
            staging=false,
            mynumplayers=61,
            maxnumplayers=139,
            nummissing=78,
            pitboss=false,
					IPAddress = "192.168.1.2",
					QueryPort = 29300,
					country = "AU",
					hostname = "Teamer 35vs49",
					gamename = "civ4",
					gamever = "3.19",
					mapname = "Awesome Map",
					gametype = "gpm_cq",
					gamevariant = "pr",
					numplayers = 61,
					maxplayers = 183,
					gamemode = "openplaying",
					password = false,
					timelimit = 14400,
					roundtime = 1,
					hostport = 16567}
                / *    ,
					bf2_dedicated = true,
					bf2_ranked = true,
					bf2_anticheat = false,
					bf2_os = "win32",
					bf2_autorec = true,
					bf2_d_idx = "http://",
					bf2_d_dl = "http://",
					bf2_voip = true,
					bf2_autobalanced = false,
					bf2_friendlyfire = true,
					bf2_tkmode = "No Punish",
					bf2_startdelay = 240.0,
					bf2_spawntime = 300.0,
					bf2_sponsortext = "Welcome to an awesome server!",
					bf2_sponsorlogo_url = "http://",
					bf2_communitylogo_url = "http://",
					bf2_scorelimit = 100,
					bf2_ticketratio = 100.0,
					bf2_teamratio = 100.0,
					bf2_team1 = "US",
					bf2_team2 = "MEC",
					bf2_bots = false,
					bf2_pure = false,
					bf2_mapsize = 64,
					bf2_globalunlocks = true,
					bf2_fps = 35.0,
					bf2_plasma = true,
					bf2_reservedslots = 16,
					bf2_coopbotratio = 0,
					bf2_coopbotcount = 0,
					bf2_coopbotdiff = 0,
					bf2_novehicles = false* /
				
			);*/

			IQueryable<GameServer> servers = _report.Servers.Select(x => x.Value).AsQueryable();
            //Console.WriteLine(servers.Where("gamever = '3.19' and gamevariant = 'pr' and hostname like '%[[]PR v1.2.0.0% %' and hostname like '%2%'").Count());
		//	*/

			Thread = new Thread(StartServer) {
				Name = "Server Retrieving Socket Thread"
			};
			Thread.Start(new AddressInfo() {
				Address = listen,
				Port = port
			});
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					if (_socket != null) {
						_socket.Close();
						_socket.Dispose();
						_socket = null;
					}
				}
			} catch (Exception) {
			}
		}

		~ServerListRetrieve()
		{
			Dispose(false);
		}

		private void StartServer(object parameter)
		{
			AddressInfo info = (AddressInfo)parameter;
            
			Program.LogCyan(Category +":Starting Server List Retrieval");

			try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
					SendTimeout = 5000,
					ReceiveTimeout = 5000,
					SendBufferSize = 65535,
					ReceiveBufferSize = 65535
				};
				_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, true);
				_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

				_socket.Bind(new IPEndPoint(info.Address, info.Port));
				_socket.Listen(10);
			} catch (Exception e) {
				LogError(Category, String.Format("Unable to bind Server List Retrieval to {0}:{1}", info.Address, info.Port));
				LogError(Category, e.ToString());
				return;
			}

			while (true) {
				_reset.Reset();
				_socket.BeginAccept(AcceptCallback, _socket);
				_reset.WaitOne();
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			_reset.Set();

			Socket listener = (Socket)ar.AsyncState;
			Socket handler = listener.EndAccept(ar);
            tmp1=tmp1+1;
            if (tmp1==1){
                tmpS1 = handler;
                //handler.AddressFamily.
                //TmpS1. handler;
                //AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            }
			SocketState state = new SocketState() {
				Socket = handler
			};
            //Program.LogCyan("SB.AcceptCallBack");
			WaitForData(state);
            
		}

		private void WaitForData(SocketState state)
		{
			Thread.Sleep(10);
			if (state == null || state.Socket == null || !state.Socket.Connected)
				return;

            
           try {
				if (_socketDataReceivedCallback == null)
					_socketDataReceivedCallback = OnDataReceived;

				state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, _socketDataReceivedCallback, state);
			} catch (ObjectDisposedException) {
				state.Socket = null;
			} catch (SocketException e) {
				if (e.SocketErrorCode == SocketError.NotConnected)
					return;

				LogError(Category, "Error receiving data");
				LogError(Category, String.Format("{0} {1}", e.SocketErrorCode, e));
				return;
			}
		}

		private void OnDataReceived(IAsyncResult async)
		{
			SocketState state = (SocketState)async.AsyncState;
            
			if (state == null || state.Socket == null || !state.Socket.Connected)
				return;

			try {
				// receive data from the socket
				int received = state.Socket.EndReceive(async);
				if (received == 0) {
					// when EndReceive returns 0, it means the socket on the other end has been shut down.
					return;
				}

				// take what we received, and append it to the received data buffer
				state.ReceivedData.Append(Encoding.UTF8.GetString(state.Buffer, 0, received));
				string receivedData = state.ReceivedData.ToString();
                Program.LogCyan("SB.OnDataReceived:"+ receivedData);
				// does what we received end with \x00\x00\x00\x00\x??
				if (receivedData.Substring(receivedData.Length - 5, 4) == "\x00\x00\x00\x00") {
					state.ReceivedData.Clear();

					// lets split up the message based on the delimiter
					string[] messages = receivedData.Split(new string[] { "\x00\x00\x00\x00" }, StringSplitOptions.RemoveEmptyEntries);
                    //LogError(messages[1], messages[2]);
                    //LogError(messages[3], messages[4]);
					for (int i = 0; i < messages.Length; i++) {
						if (messages[i].StartsWith(Program.gameName/*"civ4"/*battlefield2*/)) {
							if (ParseRequest(state, messages[i]))
								return;
						}
					}
				}
			} catch (ObjectDisposedException) {
				if (state != null)
					state.Dispose();
				state = null;
				return;
			} catch (SocketException e) {
				switch (e.SocketErrorCode) {
					case SocketError.ConnectionReset:
						if (state != null)
							state.Dispose();
						state = null;
						return;
					case SocketError.Disconnecting:
						if (state != null)
							state.Dispose();
						state = null;
						return;
					default:
						LogError(Category, "Error receiving data");
						LogError(Category, String.Format("{0} {1}", e.SocketErrorCode, e));
						if (state != null)
							state.Dispose();
						state = null;
						return;
				}
			} catch (Exception e) {
				LogError(Category, "Error receiving data");
				LogError(Category, e.ToString());
			}

			// and we wait for more data...
			WaitForData(state);
		}

		public void SendToClient(SocketState state, byte[] data)
		{
            Program.LogCyan("SB.SendToClient");
			if (state == null)
				return;

			if (state.Socket == null || !state.Socket.Connected) {
				state.Dispose();
				state = null;
				return;
			}

			if (_socketSendCallback == null)
				_socketSendCallback = OnSent;

			try {
                
				state.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, _socketSendCallback, state);
			} catch (SocketException e) {
				LogError(Category, "Error sending data");
				LogError(Category, String.Format("{0} {1}", e.SocketErrorCode, e));
			}
		}
    
		private void OnSent(IAsyncResult async)
		{
			SocketState state = (SocketState)async.AsyncState;
            //LogError("pfpfpf","is sent");
			if (state == null || state.Socket == null)
				return;

			try {
				int sent = state.Socket.EndSend(async);
                Log(Category, String.Format("Sent {0} byte response to: {1}:{2}", sent, ((IPEndPoint)state.Socket.RemoteEndPoint).Address, ((IPEndPoint)state.Socket.RemoteEndPoint).Port));
			} catch (SocketException e) {
				switch (e.SocketErrorCode) {
					case SocketError.ConnectionReset:
					case SocketError.Disconnecting:
						return;
					default:
						LogError(Category, "Error sending data");
						LogError(Category, String.Format("{0} {1}", e.SocketErrorCode, e));
						return;
				}
			} finally {
				state.Dispose();
				state = null;
			}
		}

		public bool ParseRequest(SocketState state, string message)
		{
            /////////////////Ahtung
            Program.LogCyan("SB.Parserequest." + message);
			string[] data = message.Split(new char[] { '\x00' }, StringSplitOptions.RemoveEmptyEntries);
			if (data.Length != 4 ||
                !data[0].Equals(Program.gameName/*"civ4"/*battlefield2*/, StringComparison.InvariantCultureIgnoreCase) ||
				(
                
                    !data[1].Equals(Program.gameName/*"civ4"/*battlefield2*/, StringComparison.InvariantCultureIgnoreCase) &&
					!data[1].Equals("gslive", StringComparison.InvariantCultureIgnoreCase)
				)
			) {
                Program.LogCyan("SB.CHECKPOINT X.");
				return false;
			}

			string gamename = data[1].ToLowerInvariant();
			string validate = data[2].Substring(0, 8);
            Program.LogCyan("SB.validate cp:"+validate);
            string filter = "numplayers>-1";//Temporarily so that it shows all games //"gamemode=\"openstaging\"";//FixFilter(data[2].Substring(8));
            Program.LogCyan("filtar:" + '(' + filter + ')');
			string[] fields = data[3].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

			Log(Category, String.Format("Received client request: {0}:{1}", ((IPEndPoint)state.Socket.RemoteEndPoint).Address, ((IPEndPoint)state.Socket.RemoteEndPoint).Port));

            IQueryable<GameServer> servers = _report.Servers.ToList().Select(x => x.Value).Where(x => x.Valid).AsQueryable();
			if (!String.IsNullOrWhiteSpace(filter)) {
				try {
					//Console.WriteLine(filter);
					servers = servers.Where(filter);
                    //Console.ForegroundColor = ConsoleColor.DarkCyan;
                    //Console.WriteLine(servers.Where(filter));
                    //Console.ForegroundColor = ConsoleColor.Gray;
				} catch (Exception e) {
					LogError(Category, "Error parsing filter");
					LogError(Category, filter);
					LogError(Category, e.ToString());
				}
			}

			// http://aluigi.altervista.org/papers/gslist.cfg
			byte[] key;
            if (gamename == Program.gameName)
            {/*"civ4")/*battlefield2*/
                Program.LogCyan("GKey:" + Program.getGKey(gamename));
                key = DataFunctions.StringToBytes(Program.getGKey(gamename));
            }//("y3D9Hw"/*"Cs2iIq"/*hW6m9a*/);
            else if (gamename == "arma2oapc")
                key = DataFunctions.StringToBytes("sGKWik");
            else
                key = DataFunctions.StringToBytes("Xn221z");
			
			byte[] unencryptedServerList = PackServerList(state, servers, fields);
			byte[] encryptedServerList = GSEncoding.Encode(key, DataFunctions.StringToBytes(validate), unencryptedServerList, unencryptedServerList.LongLength);

            Program.LogCyan(System.Text.Encoding.ASCII.GetString(encryptedServerList));
            SendToClient(state, encryptedServerList);
            //SendToClient(state, unencryptedServerList);
            
			return true;
		}

		private static byte[] PackServerList(SocketState state, IEnumerable<GameServer> servers, string[] fields)
		{
			IPEndPoint remoteEndPoint = ((IPEndPoint)state.Socket.RemoteEndPoint);
            Program.LogCyan("SB.PackSL.");
			byte[] ipBytes = remoteEndPoint.Address.GetAddressBytes();
			byte[] value2 = BitConverter.GetBytes((ushort)6500);
			byte fieldsCount = (byte)(fields.Length);

			List<byte> data = new List<byte>();
			data.AddRange(ipBytes);
			data.AddRange(BitConverter.IsLittleEndian ? value2.Reverse() : value2);
			data.Add(fieldsCount);
			data.Add(0);
            //Console.WriteLine("Packlist, checkpoint 1, fields count:");
            //Console.WriteLine(fieldsCount);
            foreach (var field in fields) {
				data.AddRange(DataFunctions.StringToBytes(field));
				data.AddRange(new byte[] { 0, 0 });
			}
            //Console.WriteLine("Packlist, checkpoint 2");
			foreach (var server in servers) {
				// commented this stuff out since it caused some issues on testing, might come back to it later and see what's happening...
				// NAT traversal stuff...
				// 126 (\x7E)	= public ip / public port / private ip / private port / icmp ip
				// 115 (\x73)	= public ip / public port / private ip / private port
				// 85 (\x55)	= public ip / public port
				// 81 (\x51)	= public ip / public port
                //WAS COMMENTED OUT
                //30.09
                /**/Console.WriteLine(server.IPAddress);
                Console.WriteLine(server.QueryPort);
                Console.WriteLine(server.localip0);
                Console.WriteLine(server.localip1);
                Console.WriteLine(server.localport);
                Console.WriteLine(server.natneg);/*
                if (!String.IsNullOrWhiteSpace(server.localip0) && !String.IsNullOrWhiteSpace(server.localip1) && server.localport > 0) {
                    data.Add(126);
                    data.AddRange(IPAddress.Parse(server.IPAddress).GetAddressBytes());
                    data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.QueryPort).Reverse() : BitConverter.GetBytes((ushort)server.QueryPort));
                    data.AddRange(IPAddress.Parse(server.localip0).GetAddressBytes());
                    data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.localport).Reverse() : BitConverter.GetBytes((ushort)server.localport));
                    data.AddRange(IPAddress.Parse(server.localip1).GetAddressBytes());
                }
                else if (!String.IsNullOrWhiteSpace(server.localip0) && server.localport > 0)
                {
                    data.Add(115);
                    data.AddRange(IPAddress.Parse(server.IPAddress).GetAddressBytes());
                    data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.QueryPort).Reverse() : BitConverter.GetBytes((ushort)server.QueryPort));
                    data.AddRange(IPAddress.Parse(server.localip0).GetAddressBytes());
                    data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.localport).Reverse() : BitConverter.GetBytes((ushort)server.localport));
                }
                else
                {*/
                    data.Add(81); //WAS 81//it could be 85 as well, unsure of the difference, but 81 seems more common...
                    data.AddRange(IPAddress.Parse(server.IPAddress).GetAddressBytes());
                    data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.QueryPort).Reverse() : BitConverter.GetBytes((ushort)server.QueryPort));
               //**/ }
                
                data.Add(255);
                //Console.WriteLine("Packlist, checkpoint 3");
				for (int i = 0; i < fields.Length; i++) {
                    //Console.WriteLine(fields[i]);
                    
					data.AddRange(DataFunctions.StringToBytes(GetField(server, fields[i])));

					if (i < fields.Length - 1)
						data.AddRange(new byte[] { 0, 255 });
				}

				data.Add(0);
			}

			data.AddRange(new byte[] { 0, 255, 255, 255, 255 });
            //Console.WriteLine("Packlist, checkpoint 4");
            Program.LogCyan("SB.ListCount:"+data.Count.ToString());
            byte[] bong = data.ToArray();
           /* foreach (byte color in data)
            {*/
                for (int i = 0; i < bong.Count(); i++)
                {
                    Console.Write(bong[i] + ",");
                }
                //Program.LogCyan(color.ToString());
                //Program.LogCyan(color);
            //}
            Program.LogCyan( System.Text.Encoding.ASCII.GetString(bong));
            


			return data.ToArray();
		}

		private static string GetField(GameServer server, string fieldName)
        {
            
                object value = server.GetType().GetProperty(fieldName).GetValue(server, null);
           
                if (value == null)
				return String.Empty;
			else if (value is Boolean)
				return (bool)value ? "1" : "0";
			else
				return value.ToString();
		}

		private string FixFilter(string filter)
		{
			// escape [
			filter = filter.Replace("[", "[[]");

			// fix an issue in the BF2 main menu where filter expressions aren't joined properly
			// i.e. "numplayers > 0gametype like '%gpm_cq%'"
			// becomes "numplayers > 0 && gametype like '%gpm_cq%'"
			try {
				filter = FixFilterOperators(filter);
			} catch (Exception e) {
				LogError(Category, e.ToString());
			}

			// fix quotes inside quotes
			// i.e. hostname like 'flyin' high'
			// becomes hostname like 'flyin_ high'
			try {
				filter = FixFilterQuotes(filter);
			} catch (Exception e) {
				LogError(Category, e.ToString());
			}

			// fix consecutive whitespace
			filter = Regex.Replace(filter, @"\s+", " ").Trim();

			return filter;
		}

		private static string FixFilterOperators(string filter)
		{
			PropertyInfo[] properties = typeof(GameServer).GetProperties();
			List<string> filterableProperties = new List<string>();

			// get all the properties that aren't "[NonFilter]"
			foreach (var property in properties) {
				if (property.GetCustomAttributes(false).Any(x => x.GetType().Name == "NonFilterAttribute"))
					continue;

				filterableProperties.Add(property.Name);
			}

			// go through each property, see if they exist in the filter,
			// and check to see if what's before the property is a logical operator
			// if it is not, then we slap a && before it
			foreach (var property in filterableProperties) {
				IEnumerable<int> indexes = filter.IndexesOf(property);
				foreach (var index in indexes) {
					if (index > 0) {
						int length = 0;
						bool hasLogical = IsLogical(filter, index, out length, true) || IsOperator(filter, index, out length, true) || IsGroup(filter, index, out length, true);
						if (!hasLogical) {
							filter = filter.Insert(index, " && ");
						}
					}
				}
			}
			return filter;
		}

		private static string FixFilterQuotes(string filter)
		{
			StringBuilder newFilter = new StringBuilder(filter);

			for (int i = 0; i < filter.Length; i++) {
				int length = 0;
				bool isOperator = IsOperator(filter, i, out length);

				if (isOperator) {
					i += length;
					bool isInsideString = false;
					for (; i < filter.Length; i++) {
						if (filter[i] == '\'' || filter[i] == '"') {
							if (isInsideString) {
								// check what's after the quote to see if we terminate the string
								if (i >= filter.Length - 1) {
									// end of string
									isInsideString = false;
									break;
								}
								for (int j = i + 1; j < filter.Length; j++) {
									// continue along whitespace
									if (filter[j] == ' ') {
										continue;
									} else {
										// if it's a logical operator, then we terminate
										bool op = IsLogical(filter, j, out length);
										if (op) {
											isInsideString = false;
											j += length;
											i = j;
										}
										break;
									}
								}
								if (isInsideString) {
									// and if we're still inside the string, replace the quote with a wildcard character
									newFilter[i] = '_';
								}
								continue;
							} else {
								isInsideString = true;
							}
						}
					}
				}
			}

			return newFilter.ToString();
		}

		private static bool IsOperator(string filter, int i, out int length, bool previous = false)
		{
			bool isOperator = false;
			length = 0;

			if (i < filter.Length - 1) {
				string op = filter.Substring(i - (i >= 2 ? (previous ? 2 : 0) : 0), 1);
				if (op == "=" || op == "<" || op == ">") {
					isOperator = true;
					length = 1;
				}
			}

			if (!isOperator) {
				if (i < filter.Length - 2) {
					string op = filter.Substring(i - (i >= 3 ? (previous ? 3 : 0) : 0), 2);
					if (op == "==" || op == "!=" || op == "<>" || op == "<=" || op == ">=") {
						isOperator = true;
						length = 2;
					}
				}
			}

			if (!isOperator) {
				if (i < filter.Length - 4) {
					string op = filter.Substring(i - (i >= 5 ? (previous ? 5 : 0) : 0), 4);
					if (op.Equals("like", StringComparison.InvariantCultureIgnoreCase)) {
						isOperator = true;
						length = 4;
					}
				}
			}

			if (!isOperator) {
				if (i < filter.Length - 8) {
					string op = filter.Substring(i - (i >= 9 ? (previous ? 9 : 0) : 0), 8);
					if (op.Equals("not like", StringComparison.InvariantCultureIgnoreCase)) {
						isOperator = true;
						length = 8;
					}
				}
			}

			return isOperator;
		}

		private static bool IsLogical(string filter, int i, out int length, bool previous = false)
		{
			bool isLogical = false;
			length = 0;

			if (i < filter.Length - 2) {
				string op = filter.Substring(i - (i >= 3 ? (previous ? 3 : 0) : 0), 2);
				if (op == "&&" || op == "||" || op.Equals("or", StringComparison.InvariantCultureIgnoreCase)) {
					isLogical = true;
					length = 2;
				}
			}

			if (!isLogical) {
				if (i < filter.Length - 3) {
					string op = filter.Substring(i - (i >= 4 ? (previous ? 4 : 0) : 0), 3);
					if (op.Equals("and", StringComparison.InvariantCultureIgnoreCase)) {
						isLogical = true;
						length = 3;
					}
				}
			}

			return isLogical;
		}

		private static bool IsGroup(string filter, int i, out int length, bool previous = false)
		{
			bool isGroup = false;
			length = 0;

			if (i < filter.Length - 1) {
				string op = filter.Substring(i - (i >= 2 ? (previous ? 2 : 0) : 0), 1);
				if (op == "(" || op == ")") {
					isGroup = true;
					length = 1;
				}
				if (!isGroup && previous) {
					op = filter.Substring(i - (i >= 1 ? (previous ? 1 : 0) : 0), 1);
					if (op == "(" || op == ")") {
						isGroup = true;
						length = 1;
					}
				}
			}

			return isGroup;
		}

		public class SocketState : IDisposable
		{
			public Socket Socket = null;
			public byte[] Buffer = new byte[8192];
			public StringBuilder ReceivedData = new StringBuilder(8192);

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				try {
					if (disposing) {
						if (Socket != null) {
							try {
								Socket.Shutdown(SocketShutdown.Both);
							} catch (Exception) {
							}
							Socket.Close();
							Socket.Dispose();
							Socket = null; //29.09
						}
					}

					GC.Collect();
				} catch (Exception) {
				}
			}

			~SocketState()
			{
				Dispose(false);
			}
		}
	}
}
