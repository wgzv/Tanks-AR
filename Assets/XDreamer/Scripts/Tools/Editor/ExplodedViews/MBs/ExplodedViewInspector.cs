using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.PluginTools.ExplodedViews;
using XCSJ.PluginTools.ExplodedViews.MBs;

namespace XCSJ.EditorTools.ExplodedViews.MBs
{
    /// <summary>
    /// 爆炸图检查器
    /// </summary>
    [CustomEditor(typeof(ExplodedView))]
    public class ExplodedViewInspector : MBInspector<ExplodedView>
    {
        private bool inSimulation = false;

        private float percent = 0;

        /// <summary>
        /// 禁用时
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            StopSimulate();
        }

        public override void OnBeforeArraySizeVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(ExplodedView._explodedTargets):
                    {
                        EditorSerializedObjectHelper.DrawUnityObjectArrayHandleRule(memberProperty);
                        break;
                    }
            }
            base.OnBeforeArraySizeVertical(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            if (Application.isPlaying) return;

            if (GUILayout.Button("启用爆炸图模拟并记录数据"))
            {
                StartSimulate();
            }

            EditorGUI.BeginDisabledGroup(!inSimulation);
            {
                if (GUILayout.Button("停止爆炸图模拟并还原(清除)数据"))
                {
                    StopSimulate();
                }

                EditorGUI.BeginDisabledGroup(mb.datas.Count == 0);
                {
                    if (GUILayout.Button("点爆"))
                    {
                        Explode(EExplodeType.Point);
                    }

                    if (GUILayout.Button("线爆"))
                    {
                        Explode(EExplodeType.Line);
                    }

                    if (GUILayout.Button("面爆"))
                    {
                        Explode(EExplodeType.Plane);
                    }

                    EditorGUI.BeginChangeCheck();
                    percent = EditorGUILayout.Slider(percent, 0, mb.explodeMultiple);
                    if (EditorGUI.EndChangeCheck() && inSimulation)
                    {
                        mb.UpdatePercent(percent);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void StartSimulate()
        {
            if (inSimulation) return;
            inSimulation = true;
            if (mb)
            {
                mb.SetSimulation(true);
                mb.SetDrawInfo(mb.center, mb.direction);
                mb.Record();
            }         
            percent = 0;
        }

        private void StopSimulate()
        {
            inSimulation = false;
            percent = 0;
            if (mb)
            {
                mb.Clear();
                mb.SetSimulation(false);
            }
        }

        private void Explode(EExplodeType explodeType)
        {
            if (mb)
            {
                mb.Recovry();
                //mb.explodeType = explodeType;
                var center = mb.center;
                var direction = mb.direction;
                mb.SetDrawInfo(center, direction);
                mb.datas = ExplodedViewHelper.Explode(explodeType, mb.datas, center, direction, mb.deltaIntervalValue, mb.minIntervalValue, mb._sortRule);
                mb.UpdateTranforms();
            }       
            percent = 1;
        }
    }
}
