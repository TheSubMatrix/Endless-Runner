using System;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace MatrixUtils.DependencyInjection
{
    //Yes, this is dumb, but I need it to work in the editor with the property drawer and in the runtime without it.
    #if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class InjectAttribute : PropertyAttribute { }
    #else
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute { }
    #endif
}