using Messengers;
using System.IO;
using System.Text;

namespace UdpExample.Messages
{
    public class FreeMessageSerializer : ISerializer<FreeMessage>
    {
        Encoding Encoding = Encoding.UTF8;

        public void Serialize(Stream stream, FreeMessage message)
        {
            stream.WriteByte((byte)message.Theme);

            byte[] buffer = Encoding.GetBytes(message.Text);
            stream.Write(buffer, 0, buffer.Length);
        }

        public FreeMessage Deserialize(Stream stream)
        {
            if (stream.CanRead)
            {
                var theme = (Theme)stream.ReadByte();

                if (theme == Theme.Free)
                {
                    var result = new FreeMessage();

                    using (var reader = new StreamReader(stream, Encoding))
                    {
                        result.Text = reader.ReadToEnd();
                    }

                    return result;
                }
            }

            return null;
        }
    }
}
