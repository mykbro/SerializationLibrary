using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary.Sockets
{
    /*
     * Just a synchronized version of a SocketObjectWriter<T>
     */
    
    public class ConcurrentSocketObjectWriter<T> where T : ICustomSerializable<T>
    {
        private readonly SocketObjectWriter<T> _socketWriter;
        private readonly SemaphoreSlim _socketWriterSem;        //for the async version we cannot use lock(...){}

        public ConcurrentSocketObjectWriter(Socket socket, SerializationMode serializationMode, bool useCompression = false)
        {
            _socketWriter = new SocketObjectWriter<T>(socket, serializationMode, useCompression);
            _socketWriterSem = new SemaphoreSlim(1);
        }

        public int Hash => GetHashCode();

        public async Task WriteObjectAsync(T obj, CancellationToken cancellationToken = default)
        {
            _socketWriterSem.Wait();
            try
            {
                await _socketWriter.WriteObjectAsync(obj, cancellationToken);
            }
            finally
            {
                _socketWriterSem.Release();
            }
        }

        public void WriteObject(T obj)
        {
            _socketWriterSem.Wait();
            try
            {
                _socketWriter.WriteObject(obj);
            }
            finally
            {
                _socketWriterSem.Release();
            }
        }
    }
}
