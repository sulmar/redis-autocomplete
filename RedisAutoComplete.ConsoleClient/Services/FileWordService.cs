using System.Collections.Generic;
using System.IO;

namespace redis_autocomplete
{

    public class FileWordService : IWordService
    {
        private readonly string filename;

        public FileWordService(string filename)
        {
            this.filename = filename;
        }

        public IEnumerable<string> Get()
        {
            return File.ReadAllLines(filename);
        }
    }
}
