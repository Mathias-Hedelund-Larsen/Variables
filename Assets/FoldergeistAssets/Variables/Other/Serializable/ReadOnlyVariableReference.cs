using UnityEngine;

namespace FoldergeistAssets
{
    namespace Variables
    {
        public abstract class ReadOnlyVariableReference<T1, T2> : Internal.VariableReference where T2 : Variable<T1>
        {
#pragma warning disable 0649

            [SerializeField, HideInInspector]
            private T1 _constantValue;

            [SerializeField, HideInInspector]
            private T2 _variabelValue;

#pragma warning restore 0649

            public T1 Value
            {
                get
                {
                    return _useConstant ? _constantValue : _variabelValue.Value;
                }
            }
        }
    }
}
