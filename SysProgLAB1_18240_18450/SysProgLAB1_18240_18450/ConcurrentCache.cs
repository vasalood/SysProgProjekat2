using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysProgLAB1_18240_18450
{
    internal class ConcurrentCache : ICache
    {
        private int _size;
        private ConcurrentDictionary<string, (LinkedListNode<string>, string)> _dictionary;
        private LinkedList<string> _keyList;
        private ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        public ConcurrentCache(int size = 10)
        {
            _size = size;
            _dictionary = new ConcurrentDictionary<string, (LinkedListNode<string>, string)>(
                Environment.ProcessorCount * 2, _size);
            _keyList = new LinkedList<string>();
        }
        public string CitajIzKesa(string key)
        {
            var cvor = _dictionary[key];
            try
            {
                _cacheLock.EnterWriteLock();
                _keyList.Remove(cvor.Item1);
                _keyList.AddFirst(cvor.Item1);

            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
            return cvor.Item2;
        }
        public bool SadrziKljuc(string key)
        {
            return _dictionary.ContainsKey(key);
        }
        public void UpisiUKes(string key, string value)
        {

            bool dictionaryFull = _dictionary.Count == _size;
            try
            {
                if (dictionaryFull)
                    _dictionary.Remove(_keyList.Last!.Value, out _);

                _dictionary[key] = (_keyList.AddFirst(key), value);

                if (dictionaryFull)
                {
                    _cacheLock.EnterWriteLock();
                    _keyList.RemoveLast();
                }
            }
            finally
            {
                if (_cacheLock.IsWriteLockHeld)
                    _cacheLock.ExitWriteLock();
            }

        }
    }
}
