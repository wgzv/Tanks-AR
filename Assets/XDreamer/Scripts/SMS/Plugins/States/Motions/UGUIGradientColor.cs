﻿using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// UGUI渐变色:GUI渐变色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(UGUIGradientColor))]
    [Tip("UGUI渐变色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。")]
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [RequireComponent(typeof(GameObjectSet))]
    public class UGUIGradientColor : Motion<UGUIGradientColor, UGUIGradientColor.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI渐变色";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(UGUIGradientColor))]
        [Tip("UGUI渐变色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Name("渐变色")]
        public Gradient gradient = new Gradient();

        [Name("包含成员")]
        public bool includeChildren = true;

        public class Recorder : GraphicRecorder, IPercentRecorder<UGUIGradientColor>
        {
            private UGUIGradientColor color;

            public void Record(UGUIGradientColor color)
            {
                this.color = color;
                if (!color.gameObjectSet) return;
                _records.Clear();
                foreach (var go in color.gameObjectSet.objects)
                {
                    if (go)
                    {
                        if (color.includeChildren)
                        {
                            Record(go.GetComponentsInChildren<Graphic>());
                        }
                        else
                        {
                            Record(go);
                        }
                    }
                }
            }

            public void SetPercent(Percent percent)
            {
                var color = this.color.gradient.Evaluate(percent.percent01OfWorkCurve);
                foreach (var i in _records)
                {
                    i.SetColor(color);
                }
            }
        }
    }
}
