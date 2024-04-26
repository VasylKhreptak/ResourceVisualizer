using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Serialization
{
    [Serializable]
    public class SerializedDictionary<K, V> : SerializedDictionary<K, V, K, V>
    {
        public override K SerializeKey(K key) => key;

        public override V SerializeValue(V val) => val;

        public override K DeserializeKey(K key) => key;

        public override V DeserializeValue(V val) => val;
    }

    [Serializable]
    public abstract class SerializedDictionary<K, V, SK, SV> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SK> _keys = new List<SK>();

        [SerializeField] private List<SV> _values = new List<SV>();

        public abstract SK SerializeKey(K key);

        public abstract SV SerializeValue(V value);

        public abstract K DeserializeKey(SK serializedKey);

        public abstract V DeserializeValue(SV serializedValue);

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (KeyValuePair<K, V> kvp in this)
            {
                _keys.Add(SerializeKey(kvp.Key));
                _values.Add(SerializeValue(kvp.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                Add(DeserializeKey(_keys[i]), DeserializeValue(_values[i]));
            }

            _keys.Clear();
            _values.Clear();
        }
    }
}