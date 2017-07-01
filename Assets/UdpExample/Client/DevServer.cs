using Messengers.Udp;
using System.Net;
using System.Threading;
using UnityEngine;

namespace UdpExample.Client
{
    public class DevServer : MonoBehaviour
    {
        public Messenger Messenger;

        #if UNITY_EDITOR
        Thread Service;

        void StartServer()
        {
            Server.Start(IPAddress.Parse(Messenger.Host), Messenger.Port);
        }

        void OnEnable()
        {
            // アクティブ時にスレッドを開始.
            Service = new Thread(new ThreadStart(StartServer));
            Service.Start();
        }

        void OnDisable()
        {
            // 非アクティブ時にスレッドを終了.
            Service.Abort();
        }
        #endif
    }
}
