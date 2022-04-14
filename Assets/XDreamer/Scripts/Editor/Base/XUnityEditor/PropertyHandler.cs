using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.PropertyHandler")]
    public class PropertyHandler : LinkType<PropertyHandler>
    {
        public PropertyHandler(object obj) : base(obj) { }
    }
}
