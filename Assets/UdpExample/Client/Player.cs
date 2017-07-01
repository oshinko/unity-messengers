using Messengers.Udp;
using System;
using System.Collections.Generic;
using System.IO;
using UdpExample.Messages;
using UnityEngine;

namespace UdpExample.Client
{
    public class Player : MonoBehaviour
    {
        public Messenger Messenger;

        List<Action<FreeMessage>> Consumers =
            new List<Action<FreeMessage>>();

        FreeMessageSerializer FreeMessageSerializer =
            new FreeMessageSerializer();

        public void AddConsumer(Action<FreeMessage> consumer)
        {
            Consumers.Add(consumer);
        }

        public void Send(string message)
        {
            var freeMessage = new FreeMessage();
            freeMessage.Text = message;

            var stream = new MemoryStream();
            FreeMessageSerializer.Serialize(stream, freeMessage);

            Messenger.Send(stream.ToArray());
        }

        void Start()
        {
            Messenger.AddConsumer(
                raw =>
                {
                    FreeMessage message = FreeMessageSerializer.
                        Deserialize(new MemoryStream(raw));

                    if (message != null)
                    {
                        Consumers.ForEach(a => a(message));
                    }
                });
        }
    }
}
