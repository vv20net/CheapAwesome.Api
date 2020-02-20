using System;
using System.Collections.Generic;
using System.Text;

namespace CheapAwesome.Core.Common.Interface
{
    public interface ICacheService
    {
        T GetOrAdd<T>(string key, Func<T> producer, Func<T, bool> validator, TimeSpan expiration) where T: class;
    }
}
