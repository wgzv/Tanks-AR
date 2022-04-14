﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorSMS.States.Base;
using XCSJ.EditorSMS.Utils;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.EditorSMS.States.TimeLine
{
    [CustomEditor(typeof(StateWorkClipSet), true)]
    public class StateWorkClipSetInspector : WorkClipInspector<StateWorkClipSet>
    {
        [Name("排序规则")]
        [Tip("对状态工作剪辑集合内的元素进行排序")]
        public enum ESortRule
        {
            [Name("默认")]
            [Tip("按照\"状态机连线顺序\"功能生成的顺序队列,对当前列表进行排序;")]
            Default,

            [Name("名称")]
            Name,

            [Name("开始时间")]
            BeginTime,

            [Name("结束时间")]
            EndTime,

            [Name("时长")]
            TimeLength,

            [Name("单次时长")]
            OnceTimeLength,

            [Name("速度")]
            Speed,

            [Name("次数")]
            LoopCount,

            [Name("逆序")]
            Reverse,
        }

        protected StateWorkClipSet stateWorkClipSet => target as StateWorkClipSet;

        protected ELock lockWorkClip = ELock.None;

        private UnityEditorInternal.ReorderableList _stateWorkRangeInfosList;

        private UnityEditorInternal.ReorderableList stateWorkRangeInfosList
        {
            get
            {
                if (_stateWorkRangeInfosList == null)
                {
                    _stateWorkRangeInfosList = new UnityEditorInternal.ReorderableList(serializedObject,
                        serializedObject.FindProperty(nameof(StateWorkClipSet.stateWorkClips)), true, false, false, false);
                    _stateWorkRangeInfosList.headerHeight = 2;
                    _stateWorkRangeInfosList.footerHeight = 0;
                    //stateWorkRangeInfosList.elementHeight *= 2;
                    _stateWorkRangeInfosList.drawElementCallback += DrawWorkRangeInfoElement;
                    _stateWorkRangeInfosList.onSelectCallback += OnSelectedElement;
                }

                return _stateWorkRangeInfosList;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onAnyAssetsOrOptionChanged += StateOTLCache.Cache.Clear;
            StateOTLCache.Cache.Clear();
        }

        public override void OnDisable()
        {
            XDreamerEvents.onAnyAssetsOrOptionChanged -= StateOTLCache.Cache.Clear;
            base.OnDisable();
        }

        public override bool OnNeedHandleGroup(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(WorkClip.useInitData):
                case nameof(WorkClip.loopType):
                case nameof(WorkClip.workCurve):
                    {
                        return false;
                    }
            }
            return base.OnNeedHandleGroup(type, memberProperty, arrayElementInfo);
        }

        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(WorkClip.workRange):
                case nameof(WorkClip.useInitData):
                case nameof(WorkClip.setPercentOnEntry):
                case nameof(WorkClip.percentOnEntry):
                case nameof(WorkClip.setPercentOnExit):
                case nameof(WorkClip.percentOnExit):
                case nameof(WorkClip.loopType):
                case nameof(WorkClip.workCurve):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterVertical()
        {
            //if (GUILayout.Button("生成路径"))
            {
                // 默认状态连接链表
                stateWorkClipSet.UpdateData();
                EditorGUI.BeginDisabledGroup(stateWorkClipSet.SMSequence);
                EditorGUI.BeginChangeCheck();
                {
                    DrawWorkRangeInfoHeader();
                    stateWorkRangeInfosList.DoLayoutList();
                    DrawSortRule(stateWorkRangeInfosList.serializedProperty);
                }
                if(EditorGUI.EndChangeCheck())
                {
                    StateOTLCache.Cache.Clear();
                }
                EditorGUI.EndDisabledGroup();
            }
            base.OnAfterVertical();
        }

        public override bool OnModifiedPropertyField(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(StateWorkClipSet.SMSequence):
                    {
                        if (memberProperty.boolValue)
                        {
                            UndoHelper.RegisterCompleteObjectUndo(stateWorkClipSet);

                            //return EditorUtility.DisplayDialog("是否应用修改", "已编辑状态工作剪辑集合的内容(包括顺序、时长、速度等信息)将被修改。", "确定", "取消");//本次修改操作不可撤销！
                        }
                        break;
                    }
                default:
                    break;
            }
            return base.OnModifiedPropertyField(type, memberProperty, arrayElementInfo);
        }

        protected void DrawSortRule(SerializedProperty serializedProperty)
        {
            UICommonFun.EnumButton<ESortRule>(sr => Sort(sr, serializedProperty));
        }

        protected void Sort(ESortRule sortRule, SerializedProperty serializedProperty)
        {
            switch (sortRule)
            {
                case ESortRule.Default:
                    {
                        var s = WorkClipHelper.CreateStateWorkClips(stateWorkClipSet.stateCollection.entryState as State);
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return s.FindIndex(c => c.state == aSWC.state).CompareTo(s.FindIndex(c => c.state == bSWC.state));
                        });
                        break;
                    }
                case ESortRule.Name:
                    {
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return aSWC.name.CompareTo(bSWC.name);
                        });
                        break;
                    }
                case ESortRule.BeginTime:
                    {
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return aSWC.beginTime.CompareTo(bSWC.beginTime);
                        });
                        break;
                    }
                case ESortRule.EndTime:
                    {
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return aSWC.endTime.CompareTo(bSWC.endTime);
                        });
                        break;
                    }
                case ESortRule.TimeLength:
                    {
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return aSWC.timeLength.CompareTo(bSWC.timeLength);
                        });
                        break;
                    }
                case ESortRule.OnceTimeLength:
                    {
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return aSWC.onceTimeLength.CompareTo(bSWC.onceTimeLength);
                        });
                        break;
                    }
                case ESortRule.Speed:
                    {
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return aSWC.speed.CompareTo(bSWC.speed);
                        });
                        break;
                    }
                case ESortRule.LoopCount:
                    {
                        SerializedObjectHelper.ArrayElementSort(serializedProperty, (a, b) =>
                        {
                            var aSWC = stateWorkClipSet.stateWorkClips[a.index];
                            var bSWC = stateWorkClipSet.stateWorkClips[b.index];
                            return aSWC.loopCount.CompareTo(bSWC.loopCount);
                        });
                        break;
                    }
                case ESortRule.Reverse:
                    {
                        SerializedObjectHelper.ArrayElementReverse(serializedProperty);
                        break;
                    }
            }
        }

        [Flags]
        public enum ELcokExtension
        {
            [Name("无")]
            [EnumFieldName("无")]
            None,

            [Name("速度")]
            [Tip("当前状态在状态工作剪辑集合时间轴中的速度;修改速度,仅影响次数;")]
            [EnumFieldName("速度")]
            Speed = 1 << 0,

            [Name("次数")]
            [Tip("次数=时长*速度/单次时长;即在指定时长内循环的次数;修改次数,仅影响速度;")]
            [EnumFieldName("次数")]
            LoopCount = 1 << 1,
        }

        [EnumPopup]
        public ELcokExtension lcokExtension = ELcokExtension.None;

        private bool IsLock(ELcokExtension lockCheck) => (lcokExtension & lockCheck) == lockCheck;
        private void UnLock(ELcokExtension lockCheck) => lcokExtension &= ~lockCheck;
        private void SwitchLocks(ELcokExtension lockCheck)
        {
            lcokExtension = lcokExtension ^ lockCheck;
            if (IsLock(lockCheck))
            {
                switch (lockCheck)
                {
                    case ELcokExtension.Speed:
                        {
                            UnLock(ELcokExtension.LoopCount);
                            break;
                        }
                    case ELcokExtension.LoopCount:
                        {
                            UnLock(ELcokExtension.Speed);
                            break;
                        }
                }
            }
        }

        private void DrawTitleButton(ELcokExtension lockCheck, float width)
        {
            var content = CommonFun.NameTooltip(lockCheck);
            content.image = IsLock(lockCheck) ? WorkClipEditor.lockIcon : WorkClipEditor.unlockIcon;
            if (GUILayout.Button(content, WorkClipEditor.helpBoxStyle, GUILayout.Width(width), GUILayout.Height(20)))
            {
                SwitchLocks(lockCheck);
            }
        }

        #region 时长GUI内容

        [Name("单次时长=>时长")]
        [Tip("将“单次时长”设置为“时长”的对应数值")]
        [XCSJ.Attributes.Icon(EIcon.ArrowLeft)]
        private XGUIContent OTL_TTL_Content = new XGUIContent(typeof(StateWorkClipSetInspector), nameof(OTL_TTL_Content), true);

        [Name("时长=>单次时长")]
        [Tip("将“时长”设置为“单次时长”的对应数值")]
        [XCSJ.Attributes.Icon(EIcon.ArrowRight)]
        private XGUIContent TTL_OTL_Content = new XGUIContent(typeof(StateWorkClipSetInspector), nameof(TTL_OTL_Content), true); 
        
        #endregion

        protected void DrawWorkRangeInfoHeader()
        {
            float totalTimeLength = stateWorkClipSet.timeLength;

            if (MathX.ApproximatelyZero(totalTimeLength))
            {
                var bk = GUI.backgroundColor;
                GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                WorkClipEditor.DrawTotalTimeLength(target, stateWorkClipSet.stateWorkClips.ToList(s => s as IWorkClip), ref lockWorkClip, ref totalTimeLength);
                GUI.backgroundColor = bk;
            }
            else
            {
                WorkClipEditor.DrawTotalTimeLength(target, stateWorkClipSet.stateWorkClips.ToList(s => s as IWorkClip), ref lockWorkClip, ref totalTimeLength);
            }

            var option = SMSExtensionOption.weakInstance.workClipEditorOption;

            EditorGUILayout.BeginHorizontal(WorkClipEditor.boxStyle);
            GUILayout.Space(15);
            WorkClipEditor.DrawTitle(stateWorkClipSet.stateWorkClips.ToList(s => s as IWorkClip), ref lockWorkClip, ref totalTimeLength, () =>
            {
                AfterHorizontalSpaceWidth = 0;
                if (option.OTL)
                {
                    if (option.timeLength)
                    {
                        AfterHorizontalSpaceWidth += btnWidth;
                        //时长 --> 单次时长
                        if (GUILayout.Button(TTL_OTL_Content, EditorStyles.miniButtonLeft, GUILayout.Width(btnWidth), GUILayout.Height(20)))
                        {
                            //if (EditorUtility.DisplayDialog("是否应用修改", "将“单次时长”设置为“时长”的对应数值？\n本次修改操作不可撤销！", "确定", "取消"))
                            {
                                stateWorkClipSet.stateWorkClips.ForEach(clip =>
                                {
                                    UndoHelper.RegisterCompleteObjectUndo(clip.state.components);
                                    clip.state.components.Foreach(c =>
                                    {
                                        if (c is WorkClip workClip)
                                        {
                                            workClip.onceTimeLength = clip.timeLength;
                                        }
                                    });
                                });
                            }
                        }
                        AfterHorizontalSpaceWidth += btnWidth;
                        //时长 <-- 单次时长
                        if (GUILayout.Button(OTL_TTL_Content, EditorStyles.miniButtonRight, GUILayout.Width(btnWidth), GUILayout.Height(20)))
                        {
                            //if (EditorUtility.DisplayDialog("是否应用修改", "将“时长”设置为“单次时长”的对应数值？\n本次修改操作不可撤销！", "确定", "取消"))
                            {
                                UndoHelper.RegisterCompleteObjectUndo(stateWorkClipSet);
                                stateWorkClipSet.stateWorkClips.ForEach(clip => clip.timeLength = clip.onceTimeLength);
                            }
                        }
                        AfterHorizontalSpaceWidth += 4;
                    }

                    AfterHorizontalSpaceWidth += (WorkClipEditor.TitleWidth + 4);
                    GUILayout.Label("单次时长", WorkClipEditor.helpBoxStyle, GUILayout.Width(WorkClipEditor.TitleWidth), GUILayout.Height(20));
                }

                if (option.speed)
                {
                    AfterHorizontalSpaceWidth += (WorkClipEditor.TitleWidth + 4);
                    GUILayout.Label(CommonFun.NameTooltip(ELcokExtension.Speed), WorkClipEditor.helpBoxStyle, GUILayout.Width(WorkClipEditor.TitleWidth), GUILayout.Height(20));
                }
                //DrawTitleButton(ELcokExtension.Speed, WorkClipEditor.TitleWidth);

                if (option.loopCount)
                {
                    AfterHorizontalSpaceWidth += (WorkClipEditor.TitleWidth - 10 + 4);
                    DrawTitleButton(ELcokExtension.LoopCount, WorkClipEditor.TitleWidth - 10);
                }

                AfterHorizontalSpaceWidth += btnWidth + 4;
                // 添加片段               
                if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, GUILayout.Width(btnWidth), GUILayout.Height(20)))
                {
                    AddStateWorkClip();
                }
            });
            EditorGUILayout.EndHorizontal();

            stateWorkClipSet.timeLength = totalTimeLength;
        }

        private float btnWidth = 20;
        private float AfterHorizontalSpaceWidth = 0;

        protected void DrawWorkRangeInfoElement(Rect rect, int index, bool selected, bool focused)
        {
            if (index < 0 || index >= stateWorkClipSet.stateWorkClips.Count) return;

            Color color = index % 2 == 1 ? WorkClipEditor.rowColor : GUI.backgroundColor;
            CommonFun.DrawColorGUI(color, () =>
            {
                var option = SMSExtensionOption.weakInstance.workClipEditorOption;
                // 状态工作剪辑
                var stateWorkClip = stateWorkClipSet.stateWorkClips[index];
                var tmpLoopCount = stateWorkClip.loopCount;
                if (WorkClipEditor.Draw(target, rect, index, stateWorkClip, lockWorkClip, stateWorkClipSet.timeLength,
                    AfterHorizontalSpaceWidth, (inRect) =>
                    {
                        if (option.OTL)
                        {
                            if (option.timeLength)
                            {
                                inRect.width = 20;
                                //时长 --> 单次时长
                                if (GUI.Button(inRect, TTL_OTL_Content, EditorStyles.miniButtonLeft))
                                {
                                    UndoHelper.RegisterCompleteObjectUndo(stateWorkClip.state.components);
                                    stateWorkClip.state.components.Foreach(c =>
                                    {
                                        if (c is WorkClip workClip)
                                        {
                                            workClip.onceTimeLength = stateWorkClip.timeLength;
                                        }
                                    });
                                }
                                inRect.x += inRect.width;

                                //时长 <-- 单次时长
                                if (GUI.Button(inRect, OTL_TTL_Content, EditorStyles.miniButtonRight))
                                {
                                    UndoHelper.RegisterCompleteObjectUndo(stateWorkClipSet);
                                    stateWorkClip.SetTimeLength(stateWorkClip.onceTimeLength, stateWorkClipSet.timeLength);
                                }
                                inRect.x += inRect.width + 4;
                            }

                            inRect.width = WorkClipEditor.TitleWidth;

                            EditorGUI.LabelField(inRect, CommonFun.TempContent(stateWorkClip.onceTimeLength.ToString(), StateOTLCache.GetCacheValue(stateWorkClip)));
                            inRect.x += inRect.width + 4;
                        }

                        if (option.speed)
                        {
                            inRect.width = WorkClipEditor.TitleWidth;

                            EditorGUI.BeginDisabledGroup(IsLock(ELcokExtension.Speed));
                            EditorGUI.BeginChangeCheck();
                            var speed = EditorGUI.DelayedFloatField(inRect, stateWorkClip.speed);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoHelper.RegisterCompleteObjectUndo(stateWorkClipSet);
                                stateWorkClip.speed = speed;
                            }
                            EditorGUI.EndDisabledGroup();

                            inRect.x += inRect.width + 4;
                        }

                        if (option.loopCount)
                        {
                            inRect.width = WorkClipEditor.TitleWidth - 10;
                            var loopCount = stateWorkClip.loopCount;
                            var lockLoopCount = IsLock(ELcokExtension.LoopCount);
                            EditorGUI.BeginDisabledGroup(lockLoopCount);
                            EditorGUI.BeginChangeCheck();
                            loopCount = EditorGUI.DelayedFloatField(inRect, loopCount);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoHelper.RegisterCompleteObjectUndo(stateWorkClipSet);
                                stateWorkClip.loopCount = loopCount;
                            }
                            else if (lockLoopCount)
                            {
                                stateWorkClip.loopCount = tmpLoopCount;
                            }
                            EditorGUI.EndDisabledGroup();

                            inRect.x += inRect.width + 4;
                        }

                        inRect.width = btnWidth;
                        return GUI.Button(inRect, "-", EditorStyles.miniButtonRight);
                    }))
                {
                    UICommonFun.DeleteArrayElementAtIndex(serializedObject.FindProperty(nameof(StateWorkClipSet.stateWorkClips)), index);
                }
            });
        }

        protected void OnSelectedElement(UnityEditorInternal.ReorderableList list)
        {
            var doubleClick = Event.current.clickCount > 1;
            EditorSMSHelper.PingObject(stateWorkClipSet.stateWorkClips[list.index].state, doubleClick, doubleClick);
        }

        protected void AddStateWorkClip()
        {
            // 获取当前状态机中所有可播放的工作剪辑
            var workClipsInSM = stateWorkClipSet.stateCollection.GetStateComponents(ESearchFlags.Children | ESearchFlags.Active | ESearchFlags.Inactive | ESearchFlags.Enable | ESearchFlags.Disable, typeof(IWorkClip)).Cast(sc => sc.parent).Distinct().ToList();

            // 如果该工作剪辑不可重复添加，删除已经添加的工作剪辑
            for (int i = workClipsInSM.Count - 1; i >= 0; --i)
            {
                if (stateWorkClipSet.stateWorkClips.Exists(info => info.state == workClipsInSM[i]))
                {
                    //workClipsInSM.Remove(workClipsInSM[i]);
                    workClipsInSM.RemoveAt(i);
                }
            }

            if (workClipsInSM.Count>0)
            {
                MenuHelper.DrawMenu(typeof(StateWorkClipSet).Name, m =>
                {
                    UndoHelper.RegisterCompleteObjectUndo(stateWorkClipSet);
                    foreach (var state in workClipsInSM)
                    {
                        m.AddMenuItem(state.name, (s) =>
                        {
                            stateWorkClipSet.stateWorkClips.Add(new StateWorkClip(s as State));
                        }, state);
                    }
                });
            }
            else
            {
                Debug.LogWarning("所有工作剪辑已添加到集合中！");
            }
        }

        private class StateOTLCache : TICache<StateOTLCache, StateWorkClip, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(StateWorkClip key1)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("单次时长(速度):\t{0}", key1.onceTimeLengthWithSpeed.ToString());
                sb.Append("\n组件名称\t\t时长\t单次时长\t循环次数");
                key1.state.ForEachComponents(sc =>
                {
                    if(sc is WorkClip workClip)
                    {
                        sb.AppendFormat("\n{0}\t{1}{2}\t{3}\t{4}", workClip.name, workClip.name.Length >= 5 ? "" : "\t", workClip.timeLength, workClip.onceTimeLength, workClip.loopCount);
                    }
                });
                return new KeyValuePair<bool, string>(true, sb.ToString());
            }
        }
    }
}
