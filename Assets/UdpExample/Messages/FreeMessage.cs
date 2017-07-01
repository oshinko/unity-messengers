namespace UdpExample.Messages
{
    public class FreeMessage : IMessage
    {
        public Theme Theme
        {
            get
            {
                return Theme.Free;
            }
        }

        public string Text;
    }
}
