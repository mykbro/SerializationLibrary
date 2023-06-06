using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary
{
    public abstract class ObjectWriter<T>
    {
        public abstract void WriteObject(T objToWrite);
    }
}
