using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using My.Helpers;

namespace My.WinformMvc.Validation
{
    public class ModelError
    {
        public ModelError(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            Exception = exception;
        }

        public ModelError(string errorMessage)
        {
            if (errorMessage == null)
                throw new ArgumentNullException("errorMessage");
            ErrorMessage = errorMessage;
        }

        public ModelError(Exception exception, string errorMessage)
        {
            if (exception == null && errorMessage == null)
                throw new ArgumentNullException("");
            Exception = exception;
            ErrorMessage = errorMessage ?? string.Empty;
        }

        public Exception Exception { get; private set; }

        public string ErrorMessage { get; private set; }
    }

    public class ModelErrorCollection : Collection<ModelError>
    {
        public string PropertyName { get; internal set; }

        public string DisplayName { get; internal set; }

        public void Add(Exception exception)
        {
            base.Add(new ModelError(exception));
        }

        public void Add(string errorMessage)
        {
            base.Add(new ModelError(errorMessage));
        }

        public void Add(Exception exception, string errorMessage)
        {
            base.Add(new ModelError(exception, errorMessage));
        }
    }

    public class ModelState : IDictionary<string, ModelErrorCollection>
    {
        readonly Dictionary<string, ModelErrorCollection> _innerDictionary 
            = new Dictionary<string, ModelErrorCollection>(StringComparer.OrdinalIgnoreCase);

        public ModelState()
        {
        }

        public ModelState(ModelState state)
        {
            Requires.NotNull(state, "state");
            foreach (KeyValuePair<string, ModelErrorCollection> current in state)
            {
                _innerDictionary.Add(current.Key, current.Value);
            }
        }

        public int Count
        {
            get { return _innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, ModelErrorCollection>>)_innerDictionary).IsReadOnly; }
        }

        public bool IsValid
        {
            get { return _innerDictionary.Count > 0; }
        }

        public ICollection<string> Keys
        {
            get { return _innerDictionary.Keys; }
        }

        public ICollection<ModelErrorCollection> Values
        {
            get { return _innerDictionary.Values; }
        }

        public ModelErrorCollection this[string key]
        {
            get
            {
                Requires.NotNullOrWhiteSpace(key, "key");
                ModelErrorCollection result;
                _innerDictionary.TryGetValue(key, out result);
                return result;
            }
            set
            {
                Requires.NotNullOrWhiteSpace(key, "key");
                Requires.NotNull(value, "ModelErrorCollection");
                _innerDictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<string, ModelErrorCollection> item)
        {
            Requires.NotNull(item, "item");
            ((ICollection<KeyValuePair<string, ModelErrorCollection>>)_innerDictionary).Add(item);
        }

        public void Add(string key, ModelErrorCollection value)
        {
            Requires.NotNullOrWhiteSpace(key, "key");
            Requires.NotNull(value, "value");
            _innerDictionary.Add(key, value);
        }

        public void AddModelError(string key, Exception exception)
        {
            Requires.NotNullOrWhiteSpace(key, "key");
            GetOrAddModelErrorCollection(key).Add(exception);
        }

        public void AddModelError(string key, string errorMessage)
        {
            Requires.NotNullOrWhiteSpace(key, "key");
            GetOrAddModelErrorCollection(key).Add(errorMessage);
        }

        public void AddModelError(string key, Exception exception, string errorMessage)
        {
            Requires.NotNullOrWhiteSpace(key, "key");
            GetOrAddModelErrorCollection(key).Add(exception, errorMessage);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, ModelErrorCollection> item)
        {
            Requires.NotNull(item, "item");
            return ((ICollection<KeyValuePair<string, ModelErrorCollection>>)_innerDictionary).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            Requires.NotNullOrWhiteSpace(key, "key");
            return _innerDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, ModelErrorCollection>[] array, int arrayIndex)
        {
            Requires.NotNull(array, "array");
            ((ICollection<KeyValuePair<string, ModelErrorCollection>>)_innerDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, ModelErrorCollection>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        ModelErrorCollection GetOrAddModelErrorCollection(string key)
        {
            ModelErrorCollection modelState;
            if (_innerDictionary.TryGetValue(key, out modelState))
                return modelState;

            modelState = new ModelErrorCollection();
            _innerDictionary.Add(key, modelState);
            return modelState;
        }

        public void Merge(ModelState dictionary)
        {
            if (dictionary == null)
                return;
            foreach (KeyValuePair<string, ModelErrorCollection> current in dictionary)
            {
                this[current.Key] = current.Value;
            }
        }

        public bool Remove(KeyValuePair<string, ModelErrorCollection> item)
        {
            Requires.NotNull(item, "item");
            return ((ICollection<KeyValuePair<string, ModelErrorCollection>>)_innerDictionary).Remove(item);
        }

        public bool Remove(string key)
        {
            Requires.NotNullOrWhiteSpace(key, "key");
            return _innerDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out ModelErrorCollection value)
        {
            Requires.NotNullOrWhiteSpace(key, "key");
            return _innerDictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_innerDictionary).GetEnumerator();
        }
    }
}
