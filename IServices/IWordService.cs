using System;
using System.Collections.Generic;
using System.Text;

namespace redis_autocomplete
{
    public interface IWordService
    {
        IEnumerable<string> Get();
    }
}
