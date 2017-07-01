using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UdpExample.Messages;
using UnityEngine;

namespace UdpExample
{
    public class Server
    {
        const int DefaultPort = 8080;

        // サイレントモードでのみ呼び出される想定の起動処理.
        public static void Start()
        {
            int port = DefaultPort;

            string[] args = Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-executeMethodArgs" &&
                    i + 1 < args.Length &&
                    int.TryParse(args[++i], out port) &&
                    port < 0)
                {
                    // 独自に定義した引数のポート番号が不正だった場合.
                    Debug.LogError("Invalid port number");
                    port = DefaultPort;
                    break;
                }
            }

            Start(IPAddress.Any, port);
        }

        // エディタでの再生時のみ呼び出される想定の起動処理.
        public static void Start(IPAddress addr, int port)
        {
            Debug.LogFormat("[Server] Service is available at {0}:{1}", addr, port);

            var udp = new UdpClient(new IPEndPoint(addr, port));

            IPEndPoint from = null;

            var freeMessageSerializer = new FreeMessageSerializer();

            while (true)
            {
                byte[] message = udp.Receive(ref from);

                if (message.Length > 0)
                {
                    var theme = (Theme)message[0];

                    if (theme == Theme.Check)
                    {
                        // 確認メッセージの返信.
                        Debug.Log("[Server] Received: ping");
                        udp.Send(new byte[] { (byte)Theme.Check }, 1, from);
                    }
                    else if (theme == Theme.Free)
                    {
                        // 自由メッセージの返信.
                        var stream = new MemoryStream(message);
                        FreeMessage deserialized = freeMessageSerializer.Deserialize(stream);
                        Debug.Log("[Server] Received: " + deserialized.Text);
                        Debug.Log("[Server] Length: " + message.Length);

                        deserialized.Text = "Thanks!";

                        stream = new MemoryStream();
                        freeMessageSerializer.Serialize(stream, deserialized);

                        byte[] buffer = stream.ToArray();
                        udp.Send(buffer, buffer.Length, from);
                    }
                }
                else
                {
                    Debug.LogError("[Server] Received message size is zero.");
                }
            }
        }
    }
}
