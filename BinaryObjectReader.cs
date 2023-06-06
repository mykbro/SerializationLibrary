using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary
{
    public class BinaryObjectReader<T> : ObjectReader<T>
    {
        private readonly BinaryFormatter _formatter;
        private readonly Stream _inputStream;

        public BinaryObjectReader(Stream inputStream)
        {
            _formatter = new BinaryFormatter();
            _inputStream = inputStream;
        }

        public override T? ReadObject()
        {
            return (T?)_formatter.Deserialize(_inputStream);
        }
    }
}
