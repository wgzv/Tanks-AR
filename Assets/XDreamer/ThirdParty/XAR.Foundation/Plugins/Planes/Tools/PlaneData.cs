using System;
using XCSJ.Attributes;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using UnityEngine;
using XCSJ.PluginCommonUtils;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Planes.Tools
{
    /// <summary>
    /// 平面数据
    /// </summary>
    [Serializable]
    public class PlaneData
#if XDREAMER_AR_FOUNDATION
        : TrackData<BoundedPlane, ARPlane>
#else
        : TrackData
#endif
    {
        /// <summary>
        /// 关联规则
        /// </summary>
        [Name("关联规则")]
        [EnumPopup]
        public ELinkRule _linkRule = ELinkRule.ValidMinSize;

        /// <summary>
        /// 关联规则
        /// </summary>
        [Name("关联规则")]
        public enum ELinkRule
        {
            /// <summary>
            /// 无:不做任何操作，即在尝试更新时不执行关联检测操作；
            /// </summary>
            [Name("无")]
            [Tip("不做任何操作，即在尝试更新时不执行关联检测操作；")]
            None,

            /// <summary>
            /// 跟踪顺序：将跟踪识别到的第[跟踪顺序值]个AR平面对象与当前对象执行关联；仅在触发[当已添加时]的跟踪器事件时，执行基于本规则的关联检测操作；
            /// </summary>
            [Name("跟踪顺序")]
            [Tip("将跟踪识别到的第[跟踪顺序值]个AR平面对象与当前对象执行关联；仅在触发[当已添加时]的跟踪器事件时，执行基于本规则的关联检测操作；")]
            TrackingSequence,

            /// <summary>
            /// 有效最小尺寸：可在触发[当已添加时]或[当已更新时]的跟踪器事件时，执行基于本规则的关联检测操作；
            /// </summary>
            [Name("有效最小尺寸")]
            [Tip("可在触发[当已添加时]或[当已更新时]的跟踪器事件时，执行基于本规则的关联检测操作；")]
            ValidMinSize,
        }

        #region 跟踪顺序

        /// <summary>
        /// 跟踪顺序数据
        /// </summary>
        [Serializable]
        public class TrackingSequenceData
        {
            /// <summary>
            /// 跟踪顺序：将跟踪识别到的第[跟踪顺序值]个AR平面对象与当前对象执行关联；
            /// </summary>
            [Name("跟踪顺序")]
            [Tip("将跟踪识别到的第[跟踪顺序值]个AR平面对象与当前对象执行关联；")]
            [Range(1, 8)]
            public int _trackingSequence = 1;

            /// <summary>
            /// 跟踪索引：将跟踪顺序值<see cref="_trackingSequence"/>转化为以0开始的跟踪索引值；
            /// </summary>
            public int trackingIndex
            {
                get => _trackingSequence - 1;
                set => _trackingSequence = value + 1;
            }
        }

        /// <summary>
        /// 跟踪顺序：将跟踪识别到的第[跟踪顺序值]个AR平面对象与当前对象执行关联；
        /// </summary>
        [Name("跟踪顺序")]
        [Tip("将跟踪识别到的第[跟踪顺序值]个AR平面对象与当前对象执行关联；")]
        [HideInSuperInspector(nameof(_linkRule), EValidityCheckType.NotEqual, ELinkRule.TrackingSequence)]
        [OnlyMemberElements]
        public TrackingSequenceData _trackingSequenceData = new TrackingSequenceData();

        #endregion

        #region 有效最小尺寸

        /// <summary>
        /// 有效最小尺寸数据
        /// </summary>
        [Serializable]
        public class ValidMinSizeData
        {
            /// <summary>
            /// 有效最小尺寸：将跟踪识别到尺寸大于[有效最小尺寸]的有效AR平面对象与当前对象执行关联；单位：米；
            /// </summary>
            [Name("有效最小尺寸")]
            [Tip("将跟踪识别到尺寸大于[有效最小尺寸]的有效AR平面对象与当前对象执行关联；单位：米；")]
            public Vector2 _validMinSize = new Vector2(0.5f, 0.5f);

            /// <summary>
            /// 判断是否符合要求的有效平面
            /// </summary>
            /// <param name="planeSize"></param>
            /// <returns></returns>
            public bool ValidPlane(Vector2 planeSize)
            {
                return planeSize.x >= _validMinSize.x && planeSize.y >= _validMinSize.y;
            }
        }

        /// <summary>
        /// 有效最小尺寸：将跟踪识别到尺寸大于[有效最小尺寸]的有效AR平面对象与当前对象执行关联；单位：米；
        /// </summary>
        [Name("有效最小尺寸")]
        [Tip("将跟踪识别到尺寸大于[有效最小尺寸]的有效AR平面对象与当前对象执行关联；单位：米；")]
        [HideInSuperInspector(nameof(_linkRule), EValidityCheckType.NotEqual, ELinkRule.ValidMinSize)]
        [OnlyMemberElements]
        public ValidMinSizeData _validMinSizeData = new ValidMinSizeData();

        #endregion

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 尝试更新
        /// </summary>
        /// <param name="trackable"></param>
        /// <param name="trackEvent"></param>
        /// <param name="trackEntity"></param>
        /// <returns></returns>
        public override bool TryUpdate(ARPlane trackable,ETrackEvent trackEvent, ITrackEntity<BoundedPlane, ARPlane> trackEntity)
        {
            switch (trackEvent)
            {
                case ETrackEvent.OnAdded:
                    {
                        switch (_linkRule)
                        {
                            case ELinkRule.TrackingSequence:
                                {
                                    if (this.trackable) break;
                                    if (trackEntity.trackables.TryGetByIndex(_trackingSequenceData.trackingIndex, out ARPlane plane)
                                        && plane == trackable)
                                    {
                                        //可关联
                                        Update(trackable);
                                        return trackable;
                                    }
                                    return false;
                                }
                            case ELinkRule.ValidMinSize:
                                {
                                    if (!_validMinSizeData.ValidPlane(trackable.size)) break;

                                    if (this.trackable)//已有关联
                                    {
                                        if (this.trackable.IsSubsumedBy(trackable)
                                            && trackEntity is PlaneTracker planeTracker)
                                        {
                                            //更新关联
                                            planeTracker.RecoverTransform();
                                            trackEntity.OnTrackUnlink();
                                            Update(trackable);
                                            return trackable;
                                        }
                                    }
                                    else
                                    {
                                        //可关联
                                        Update(trackable);
                                        return trackable;
                                    }

                                    break;
                                }
                        }
                        break;
                    }
                case ETrackEvent.OnUpdated:
                    {
                        switch (_linkRule)
                        {
                            case ELinkRule.ValidMinSize:
                                {
                                    if (!_validMinSizeData.ValidPlane(trackable.size)) break;
                                    if (!(trackEntity is PlaneTracker planeTracker)) break;

                                    if (this.trackable)//已有关联
                                    {
                                        if (this.trackable.IsSubsumedBy(trackable))
                                        {
                                            //更新关联
                                            planeTracker.RecoverTransform();
                                            trackEntity.OnTrackUnlink();
                                            trackEntity.OnTrackLink(trackable);
                                            planeTracker.RecordTransform(trackable.transform);
                                            Update(trackable);
                                            return trackable;
                                        }
                                    }
                                    else
                                    {
                                        //可关联
                                        trackEntity.OnTrackLink(trackable);
                                        planeTracker.RecordTransform(trackable.transform);
                                        Update(trackable);
                                        return trackable;
                                    }

                                    break;
                                }
                        }
                        break;
                    }
            }           
            return base.TryUpdate(trackable,trackEvent, trackEntity);
        }

#endif
    }
}
