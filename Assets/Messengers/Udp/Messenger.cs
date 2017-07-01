using Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

namespace Messengers.Udp
{
    public class Messenger : MonoBehaviour
    {
        public string Host = "127.0.0.1";

        public int Port = 8080;

        public float CheckIntervalSeconds = 4.0f;

        public float CheckTimeoutSeconds = 4.0f;

        /// <summary>
        /// Max data size.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <term>Max DNS packet size is 512 bytes.</term>
        /// <description><see href="https://www.ietf.org/rfc/rfc1035.txt" /></description>
        /// </item>
        /// <item>
        /// <term>IPv4 Minimum MTU is 576 bytes.</term>
        /// <description><see href="https://tools.ietf.org/html/rfc791" /></description>
        /// </item>
        /// <item>
        /// <term>Ethernet frame max size is 1518 bytes.</term>
        /// <description><see href="http://www.ieee802.org/3/" /></description>
        /// </item>
        /// </list>
        /// </remarks>
        /// </summary>
        public int MaxDataBytes = 512;

        public UnityEvent OnAvailable;

        public UnityEvent OnUnavailable;

        IPEndPoint Remote;

        UdpClient Client;

        List<Action<byte[]>> Consumers = new List<Action<byte[]>>();

        float LastReceived;

        bool _available;

        bool Available
        {
            get
            {
                return _available;
            }
            set
            {
                if (value != _available)
                {
                    if (value)
                    {
                        OnAvailable.Invoke();
                    }
                    else
                    {
                        OnUnavailable.Invoke();
                    }
                }

                _available = value;
            }
        }

        public void AddConsumer(Action<byte[]> consumer)
        {
            Consumers.Add(consumer);
        }

        public void Send(byte[] message)
        {
            if (message.Length > MaxDataBytes)
            {
                Debug.LogError("Data size is too large.");
            }
            else
            {
                try
                {
                    Client.Send(message, message.Length, Remote);
                }
                catch (SocketException)
                {
                    Debug.LogError("Failed to send.");
                }
            }
        }

        void Awake()
        {
            Remote = new IPEndPoint(IPAddress.Parse(Host), Port);

            Client = new UdpClient();

            Available = true;

            StartCoroutine(Receive());

            StartCoroutine(Check());
        }

        IEnumerator Receive()
        {
            IPEndPoint endpoint = null;

            while (true)
            {
                // Socket.Poll で到着を待機.
                yield return new Poll(Client.Client);

                if (Client.Available > 0)
                {
                    // 以下、受信処理.
                    byte[] message = Client.Receive(ref endpoint);

                    if (message.Length > MaxDataBytes)
                    {
                        Debug.LogError("Data size is too large.");
                    }
                    else if (endpoint.Equals(Remote))
                    {
                        LastReceived = Time.realtimeSinceStartup;
                        Available = true;

                        // Consume messages.
                        Consumers.ForEach(a => a(message));
                    }
                }
            }
        }

        IEnumerator Check()
        {
            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    // ネットワークが無効な場合.
                    Available = false;
                }
                else if (Time.realtimeSinceStartup - LastReceived >= CheckIntervalSeconds)
                {
                    // メッセージをしばらく受信していない場合.
                    var ok = false;

                    // 確認メッセージに対する返信の Consumer を定義.
                    Action<byte[]> replyConsumer =
                        a =>
                        {
                            if (a.Length == 1 && a[0] == 0)
                            {
                                // 正式な返信なら OK.
                                ok = true;
                            }
                        };

                    AddConsumer(replyConsumer);

                    // 確認メッセージを送信.
                    Send(new byte[] { 0 });

                    // 返信を待機.
                    yield return new WaitForSeconds(CheckTimeoutSeconds);

                    if (Time.realtimeSinceStartup - LastReceived >= CheckIntervalSeconds)
                    {
                        // 引き続きメッセージを受信していない場合、結果を反映.
                        Available = ok;
                    }

                    Consumers.Remove(replyConsumer);
                }

                yield return null;
            }
        }

        void OnDestroy()
        {
            Client.Close();
        }
    }
}
