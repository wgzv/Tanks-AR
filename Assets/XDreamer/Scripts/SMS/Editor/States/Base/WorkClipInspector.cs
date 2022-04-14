using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Kernel;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 工作剪辑检查器
    /// </summary>
    [CustomEditor(typeof(WorkClip), true)]
    [Serializable]
    public class WorkClipInspector : WorkClipInspector<WorkClip> { }

    /// <summary>
    /// 工作剪辑检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WorkClipInspector<T> : StateComponentInspector<T> where T : WorkClip
    {
        /// <summary>
        /// 工作剪辑对象
        /// </summary>
        public T workClip => target as T;

        /// <summary>
        /// 单次时长字符串
        /// </summary>
        public const string OnceTimeLengthString = SMSHelper.Prefix + nameof(WorkClip.onceTimeLength);

        private bool workClipValidity = true;

        private Color guiColor;

        private const float ButtonSize = 32;

        #region 锁定时间与百分比的比例关系

        private Vector2 lockRatioButtonSize = new Vector2(ButtonSize, ButtonSize);

        private string EndString(string name) => string.Format(".{0}.{0}", name);

        /// <summary>
        /// 同步工作区间
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        protected void SyncWorkRange(SerializedProperty memberProperty, string to, Vector2 value)
        {
            var path = memberProperty.propertyPath.Replace(EndString(memberProperty.name), EndString(to));
            var sp = memberProperty.serializedObject.FindProperty(path);
            if (sp != null) sp.vector2Value = value;
        }

        /// <summary>
        /// 限定百分比范围到[0,1]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Vector2 ClampPercent(Vector2 value)
        {
            value.x = Mathf.Clamp01(value.x);
            value.y = Mathf.Clamp01(value.y);
            return value;
        }

        #endregion

        #region 同步TL

        /// <summary>
        /// 标识是否有同步时长按钮
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasSyncTLButton() => false;

        /// <summary>
        /// 获取同步时长按钮内容
        /// </summary>
        /// <returns></returns>
        protected virtual GUIContent GetSyncTLButtonContent() => CommonFun.NameTooltip(workClip, nameof(workClip.syncTL), ENameTip.EmptyTextWhenHasImage);

        protected void SetTLByTimeRange(SerializedProperty timeRangeMemberProperty, float timeLength)
        {
            var v2 = new Vector2(timeRangeMemberProperty.vector2Value.x, timeRangeMemberProperty.vector2Value.x + timeLength);
            timeRangeMemberProperty.vector2Value = v2;
            //同步更新百分比
            if (workClip.lockRatioOfWorkRange && !MathX.ApproximatelyZero(workClip.ttlOfLockRatio))
            {
                SyncWorkRange(timeRangeMemberProperty, nameof(PercentRange.percentRange), ClampPercent(v2 / workClip.ttlOfLockRatio));
            }
        }

        /// <summary>
        /// 通过工作区间设置时长
        /// </summary>
        /// <param name="workRangeMemberProperty"></param>
        /// <param name="timeLength"></param>
        protected void SetTLByWorkRange(SerializedProperty workRangeMemberProperty, float timeLength)
        {
            SetTLByTimeRange(workRangeMemberProperty.FindPropertyRelative(nameof(WorkRange.timeRange)).FindPropertyRelative(nameof(TimeRange.timeRange)), timeLength);
        }

        /// <summary>
        /// 当同步时长时
        /// </summary>
        /// <param name="workRangeMemberProperty"></param>
        protected virtual void OnSyncTL(SerializedProperty workRangeMemberProperty) { }

        /// <summary>
        /// 默认同步时长按钮
        /// </summary>
        /// <param name="workRangeMemberProperty"></param>
        protected void DefaultSyncTLButton(SerializedProperty workRangeMemberProperty)
        {
            if (workClip.syncTL = UICommonFun.ButtonToggle(GetSyncTLButtonContent(), workClip.syncTL, EditorStyles.miniButtonMid, GUILayout.Width(lockRatioButtonSize.x), GUILayout.Height(lockRatioButtonSize.y)))
            {
                OnSyncTL(workRangeMemberProperty);

                workClip.lockRatioOfWorkRange = false;
                workClip.ttlOfLockRatio = workClip.totalTimeLength;
            }
        }

        #endregion

        #region 同步OTL

        /// <summary>
        /// 获取同步单次时长按钮内容
        /// </summary>
        /// <returns></returns>
        protected virtual GUIContent GetSyncOTLButtonContent() => CommonFun.NameTooltip(workClip, nameof(workClip.syncOTL));

        /// <summary>
        /// 当同步单次时长时
        /// </summary>
        /// <param name="memberProperty"></param>
        protected virtual void OnSyncOTL(SerializedProperty memberProperty)
        {
            if (workClip.syncOTL) memberProperty.floatValue = workClip.timeLength;
            else if (memberProperty.floatValue < 0) memberProperty.floatValue = 0;
        }

        #endregion

        #region 工作曲线

        /// <summary>
        /// 工作曲线范围
        /// </summary>
        public virtual Rect workCurveRange { get; } = new Rect(0, 0, 1, 1);

        /// <summary>
        /// 工作曲线颜色
        /// </summary>
        public virtual Color workCurveColor { get; } = Color.green;

        /// <summary>
        /// 曲线库按钮尺寸
        /// </summary>
        public Vector2 curveLibButtonSize = new Vector2(ButtonSize, ButtonSize);

        /// <summary>
        /// XDreamer曲线库
        /// </summary>
        [Name(Product.Name + "曲线库")]
        [Tip("点击打开[" + Product.Name + "曲线库]")]
        [XCSJ.Attributes.Icon(EIcon.Curve)]
        public XGUIContent XDreamerCurveLib { get; } = new XGUIContent(typeof(WorkClipInspector), nameof(XDreamerCurveLib), true);

        /// <summary>
        /// 百分比
        /// </summary>
        [Name("百分比")]
        [Tip("根据工作曲线通过输入百分比值(即横坐标=百分比,百分比范围值[0,1])计算得出纵坐标值;本值仅用于界面显示,不存储也不影响逻辑执行;")]
        public float percent = 0;

        /// <summary>
        /// 时间
        /// </summary>
        [Name("时间")]
        [Tip("根据工作曲线通过输入时间值(即横坐标=时间值/单次时长,时间值范围[0,单次时长])计算得出纵坐标值;本值仅用于界面显示,,不存储也不影响逻辑执行;;")]
        public float time = 0;

        /// <summary>
        /// 标题宽度
        /// </summary>
        public const float titleWidth = 80;

        /// <summary>
        /// 锁定百分比与时间的比例关系
        /// </summary>
        [Name("锁定\n比例")]
        [Tip("锁定百分比与时间的比例关系,根据锁定时当前状态组件单次时长,对二者进行等比例同步调整;即其中一横坐标修改，另一横坐标数据将同步进行等比例的数据修改;")]
        public bool lockPercentTimeRatio = true;

        /// <summary>
        /// 当绘制工作曲线时
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <returns></returns>
        protected virtual bool OnDrawWorkCurve(SerializedProperty memberProperty)
        {
            try
            {
                #region AnimationCurve绘制

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                var curve = EditorGUILayout.CurveField(CommonFun.NameTooltip(workClip, nameof(workClip.workCurve)), memberProperty.animationCurveValue, workCurveColor, workCurveRange, GUILayout.Height(curveLibButtonSize.y));
                if (EditorGUI.EndChangeCheck())
                {
                    curve.preWrapMode = WrapMode.Clamp;
                    curve.postWrapMode = WrapMode.Clamp;
                    memberProperty.animationCurveValue = curve;
                }

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button(XDreamerCurveLib, EditorStyles.miniButtonRight, GUILayout.Width(curveLibButtonSize.x), GUILayout.Height(curveLibButtonSize.y - 2)))
                {
                    CurvePresetLibraryHelper.ShowXDreamer();
                    CurveEditorWindow.ShowCurveEditorWindow(curve, memberProperty, workCurveRange, workCurveColor);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    memberProperty.animationCurveValue.preWrapMode = WrapMode.Clamp;
                    memberProperty.animationCurveValue.postWrapMode = WrapMode.Clamp;
                    curve = memberProperty.animationCurveValue;
                }

                EditorGUILayout.EndHorizontal();

                #endregion

                #region 计算器

                #region 标题

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("计算类型", GUILayout.Width(titleWidth));
                EditorGUILayout.LabelField(CommonFun.TempContent("横坐标", "时间进度百分比,范围[0,1]"));
                EditorGUILayout.LabelField(CommonFun.TempContent("纵坐标", "逻辑进度百分比,理论范围[0,1]"), GUILayout.Width(titleWidth));
                EditorGUILayout.LabelField("操作", GUILayout.Width(lockRatioButtonSize.x));
                EditorGUILayout.EndHorizontal();

                #endregion

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();

                var otl = workClip.onceTimeLength;

                #region percent

                UICommonFun.BeginHorizontal(true);
                EditorGUILayout.LabelField(CommonFun.NameTooltip(this, nameof(percent)), GUILayout.Width(titleWidth));
                EditorGUI.BeginChangeCheck();
                percent = EditorGUILayout.Slider(percent, 0, 1);
                if(EditorGUI.EndChangeCheck() && lockPercentTimeRatio)
                {
                    time = otl * percent;
                }
                EditorGUILayout.SelectableLabel(curve.Evaluate(percent).ToString(), GUILayout.Width(titleWidth), GUILayout.Height(lockRatioButtonSize.y / 2));
                UICommonFun.EndHorizontal();

                #endregion

                #region time

                UICommonFun.BeginHorizontal(false);
                EditorGUILayout.LabelField(CommonFun.NameTooltip(this, nameof(time)), GUILayout.Width(titleWidth));               
                EditorGUI.BeginChangeCheck();
                time = EditorGUILayout.Slider(time, 0, otl);
                if (EditorGUI.EndChangeCheck() && lockPercentTimeRatio)
                {
                    percent = MathX.Scale(time, otl);
                }
                EditorGUILayout.SelectableLabel(curve.Evaluate(MathX.Scale(time, otl)).ToString(), GUILayout.Width(titleWidth), GUILayout.Height(lockRatioButtonSize.y / 2));
                UICommonFun.EndHorizontal();

                #endregion

                EditorGUILayout.EndVertical();

                lockPercentTimeRatio = UICommonFun.ButtonToggle(CommonFun.NameTooltip(this, nameof(lockPercentTimeRatio)), lockPercentTimeRatio, GUILayout.Width(lockRatioButtonSize.x), GUILayout.Height(lockRatioButtonSize.y));

                EditorGUILayout.EndHorizontal();

                #endregion
            }
            catch (ExitGUIException)
            {
                //忽略本异常
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 当检测是否需要绘制对象的成员时回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(TimeRange.timeRange):
                    {
                        if (!memberProperty.propertyPath.EndsWith(EndString(memberProperty.name))) break;

                        EditorGUILayout.BeginHorizontal();
                        var max = TimeRange.DefaultMaxTimeLength;
                        if (workClip.lockRatioOfWorkRange)
                        {
                            max = workClip.ttlOfLockRatio;
                        }
                        else if (HasSyncTLButton() && workClip.syncTL)
                        {
                            max = workClip.totalTimeLength;
                        }

                        var v2 = memberProperty.vector2Value;
                        EditorGUI.BeginChangeCheck();
                        UICommonFun.MinMaxSliderLayout(CommonFun.NameTooltip(typeof(TimeRange), nameof(TimeRange.timeRange)), ref v2.x, ref v2.y, 0, max, 80);
                        if (EditorGUI.EndChangeCheck())
                        {
                            memberProperty.vector2Value = v2;
                            if (workClip.lockRatioOfWorkRange && !MathX.ApproximatelyZero(workClip.ttlOfLockRatio))
                            {
                                SyncWorkRange(memberProperty, nameof(PercentRange.percentRange), ClampPercent(v2 / workClip.ttlOfLockRatio));
                            }

                            if(workClip.syncOTL)
                            {
                                memberProperty.serializedObject.FindProperty(OnceTimeLengthString).floatValue = v2.y - v2.x;
                                //Debug.Log("同步更新OTL:" + memberProperty.serializedObject.FindProperty(OnceTimeLengthString).floatValue);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        return false;
                    }
                case nameof(WorkClip.percentOnEntry):
                case nameof(WorkClip.percentOnExit):
                case nameof(WorkClip.leastLoopCount):
                case nameof(WorkClip.percentOnAfterWorkRange):
                    {
                        memberProperty.floatValue = EditorGUILayout.Slider(CommonFun.NameTooltip(workClip, memberProperty.name), memberProperty.floatValue, 0, workClip.loopCount);
                        return false;
                    }
                case nameof(WorkClip.workCurve):
                    {
                        return OnDrawWorkCurve(memberProperty);
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制对象的成员属性之前回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnBeforePropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(WorkClip.workRange):
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (!(workClipValidity = WorkClip.WorkClipValidity(workClip)))
                        {
                            guiColor = GUI.backgroundColor;
                            GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                        }
                        break;
                    }
                case nameof(PercentRange.percentRange):
                    {
                        EditorGUI.BeginChangeCheck();
                        break;
                    }
            }
            base.OnBeforePropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(workClip.workRange):
                    {
                        if (!workClipValidity)
                        {
                            GUI.backgroundColor = guiColor;
                        }

                        if (HasSyncTLButton())
                        {
                            DefaultSyncTLButton(memberProperty);
                        }

                        if (workClip.lockRatioOfWorkRange = UICommonFun.ButtonToggle(CommonFun.NameTooltip(workClip, nameof(workClip.lockRatioOfWorkRange), ENameTip.EmptyTextWhenHasImage), workClip.lockRatioOfWorkRange, EditorStyles.miniButtonRight, GUILayout.Width(lockRatioButtonSize.x), GUILayout.Height(lockRatioButtonSize.y)))
                        {
                            workClip.syncTL = false;
                            workClip.ttlOfLockRatio = workClip.totalTimeLength;
                        }

                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                case nameof(PercentRange.percentRange):
                    {
                        if (!memberProperty.propertyPath.EndsWith(EndString(memberProperty.name))) break;
                        if (EditorGUI.EndChangeCheck() && workClip.lockRatioOfWorkRange && !MathX.ApproximatelyZero(workClip.ttlOfLockRatio))
                        {
                            var value = memberProperty.vector2Value * workClip.ttlOfLockRatio;
                            SyncWorkRange(memberProperty, nameof(TimeRange.timeRange), value);
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当横向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(PercentRange.percentRange):
                    {
                        if (!memberProperty.propertyPath.EndsWith(EndString(memberProperty.name))) break;
                        //if (HasSyncTLButton()) EditorGUILayout.LabelField("", GUILayout.Width(syncTLButtonWidth));

                        break;
                    }
                case OnceTimeLengthString:
                    {
                        var sp = memberProperty.serializedObject.FindProperty(nameof(workClip.syncOTL));
                        sp.boolValue = UICommonFun.ButtonToggle(GetSyncOTLButtonContent(), sp.boolValue, EditorStyles.miniButtonRight, GUILayout.Width(60));
                        OnSyncOTL(memberProperty);
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (workClip.loop)
            {
                info.Append("\n循环信息:");
                info.AppendFormat("\n\t单次时长:\t{0}", workClip.onceTimeLength);
                info.AppendFormat("\n\t单次%长:\t{0}", workClip.oncePercentLength);
                info.AppendFormat("\n\t循环次数:\t{0}", workClip.loopCount);
            }
            return info;
        }

        /// <summary>
        /// 获取运行时辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetRuntimeHelpInfo()
        {
            var info = base.GetRuntimeHelpInfo();
            if (workClip.loop)
            {
                info.Append("\n循环运行时信息:");
                info.AppendFormat("\n\t总进度(已循环次数):\t{0}", workClip.percent.percent);
                info.AppendFormat("\n\t进度[0,1]:\t\t{0}", workClip.percent.percent01);
                info.AppendFormat("\n\t工作曲线进度:\t{0}", workClip.percent.percentOfWorkCurve);
            }
            return info;
        }
    }
}
