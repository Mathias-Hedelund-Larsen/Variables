using System;
using UnityEngine;

namespace HephaestusForge
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
                protected bool _useConstant = true;

#pragma warning restore 0649
            }
        }
    }
}
