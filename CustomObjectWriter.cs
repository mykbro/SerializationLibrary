using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary
{
    public class CustomObjectWriter<T> : ObjectWriter<T> where T : ICustomSerializable<T>
    {
        private readonly Stream _outputStream;

        public CustomObjectWriter(Stream outputStream)
        {
            _outputStream = outputStream;
        }

        public override void WriteObject(T objToWrite)
        {
            _outputStream.Write(objToWrite.Serialize());
        }
    }
}
