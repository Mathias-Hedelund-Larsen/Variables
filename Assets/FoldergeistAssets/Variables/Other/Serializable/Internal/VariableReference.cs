using System;
using UnityEngine;

namespace FoldergeistAssets
{
    namespace Variables
    {
        namespace Internal
        {
            [Serializable]
            public abstract class VariableReference 
            {
#pragma warning disable 0649

                [SerializeField, HideInInspector]
                protected bool _useConstant;

#pragma warning restore 0649
            }
        }
    }
}
