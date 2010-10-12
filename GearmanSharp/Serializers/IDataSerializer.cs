using System.Linq;
using System.Linq.Expressions;

namespace Twingly.GearmanApi.Serializers
{
    public interface IDataSerializer<T> where T : class
    {
        byte[] Serialize(T data);
        T Deserialize(byte[] data);
    }
}