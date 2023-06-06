using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary
{
    public interface ICustomSerializable<T>
    {
        public byte[] Serialize();
        static public abstract T Deserialize(byte[] objRep);
    }
}
