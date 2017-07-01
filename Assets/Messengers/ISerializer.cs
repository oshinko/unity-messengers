using System.IO;

namespace Messengers
{
    public interface ISerializer<T>
    {
        void Serialize(Stream stream, T message);

        T Deserialize(Stream stream);
    }
}
