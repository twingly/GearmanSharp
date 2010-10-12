using System.Linq;
using System.Linq.Expressions;

namespace Twingly.GearmanApi.Serializers
{
    public class NoopSerializer : IDataSerializer<byte[]>
    {
        public byte[] Serialize(byte[] data)
        {
            return data;
        }

        public byte[] Deserialize(byte[] data)
        {
            return data;
        }
    }
}