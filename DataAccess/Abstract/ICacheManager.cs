using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        bool Set(string key, object data);
        bool Remove(string key);
    }
}
