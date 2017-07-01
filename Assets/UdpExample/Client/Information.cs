using System;
using UnityEngine;
using UnityEngine.UI;

namespace UdpExample.Client
{
    public class Information : MonoBehaviour
    {
        public Player Player;

        Text Text;

        bool Available;

        string ReceivedMessages;

        DateTime LastReceived;

        void Start()
        {
            Text = GetComponent<Text>();

            Player.AddConsumer(
                message =>
                {
                    ReceivedMessages = message.Text;
                    LastReceived = DateTime.Now;
                });
        }

        void Update()
        {
            if (Text == null)
            {
                return;
            }

            Text.text = string.Format(
                "Status: {0}\n" +
                "Last Received: {1}\n" +
                "Received Messages: {2}\n" +
                "",
                Available ? "Available" : "Unavailable",
                ReceivedMessages == null ? "None" : LastReceived.ToString(),
                ReceivedMessages == null ? "None" : ReceivedMessages);
        }

        public void OnAvailable()
        {
            Available = true;
            Debug.Log("Service is available.");
        }

        public void OnUnavailable()
        {
            Available = false;
            Debug.LogError("Service is unavailable.");
        }
    }
}
