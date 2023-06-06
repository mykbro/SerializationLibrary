using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary
{
    public class BinaryObjectWriter<T> : ObjectWriter<T>
    {
        private readonly BinaryFormatter _formatter;
        private readonly Stream _outputStream;

        public BinaryObjectWriter(Stream outputStream)
        {
            _formatter = new BinaryFormatter();
            _outputStream = outputStream;
        }

        public override void WriteObject(T obj)
        {
            _formatter.Serialize(_outputStream, obj);
        }
    }
}
