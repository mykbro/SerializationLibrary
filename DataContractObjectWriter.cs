using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SerializationLibrary
{
    public class DataContractObjectWriter<T> : ObjectWriter<T>
    {
        private readonly Stream _outputStream;
        private readonly DataContractSerializer _serializer;
        private readonly XmlWriterSettings _writerSettings;

        public DataContractObjectWriter(Stream s, bool indentationEnabled)
        {
            _outputStream = s;
            _serializer = new DataContractSerializer(typeof(T));
            _writerSettings = new XmlWriterSettings() { Indent = indentationEnabled, ConformanceLevel = ConformanceLevel.Fragment };
        }

        public override void WriteObject(T obj)
        {
            using (XmlWriter tempXmlWriter = XmlWriter.Create(_outputStream, _writerSettings))
            {
                _serializer.WriteObject(tempXmlWriter, obj);
                tempXmlWriter.Close();
            }
        }
    }
}
