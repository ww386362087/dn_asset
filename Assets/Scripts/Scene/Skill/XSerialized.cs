#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace XEditor
{
    public class XSerialized<T> where T : class
    {
        //deep copy of T
        private static string SerializeToString(T value)
        {
            using (MemoryStream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, value);
                objectStream.Flush();
                return Convert.ToBase64String(objectStream.ToArray());
            }
        }

        private static T DeserializeFromString(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return (T)(new BinaryFormatter()).Deserialize(stream);
            }
        }

        [SerializeField]
        private string _serializedData;

        protected T _class;

        public XSerialized() { }

        public XSerialized(T _class)
        {
            Set(_class);
        }

        public void Set(T _class)
        {
            this._class = _class;
            Serialize();
        }

        public T Get()
        {
            if (_class == null) _class = Deserialize();
            return _class;
        }

        public virtual void Serialize()
        {
            _serializedData = SerializeToString(_class);
        }

        protected virtual T Deserialize()
        {
            return DeserializeFromString(_serializedData);
        }
    }
}
#endif