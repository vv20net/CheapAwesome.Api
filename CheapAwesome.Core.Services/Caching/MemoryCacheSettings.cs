using System;
using System.Collections.Generic;
using System.Text;

namespace CheapAwesome.Core.Services.Caching
{
    public class MemoryCacheSettings
    {
        public bool Enabled { get; set; }
        public long? SizeLimit { get; set; }

    }
}
