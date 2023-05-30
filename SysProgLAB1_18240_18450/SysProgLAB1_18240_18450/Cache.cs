using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysProgLAB1_18240_18450
{
    internal class Cache
    {
        private static int _TIME_OUT = 200;
        private int _size;
        private Dictionary<string, (LinkedListNode<string>, string)> _dictionary;
        private LinkedList<string> _keyList;
        private ReaderWriterLockSlim _cacheLock=new ReaderWriterLockSlim();
        public Cache(int size = 10)
        {
            _size = size;
            _dictionary = new Dictionary<string, (LinkedListNode<string>, string)>(_size);
            _keyList = new LinkedList<string>();
        }
        public string CitajIzKesa(string key)
        {
            try
            {
                _cacheLock.EnterReadLock();
                var cvor = _dictionary[key];
                _keyList.Remove(cvor.Item1);
                _keyList.AddFirst(cvor.Item1);

                return cvor.Item2;
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }

        }
        public bool SadrziKljuc (string key)
        {
            try
            {
                _cacheLock.EnterReadLock();
                return _dictionary.ContainsKey(key);
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }
        public void UpisiUKes(string key,string value)
        {
            bool locked = false;
            try
            {
                locked = _cacheLock.TryEnterWriteLock(_TIME_OUT);
                if (locked)
                {
                    if(_dictionary.Count==_size)
                    {
                        _dictionary.Remove(_keyList.Last!.Value);
                        _keyList.RemoveLast();
                    }

                    _dictionary.Add(key, (_keyList.AddFirst(key), value));
                }
            }
            finally
            {
                if(locked)
                    _cacheLock.ExitWriteLock();
            }
        }
    }
}
