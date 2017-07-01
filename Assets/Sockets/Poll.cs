using System.Net.Sockets;
using UnityEngine;

namespace Sockets
{
    public class Poll : CustomYieldInstruction
    {
        public bool Success
        {
            get;
            private set;
        }

        Socket Socket;

        SelectMode Mode;

        float Timeout;

        public Poll(Socket socket)
            : this(socket, -1)
        {
        }

        public Poll(Socket socket, float timeout)
            : this(socket, SelectMode.SelectRead, timeout)
        {
        }

        public Poll(Socket socket, SelectMode mode)
            : this(socket, mode, -1)
        {
        }

        public Poll(Socket socket, SelectMode mode, float timeout)
        {
            Socket = socket;

            Mode = mode;

            if (timeout > -1)
            {
                Timeout = Time.realtimeSinceStartup + timeout;
            }
            else
            {
                Timeout = -1;
            }
        }

        public override bool keepWaiting
        {
            get
            {
                Success = Socket.Poll(0, Mode);

                if (Success || Timeout > -1 && Timeout <= Time.realtimeSinceStartup)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
