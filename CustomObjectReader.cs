using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary
{
    public class CustomObjectReader<T> : ObjectReader<T> where T : ICustomSerializable<T>
    {
        private readonly Stream _inputStream;

        public CustomObjectReader(Stream inputStream)
        {
            _inputStream = inputStream;
        }

        public override T? ReadObject()
        {
            byte[] buffer = new byte[_inputStream.Length];
            _inputStream.Read(buffer, 0, buffer.Length);

            return T.Deserialize(buffer);
        }
    }
}
