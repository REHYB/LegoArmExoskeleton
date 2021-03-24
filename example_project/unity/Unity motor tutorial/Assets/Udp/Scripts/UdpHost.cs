using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace Udp
{
    public class UdpHost : MonoBehaviour
    {
        public static event Action<string> OnReceiveMsg;
        public static event Action<string> OnClientError;

        [SerializeField, Tooltip("Set to true if you want the debug logs to show in the console")] 
        protected bool _consoleLogsEnabled = false;
        [Header("Host settings")]
        [SerializeField] protected Int32 _hostPort = 5013;
        [SerializeField] protected string _hostIp = "127.0.0.1";
        [Header("Client settings")]
        [SerializeField] protected Int32 _clientPort = 5011;
        [SerializeField] protected string _clientIp = "127.0.0.1";
        [SerializeField, Tooltip("Set true if host should auto start and connect to the client")]
        protected bool _autoConnect = false;
        [Header("Stream")]
        [SerializeField] protected string _message;
        protected Thread _socketThread = null;
        protected bool _connected;
        protected EndPoint _client;
        protected Socket _socket;

        public virtual void Start()
        {
           if (_autoConnect) Connect();
        }

        /// <summary>
        /// Opens a connection to the set client
        /// </summary>
        public virtual void Connect()
        {
            //Only try to connect if the host and client are valid
            if (IsClientValid() && IsHostValid())
            {
                _socketThread = new Thread(ExecuteHost);
                _socketThread.IsBackground = true;
                _socketThread.Start();
            }
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public virtual void Close()
        {
            _connected = false; //Set to false, the thread will do the rest
        }

        #region Settings
        public virtual void SetClient(string clientIp, Int32 clientPort)
        {
            _clientIp = clientIp;
            _clientPort = clientPort;
        }

        public virtual void SetHost(string hostIp, Int32 hostPort)
        {
            _hostIp = hostIp;
            _hostPort = hostPort;
        }

        #endregion

        #region Comms
        /// <summary>
        /// Send a message to the client
        /// </summary>
        /// <param name="msg"></param>
        public virtual void SendMsg(string msg)
        {
            if (_connected)
            {
                byte[] data = Encoding.ASCII.GetBytes(msg);
                _socket.SendTo(data, data.Length, SocketFlags.None, _client);
            }
            else
            {
                Log("Not connected, can't send a message");
            }
        }

        /// <summary>
        /// Called when a message is received
        /// </summary>
        /// <param name="message">string msg</param>
        public virtual void MessageReceived(string message)
        {
            _message = message;

            OnReceiveMsg?.Invoke(message);
        }

        /// <summary>
        /// Starts the host and runs a loop waiting for messages.
        /// </summary>
        protected virtual void ExecuteHost()
        {
            try
            {
                int recv;
                byte[] data = new byte[1024];
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(_hostIp), _hostPort);

                _socket = new Socket(AddressFamily.InterNetwork,
                                SocketType.Dgram, ProtocolType.Udp);

                _socket.Bind(ipep);
                Log("Waiting for a client...");

                IPEndPoint sender = new IPEndPoint(IPAddress.Parse(_clientIp), _clientPort);
                _client = (EndPoint)(sender);

                //string welcome = "Welcome to my test host";
                data = Encoding.UTF8.GetBytes($"\"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture)}\"");
                _socket.SendTo(data, data.Length, SocketFlags.None, _client);

                _connected = _socket.Connected;
                if (_connected) Log("Connected to client");

                while (_connected)
                {
                    data = new byte[1024];
                    recv = _socket.ReceiveFrom(data, ref _client);

                    MessageReceived(Encoding.ASCII.GetString(data, 0, recv));
                }
            }
            catch (SocketException e)
            {
                //If the client is unreachable, notify user
                if (e.ErrorCode == 10051)
                {
                    string msg = "Client is unreachable, check the ip or port " + e.ErrorCode;
                    OnClientError?.Invoke(msg);
                    Log(msg);
                }
                else if (e.ErrorCode == 10054)
                {
                    OnClientError?.Invoke(e.Message);
                    Log(e.Message);
                }
                else if (e.ErrorCode == 10048) //Address already in use
                {
                    string msg = "The host address and port is already in use!";
                    Log(msg);
                }
                else
                {
                    Log(e.Message + " errorcode:" + e.ErrorCode);
                }
            }
            finally
            {
                //Cleanup
                Log("Closing...");
                if (_socket.Connected) _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _connected = _socket.Connected;
            }
            
        }
        #endregion

        #region Utils
        protected bool IsClientValid()
        {
            if (!IsIpValid(_clientIp))
            {
                Log("Client ip is not a valid ip!");
                return false;
            }
            return true;
        }

        protected bool IsHostValid()
        {
            if (!IsIpValid(_hostIp))
            {
                Log("Host ip is not a valid ip!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Used to verify that the given IP string is valid
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        protected bool IsIpValid(string ip)
        {
            string ipPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            Regex regex = new Regex(ipPattern);

            return !string.IsNullOrEmpty(ip) && regex.IsMatch(ip, 0);
        }

        /// <summary>
        /// Used for logging messages in the Unity console
        /// </summary>
        /// <param name="msg">the string message</param>
        protected void Log(string msg)
        {
            if (_consoleLogsEnabled)
                Debug.Log($"UDP HOST: {msg}");
        }
        #endregion

        public virtual void OnApplicationQuit()
        {
            Close();
        }

        
    }
}
