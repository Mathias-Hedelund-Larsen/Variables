using UnityEngine;

namespace FoldergeistAssets
{
    namespace Variables
    {
        public abstract class ReadOnlyVariable<T> : ScriptableObject
        {
            [SerializeField]
            protected T _value;

            public T Value { get => _value; }
        }
    }
}
