using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace Sockets
{
    public class Select : CustomYieldInstruction
    {
        public List<Socket> Ready
        {
            get;
            private set;
        }

        List<Socket> Sockets;

        float Timeout;

        public Select(List<Socket> sockets)
            : this(sockets, -1)
        {
        }

        public Select(List<Socket> sockets, float timeout)
        {
            Sockets = sockets;

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
                Ready = new List<Socket>(Sockets);

                Socket.Select(Ready, null, null, 0);

                if (Ready.Count > 0 || Timeout > -1 && Timeout <= Time.realtimeSinceStartup)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
