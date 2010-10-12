using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Twingly.GearmanApi.Serializers
{
    public class UTF8StringSerializer : IDataSerializer<string>
    {
        public byte[] Serialize(string data)
        {
            if (data == null)
                return null;

            return Encoding.UTF8.GetBytes(data);
        }

        public string Deserialize(byte[] data)
        {
            if (data == null)
                return null;

            return Encoding.UTF8.GetString(data);
        }
    }
}