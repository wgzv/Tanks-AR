using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    [Serializable]
    public class Content : Element
    {
        public Element name = new Element(nameof(name));
        public Element tip = new Element(nameof(tip));

        [Json(false)]
        public GUIContent content => new GUIContent(name.value, tip.value);
    }
}
