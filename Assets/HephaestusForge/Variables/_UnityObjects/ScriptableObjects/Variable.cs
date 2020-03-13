using UnityEngine;

namespace HephaestusForge
{
    namespace Variables
    {
        public abstract class Variable<T> : ScriptableObject
        {
            [SerializeField]
            protected T _value;

            public T Value { get => _value; set => _value = value; }
        }
    }
}
