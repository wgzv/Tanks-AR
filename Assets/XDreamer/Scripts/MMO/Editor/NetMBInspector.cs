using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.Scripts;

namespace XCSJ.EditorMMO
{
    [CustomEditor(typeof(NetMB), true)]
    public class NetMBInspector : NetMBInspector<NetMB> { }

    public class NetMBInspector<T> : MMOMBInspector<T> where T : NetMB
    {
        private Type hudType = null;
        private Type windowType = null;
        private MMOOption option = null;
        private List<string> allSyncVars = null;

        private GUIStyle _syncVarGUIStyle;
        private GUIStyle syncVarGUIStyle
        {
            get
            {
                if (_syncVarGUIStyle == null)
                {
                    _syncVarGUIStyle = new GUIStyle(GUI.skin.box);
                    _syncVarGUIStyle.normal.background = Texture2DHelper.GetTexture2D(option.syncVarHighlightColor);
                }
                return _syncVarGUIStyle;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            hudType = Caches.TypeCache.Get(targetObject.GetType().FullName + "HUD");
            windowType = Caches.TypeCache.Get(targetObject.GetType().FullName + "Window");

            MMOOption.onModified += OnOptionModified;
            option = MMOOption.weakInstance;

            allSyncVars = mb.GetAllSyncVarNames();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            MMOOption.onModified -= OnOptionModified;
        }

        private void OnOptionModified(MMOOption option)
        {
            this.option = option;
            _syncVarGUIStyle = null;
            Repaint();
        }

        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            if (hudType != null && !targetObject.GetComponent(hudType))
            {
                if (GUILayout.Button(CommonFun.NameTip(hudType)))
                {
                    Undo.AddComponent(targetObject.gameObject, hudType);
                }
            }
            if (windowType != null && !targetObject.GetComponent(windowType))
            {
                //if (GUILayout.Button(CommonFun.NameTip(windowType)))
                //{
                //    Undo.AddComponent(targetObject.gameObject, windowType);
                //}
            }
        }

        private bool IsSyncVar(Type type, SerializedProperty memberProperty) => type == mb.GetType() && allSyncVars.Contains(memberProperty.name);
        protected void BeginSyncVar(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (IsSyncVar(type, memberProperty) && option.syncVarHighlight)
            {
                EditorGUILayout.BeginVertical(syncVarGUIStyle);
            }
        }
        protected void EndSyncVar(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (IsSyncVar(type, memberProperty) && option.syncVarHighlight)
            {
                EditorGUILayout.EndVertical();
            }
        }

        public override void OnBeforePropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            BeginSyncVar(type, memberProperty, arrayElementInfo);
            base.OnBeforePropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
            EndSyncVar(type, memberProperty, arrayElementInfo);
        }
    }
}
