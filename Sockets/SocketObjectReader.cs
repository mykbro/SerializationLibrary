using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO.Compression;
using SerializationLibrary.Exceptions;


namespace SerializationLibrary.Sockets
{
    public class SocketObjectReader<T> where T : ICustomSerializable<T>
    {
        private readonly ObjectReader<T> _objReader;
        private readonly MemoryStream _supportMemStream;
        private readonly Stream _inputStream;
        private readonly Socket _socket;

        public SocketObjectReader(Socket targetSocket, SerializationMode serializationMode, bool useCompression = false)
        {
            _socket = targetSocket;
            _supportMemStream = new MemoryStream();

            if (useCompression)
                _inputStream = new GZipStream(_supportMemStream, CompressionMode.Decompress);
            else
                _inputStream = _supportMemStream;

            if (serializationMode == SerializationMode.Binary)
                _objReader = new BinaryObjectReader<T>(_inputStream);
            else if (serializationMode == SerializationMode.DataContract)
                _objReader = new DataContractObjectReader<T>(_inputStream);
            else if (serializationMode == SerializationMode.Custom)
                _objReader = new CustomObjectReader<T>(_inputStream);
            else
                throw new ArgumentOutOfRangeException("serializationMode");
        }

        public async Task<T?> ReadObjectAsync(CancellationToken cancellationToken = default)
        {
            T? result;
            byte[] msgLengthAsBytes = await ReadBytesAsync(4, cancellationToken);          //we first read the payload length (an Int32 => 4 bytes)
            int msgLength = BitConverter.ToInt32(msgLengthAsBytes, 0);

            byte[] objBlob = await ReadBytesAsync(msgLength, cancellationToken);           //we than get the payload                     

            _supportMemStream.Write(objBlob, 0, objBlob.Length);        //we write the payload in the MemStream
            _supportMemStream.Position = 0;     //we rewind the streamposition otherwise we would read at the end of the stream and not at the beginning           

            try
            {
                result = _objReader.ReadObject();    //we finally read the object  
            }
            finally     //even if we throw an SerializationException we may want to keep reading so we cleanup the MemStream
            {
                _supportMemStream.SetLength(0);     //we clear the MemoryStream for future uses and clean up unused memory
            }


            return result;
        }

        private async Task<byte[]> ReadBytesAsync(int bytesToRead, CancellationToken cancellationToken = default)
        {
            if (bytesToRead == 0)
                return new byte[0];
            else if (bytesToRead < 0)
                throw new ArgumentOutOfRangeException();
            else
            {
                int bytesReadForThisReceive;
                int bytesReadSoFar = 0;
                int bytesLeftToRead = bytesToRead;
                byte[] result = new byte[bytesToRead];


                while (bytesLeftToRead > 0)
                {
                    bytesReadForThisReceive = await _socket.ReceiveAsync(new ArraySegment<byte>(result, bytesReadSoFar, bytesLeftToRead), SocketFlags.None, cancellationToken);
                    if (bytesReadForThisReceive == 0)
                    {
                        throw new ConnectionClosedByRemoteHostException();
                    }
                    else
                    {
                        bytesReadSoFar += bytesReadForThisReceive;
                        bytesLeftToRead -= bytesReadForThisReceive;
                    }
                }

                return result;
            }
        }


    }
}
