using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializationLibrary
{
    public abstract class ObjectReader<T>
    {
        public abstract T? ReadObject();
    }
}
