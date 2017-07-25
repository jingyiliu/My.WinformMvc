//using System;
//using System.Collections.Generic;
//using My.System;

//namespace My.Ioc.Collections
//{
//    public static class DictionaryHelper
//    {
//        /// <summary>
//        /// This method is used by the dictionary implementations to adapt
//        /// a given size to a prime number (or at least to some number that's
//        /// not easily divided).
//        /// </summary>
//        public static int AdaptSize(int value)
//        {
//            if (value <= 31)
//                return 31;

//            if ((value % 2) == 0)
//                value--;

//            checked
//            {
//                while (true)
//                {
//                    value += 2;

//                    if (value % 3 == 0)
//                        continue;

//                    if (value % 5 == 0)
//                        continue;

//                    if (value % 7 == 0)
//                        continue;

//                    if (value % 11 == 0)
//                        continue;

//                    if (value % 13 == 0)
//                        continue;

//                    if (value % 17 == 0)
//                        continue;

//                    if (value % 19 == 0)
//                        continue;

//                    if (value % 23 == 0)
//                        continue;

//                    if (value % 29 == 0)
//                        continue;

//                    if (value % 31 == 0)
//                        continue;

//                    return value;
//                }
//            }
//        }
//    }
//    public sealed class ThreadSafeGetOrCreateValueDictionary<TKey, TValue>
//    {
//        sealed class _Node
//        {
//            internal _Node(int hashCode, _Node nextNode, TKey key, TValue value)
//            {
//                _hashCode = hashCode;
//                _nextNode = nextNode;
//                _key = key;
//                _value = value;
//            }

//            internal readonly int _hashCode;
//            internal _Node _nextNode;
//            internal readonly TKey _key;
//            internal readonly TValue _value;
//        }

//        readonly object _lock = new object();
//        readonly Func<TKey, TValue> _creator;
//        readonly IEqualityComparer<TKey> _comparer;
//        _Node[] _baseNodes;
//        int _count;

//        public ThreadSafeGetOrCreateValueDictionary(Func<TKey, TValue> creator) :
//            this(creator, 31, null)
//        {
//        }
//        public ThreadSafeGetOrCreateValueDictionary(Func<TKey, TValue> creator, int capacity, IEqualityComparer<TKey> comparer)
//        {
//            if (creator == null)
//                throw new ArgumentNullException("creator");

//            capacity = DictionaryHelper.AdaptSize(capacity);

//            if (comparer == null)
//                comparer = EqualityComparer<TKey>.Default;

//            _creator = creator;
//            _comparer = comparer;
//            _baseNodes = new _Node[capacity];
//        }

//        public void Clear()
//        {
//            if (_count == 0)
//                return;

//            lock (_lock)
//            {
//                if (_count == 0)
//                    return;

//                for (int i = 0; i < _baseNodes.Length; i++)
//                    _baseNodes[i] = null;

//                _count = 0;
//            }
//        }

//        public TValue GetOrCreateValue(TKey key)
//        {
//            if (key == null)
//                throw new ArgumentNullException("key");

//            int hashCode = _comparer.GetHashCode(key) & int.MaxValue;

//            var baseNodes = _baseNodes;
//            int bucketIndex = hashCode % baseNodes.Length;
//            var node = baseNodes[bucketIndex];
//            while (node != null)
//            {
//                if (hashCode == node._hashCode)
//                    if (_comparer.Equals(key, node._key))
//                        return node._value;

//                node = node._nextNode;
//            }

//            lock (_lock)
//            {
//                bucketIndex = hashCode % _baseNodes.Length;
//                node = _baseNodes[bucketIndex];
//                while (node != null)
//                {
//                    if (hashCode == node._hashCode)
//                        if (_comparer.Equals(key, node._key))
//                            return node._value;

//                    node = node._nextNode;
//                }

//                if (_count >= _baseNodes.Length)
//                {
//                    _Resize();
//                    bucketIndex = hashCode % _baseNodes.Length;
//                }

//                TValue value = _creator(key);
//                var newNode = new _Node(hashCode, _baseNodes[bucketIndex], key, value);
//                _baseNodes[bucketIndex] = newNode;
//                _count++;
//                return value;
//            }
//        }

//        public void Set(TKey key, TValue value)
//        {
//            if (key == null)
//                throw new ArgumentNullException("key");

//            int hashCode = _comparer.GetHashCode(key) & int.MaxValue;
//            var newNode = new _Node(hashCode, null, key, value);

//            lock (_lock)
//            {
//                int bucketIndex = hashCode % _baseNodes.Length;

//                _Node node = _baseNodes[bucketIndex];
//                _Node previousNode = null;
//                while (node != null)
//                {
//                    if (hashCode == node._hashCode)
//                    {
//                        if (_comparer.Equals(key, node._key))
//                        {
//                            newNode._nextNode = node._nextNode;

//                            if (previousNode != null)
//                                previousNode._nextNode = newNode;
//                            else
//                                _baseNodes[bucketIndex] = newNode;

//                            return;
//                        }
//                    }

//                    previousNode = node;
//                    node = node._nextNode;
//                }

//                if (_count >= _baseNodes.Length)
//                {
//                    _Resize();
//                    bucketIndex = hashCode % _baseNodes.Length;
//                }

//                newNode._nextNode = _baseNodes[bucketIndex];
//                _baseNodes[bucketIndex] = newNode;
//                _count++;
//            }
//        }

//        public bool Remove(TKey key)
//        {
//            if (key == null)
//                throw new ArgumentNullException("key");

//            int hashCode = _comparer.GetHashCode(key) & int.MaxValue;
//            // We consider that Removes are uncommon and that they will
//            // usually find an item to remove, so we do a lock directly avoiding
//            // the double search done in the GetOrCreateValue (in that case it is
//            // useful because it guarantees the lock-free read when the items are
//            // there).
//            lock (_lock)
//            {
//                int bucketIndex = hashCode % _baseNodes.Length;

//                _Node previousNode = null;
//                _Node node = _baseNodes[bucketIndex];
//                while (node != null)
//                {
//                    if (hashCode == node._hashCode)
//                    {
//                        if (_comparer.Equals(key, node._key))
//                        {
//                            if (previousNode != null)
//                                previousNode._nextNode = node._nextNode;
//                            else
//                                _baseNodes[bucketIndex] = node._nextNode;

//                            _count--;
//                            return true;
//                        }
//                    }

//                    previousNode = node;
//                    node = node._nextNode;
//                }
//            }

//            return false;
//        }

//        void _Resize()
//        {
//            int newSize;
//            checked
//            {
//                newSize = DictionaryHelper.AdaptSize(_baseNodes.Length * 2);
//            }

//            var newNodes = new _Node[newSize];

//            foreach (var baseNode in _baseNodes)
//            {
//                var oldNode = baseNode;
//                while (oldNode != null)
//                {
//                    int hashCode = oldNode._hashCode;
//                    int bucketIndex = hashCode % newSize;
//                    var newNode = new _Node(hashCode, newNodes[bucketIndex], oldNode._key, oldNode._value);
//                    newNodes[bucketIndex] = newNode;

//                    oldNode = oldNode._nextNode;
//                }
//            }

//            _baseNodes = newNodes;
//        }
//    }
//}
