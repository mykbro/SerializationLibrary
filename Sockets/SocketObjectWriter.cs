using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary.Sockets
{
    public class SocketObjectWriter<T> where T : ICustomSerializable<T>
    {
        private readonly ObjectWriter<T> _objWriter;
        private readonly Stream _outputStream;
        private readonly MemoryStream _supportMemStream;
        private readonly Socket _socket;


        public SocketObjectWriter(Socket socket, SerializationMode serializationMode, bool useCompression = false)
        {
            _socket = socket;
            _supportMemStream = new MemoryStream();

            if (useCompression)
                _outputStream = new GZipStream(_supportMemStream, CompressionMode.Compress);
            else
                _outputStream = _supportMemStream;

            if (serializationMode == SerializationMode.Binary)
                _objWriter = new BinaryObjectWriter<T>(_outputStream);
            else if (serializationMode == SerializationMode.DataContract)
                _objWriter = new DataContractObjectWriter<T>(_outputStream, false);
            else if (serializationMode == SerializationMode.Custom)
                _objWriter = new CustomObjectWriter<T>(_outputStream);
            else
                throw new ArgumentOutOfRangeException("serializationMode");
        }

        public async Task WriteObjectAsync(T obj, CancellationToken cancellationToken = default)
        {
            byte[] objAsBytes;
            byte[] dataLengthAsBytes;
            byte[] payload;
            int dataLength;
            int bytesWritten;       //used for debug purposes


            _objWriter.WriteObject(obj);                    //we write the serialization to a temp Mem stream
            objAsBytes = _supportMemStream.ToArray();       //we get the byte[]
            _supportMemStream.SetLength(0);                 //we reset the Stream length for re-using it to avoid re-instantiating the DataContractObjectWriter

            dataLength = objAsBytes.Length;                         //we get the payload length...
            dataLengthAsBytes = BitConverter.GetBytes(dataLength);  //...and convert it to a 4bytes array

            payload = new byte[4 + dataLength];    //we create an array which is the union of the 2 in orderd to call only one Send
            Array.Copy(dataLengthAsBytes, 0, payload, 0, 4);
            Array.Copy(objAsBytes, 0, payload, 4, dataLength);

            await _socket.SendAsync(payload, SocketFlags.None, cancellationToken);

            //bytesWritten = await _socket.SendAsync(new ArraySegment<byte>(dataLengthAsBytes), SocketFlags.None, cancellationToken);  //we firstly send the msg length (4 bytes)
            //bytesWritten = await _socket.SendAsync(new ArraySegment<byte>(objAsBytes), SocketFlags.None, cancellationToken);  //and then the payload
        }

        public void WriteObject(T obj)
        {
            byte[] objAsBytes;
            byte[] dataLengthAsBytes;
            byte[] payload;
            int dataLength;
            int bytesWritten;       //used for debug purposes


            _objWriter.WriteObject(obj);                    //we write the serialization to a temp Mem stream
            objAsBytes = _supportMemStream.ToArray();       //we get the byte[]
            _supportMemStream.SetLength(0);                 //we reset the Stream length for re-using it to avoid re-instantiating the DataContractObjectWriter

            dataLength = objAsBytes.Length;                         //we get the obj serialization length...
            dataLengthAsBytes = BitConverter.GetBytes(dataLength);  //...and convert it to a 4bytes array

            payload = new byte[4 + dataLength];    //we create an array which is the union of the 2 in orderd to call only one Send
            Array.Copy(dataLengthAsBytes, 0, payload, 0, 4);
            Array.Copy(objAsBytes, 0, payload, 4, dataLength);

            _socket.Send(payload);     //send the whole payload            
        }
    }
}
