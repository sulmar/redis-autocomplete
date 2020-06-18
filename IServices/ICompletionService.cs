using System;
using System.Collections;
using System.Collections.Generic;

namespace redis_autocomplete
{
    public interface ICompletionService
    {
        bool Exists { get; }
        void Add(string word);
        void AddRange(IEnumerable<string> words, IProgress<string> progress = default);
        public IEnumerable<string> Get(string prefix);
    }
}
