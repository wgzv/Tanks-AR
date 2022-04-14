using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Tools
{
    /// <summary>
    /// 编辑器序列化对象组手
    /// </summary>
    public static class EditorSerializedObjectHelper
    {
        /// <summary>
        /// 处理规则:对象Unity对象数组（列表）中的元素进行处理的规则
        /// </summary>
        [Name("处理规则")]
        [Tip("对象Unity对象数组（列表）中的元素进行处理的规则")]
        public enum EUnityObjectArrayHandleRule
        {
            /// <summary>
            /// 名称排序：根据元素的名称对对象数组（列表）执行升序排序
            /// </summary>
            [Name("名称")]
            [Tip("根据元素的名称对对象数组（列表）执行升序排序")]
            [XCSJ.Attributes.Icon(EIcon.NameAscendingOrder)]
            NameSort,

            /// <summary>
            /// 名称路径排序：根据元素的名称路径对对象数组（列表）执行升序排序
            /// </summary>
            [Name("名称路径")]
            [Tip("根据元素的名称路径对对象数组（列表）执行升序排序")]
            [XCSJ.Attributes.Icon(EIcon.NameAscendingOrder)]
            NamePathSort,

            /// <summary>
            /// 逆序：将对象数组（列表）中元素逆序
            /// </summary>
            [Name("逆序")]
            [Tip("将对象数组（列表）中元素逆序")]
            [XCSJ.Attributes.Icon(EIcon.ReverseOrder)]
            Reverse,

            /// <summary>
            /// 无效：将对象数组（列表）中无效元素移除
            /// </summary>
            [Name("无效")]
            [Tip("将对象数组（列表）中无效元素移除")]
            [XCSJ.Attributes.Icon(EIcon.Delete)]
            DeleteInvalid,

            /// <summary>
            /// 去重：将对象数组（列表）中重复元素移除
            /// </summary>
            [Name("去重")]
            [Tip("将对象数组（列表）中重复元素移除")]
            [XCSJ.Attributes.Icon(EIcon.Delete)]
            Distinct,
        }

        /// <summary>
        /// 绘制Unity对象数组处理规则
        /// </summary>
        /// <param name="unityObjectArray"></param>
        public static void DrawUnityObjectArrayHandleRule(this SerializedProperty unityObjectArray)
        {
            DrawUnityObjectArrayHandleRule(unityObjectArray, UnityObjectArrayHandle);
        }

        /// <summary>
        /// 绘制Unity对象数组处理规则
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="onClick"></param>
        private static void DrawUnityObjectArrayHandleRule(SerializedProperty serializedProperty, Action<EUnityObjectArrayHandleRule, SerializedProperty> onClick)
        {
            UICommonFun.EnumButton<EUnityObjectArrayHandleRule>(sr => onClick?.Invoke(sr, serializedProperty), true, false, null, null, null, null, ENameTip.Image, GUILayout.ExpandWidth(true), GUILayout.Height(20));
        }

        /// <summary>
        /// Unity对象数组处理
        /// </summary>
        /// <param name="sortRule"></param>
        /// <param name="unityObjectArray"></param>
        private static void UnityObjectArrayHandle(EUnityObjectArrayHandleRule rule, SerializedProperty unityObjectArray)
        {
            switch (rule)
            {
                case EUnityObjectArrayHandleRule.NameSort:
                    {
                        SerializedObjectHelper.ArrayElementSort(unityObjectArray, (a, b) =>
                        {
                            var oa = a.serializedProperty.objectReferenceValue;
                            var ob = b.serializedProperty.objectReferenceValue;

                            if (!oa && !ob) return 0;
                            if (!oa) return -1;
                            if (!ob) return 1;

                            return UICommonFun.NaturalCompare(oa.name, ob.name);
                        });
                        break;
                    }
                case EUnityObjectArrayHandleRule.NamePathSort:
                    {
                        SerializedObjectHelper.ArrayElementSort(unityObjectArray, (a, b) =>
                        {
                            var oa = a.serializedProperty.objectReferenceValue;
                            var ob = b.serializedProperty.objectReferenceValue;

                            if (!oa && !ob) return 0;
                            if (!oa) return -1;
                            if (!ob) return 1;

                            return UICommonFun.NaturalCompare(CommonFun.ObjectToString(oa), CommonFun.ObjectToString(ob));
                        });
                        break;
                    }
                case EUnityObjectArrayHandleRule.Reverse:
                    {
                        SerializedObjectHelper.ArrayElementReverse(unityObjectArray);
                        break;
                    }
                case EUnityObjectArrayHandleRule.Distinct:
                    {
                        SerializedObjectHelper.ArrayElementDistinct(unityObjectArray, (x, y) => x.serializedProperty.objectReferenceValue == y.serializedProperty.objectReferenceValue);
                        break;
                    }
                case EUnityObjectArrayHandleRule.DeleteInvalid:
                    {
                        SerializedObjectHelper.DeleteArrayInvalidElements(unityObjectArray);
                        break;
                    }
            }
            GUI.FocusControl("");
        }
    }
}
