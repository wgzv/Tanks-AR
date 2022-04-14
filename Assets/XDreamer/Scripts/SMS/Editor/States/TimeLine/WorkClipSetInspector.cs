﻿using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.Maths;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.EditorSMS.States.TimeLine
{
    [CustomEditor(typeof(WorkClipSet))]
    public class WorkClipSetInspector : StateComponentInspector<WorkClipSet>
    {
        protected IWorkClip[] workClips => targetObject.GetComponents<IWorkClip>(true);

        public float totalTimeLength = -1;

        public bool lockTTL => lockWorkClip.IsLock(ELock.TotalTimeLength);

        public ELock lockWorkClip = ELock.None;

        public override void OnEnable()
        {
            base.OnEnable();
            totalTimeLength = -1;
            lockWorkClip = ELock.None;
        }

        public override void OnAfterVertical()
        {
            // 工作剪辑数据
            var clips = workClips.ToList();

            if (Event.current.type == EventType.Layout)
            {
                if (!lockTTL || totalTimeLength < 0)
                {
                    // 计算最大总时长
                    var ttl = WorkClipEditor.GetMaxTotalTimeLength(clips);
                    if (totalTimeLength < 0 || (ttl > 0 && !MathX.Approximately(ttl, totalTimeLength, WorkClip.Epsilon)))
                    {
                        totalTimeLength = ttl;
                    }
                }

                foreach (var clip in clips)
                {
                    if (clip is WorkClip workClip)
                    {
                        workClip.syncTL = false;
                        workClip.lockRatioOfWorkRange = true;
                        workClip.ttlOfLockRatio = totalTimeLength;
                    }
                }
            }

            if (totalTimeLength >= 0)
            {
                // 开启表格渲染
                WorkClipEditor.Draw(target, clips, ref lockWorkClip, ref totalTimeLength);
            }

            base.OnAfterVertical();
        }
    }
}

