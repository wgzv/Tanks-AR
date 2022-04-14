using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    public abstract class UnityAnimator<T> : WorkClip<T> where T : UnityAnimator<T>
    {
        [Group("Animator属性")]
        [Name("Animator")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Animator))]
        public Animator animator = null;

        [Name("层索引")]
        public int layerIndex = 0;

        [Name("状态名称")]
        public string stateName = "Take 001";

        public void PlayAnimator(float percent)
        {
            try
            {
                if (!animator) return;
                animator.speed = 0;
                animator.Play(stateName, layerIndex, percent);
            }
            catch (Exception ex)
            {
                Log.ExceptionFormat("执行[{0}]中函数[{1}]异常:{2}", GetType(), nameof(PlayAnimator), ex);
            }
        }

        protected override void OnSetPercent(Percent percent, StateData stateData) => PlayAnimator(percent.percent01OfWorkCurve);
       
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            switch(data.dataRule)
            {
                case EDataRule.Init:
                case EDataRule.Entry:
                    {
                        SetPercent(0);
                        break;
                    }
            }
        }

        public override bool DataValidity()
        {
            return animator;
        }
    }
}
