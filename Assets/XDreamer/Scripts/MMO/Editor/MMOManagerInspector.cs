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
using XCSJ.Caches;
using XCSJ.ComponentModel;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.Scripts;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 多人在线MMO检查器
    /// </summary>
    [CustomEditor(typeof(MMOManager))]
    public class MMOManagerInspector : BaseManagerInspector<MMOManager>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            if (Application.isPlaying) return;

            if (!manager.GetComponent<MMOManagerHUD>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(MMOManagerHUD))))
                {
                    Undo.AddComponent<MMOManagerHUD>(manager.gameObject);
                }
            }

            if (!manager.GetComponent<MMOPlayerCreater>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(MMOPlayerCreater))))
                {
                    Undo.AddComponent<MMOPlayerCreater>(manager.gameObject);
                }
            }

            DrawDetailInfos();

            MMOMBInspector.categoryList.DrawVertical();
        }

        /// <summary>
        /// 网络标识列表
        /// </summary>
        [Name("网络标识列表")]
        [Tip("当前场景中所有的网络标识对象")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("网络标识", "网络标识所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("本地权限", "详细解释请查看网络标识对象上对应属性的具体解释；本项只读；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("权限", "详细解释请查看网络标识对象上对应属性的具体解释；本项只读；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(NetIdentity), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as NetIdentity;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //网络标识
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //本地权限
                EditorGUILayout.Toggle(component.localAccess, UICommonOption.Width60);

                //权限
                EditorGUILayout.Toggle(component.access, UICommonOption.Width60);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
