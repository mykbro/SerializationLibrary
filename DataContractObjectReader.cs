using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SerializationLibrary
{
    public class DataContractObjectReader<T> : ObjectReader<T>
    {
        private readonly Stream _inputStream;
        private readonly DataContractSerializer _serializer;
        private readonly XmlReaderSettings _readerSettings;

        public DataContractObjectReader(Stream s)
        {
            _inputStream = s;
            _serializer = new DataContractSerializer(typeof(T));
            _readerSettings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment };
        }

        public override T? ReadObject()
        {
            T? toReturn;

            XmlReader tempXmlReader = XmlReader.Create(_inputStream, _readerSettings);
            toReturn = (T?)_serializer.ReadObject(tempXmlReader);
            tempXmlReader.Close();

            return toReturn;
        }
    }
}
