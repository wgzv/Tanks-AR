using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.Maths;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorSMS.States.TimeLine
{
    public class WorkClipRecorder
    {
        public float beginTime = 0;
        public float endTime = 0;
        public float timeLength = 0;

        public float beginPercent = 0;
        public float endPercent = 0;
        public float percentLength => endPercent - beginPercent;

        public float totalTimeLength { get; private set; }

        public UnityEngine.Object obj { get; private set; }

        public WorkClipRecorder(UnityEngine.Object obj) { this.obj = obj; }

        public WorkClipRecorder(UnityEngine.Object obj, IWorkClip workClip, float totalTimeLength)
        {
            this.obj = obj;
            Record(workClip, totalTimeLength);
        }

        public void Record(IWorkClip workClip, float totalTimeLength)
        {
            beginTime = workClip.beginTime;
            endTime = workClip.endTime;
            timeLength = workClip.timeLength;

            beginPercent = workClip.beginPercent;
            endPercent = workClip.endPercent;
            //percentLength = workClip.percentLength;

            this.totalTimeLength = totalTimeLength;
        }

        public void Recover(IWorkClip workClip)
        {
            UndoHelper.RegisterCompleteObjectUndo(obj);

            workClip.endPercent = Mathf.Clamp(endPercent, 0, 1);
            workClip.beginPercent = Mathf.Clamp(beginPercent, 0, endPercent);
            //workClip.percentLength = workClip.endPercent - workClip.beginPercent;

            workClip.beginTime = beginTime;
            workClip.endTime = endTime;
            //workClip.timeLength = workClip.endTime - workClip.beginTime;
        }

        public void KeepPercent()
        {
            beginTime = totalTimeLength * beginPercent;
            endTime = totalTimeLength * endPercent;
        }

        public void KeepBeginTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = beginTime / totalTimeLength;
            endTime = totalTimeLength * endPercent;
        }

        public void KeepEndTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endPercent = endTime / totalTimeLength;
            beginTime = totalTimeLength * beginPercent;            
        }

        public void KeepTimeLengthAndBeginPercent()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = totalTimeLength * beginPercent;
            endTime = beginTime + timeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void KeepTimeLengthAndEndPercent()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = totalTimeLength * endPercent;
            beginTime = endTime - timeLength;
            beginPercent = beginTime / totalTimeLength;
        }

        public void KeepTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = beginTime / totalTimeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void SetBeginTime(float newBeginTime)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = newBeginTime;
            beginPercent = beginTime / totalTimeLength;
        }

        public void SetBeginPercent(float newBeginPercent)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginPercent = newBeginPercent;
            beginTime = beginPercent * totalTimeLength;
        }

        public void KeepTimeLengthOnBeginTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + timeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void SetEndPercent(float newEndnPercent)
        {
            endPercent = newEndnPercent;
            endTime = endPercent * totalTimeLength;
        }

        public void KeepTimeLengthOnEndTime()
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = endTime - timeLength;
            beginPercent = beginTime / totalTimeLength;
        }

        public void SetEndTime(float newEndTime)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = newEndTime;
            endPercent = endTime / totalTimeLength;
        }

        public void SetTimeLength(float newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + newTimeLength;

            if (endTime > totalTimeLength)
            {
                float offset = endTime - totalTimeLength;
                endTime = totalTimeLength;
                beginTime = Mathf.Clamp(beginTime - offset, 0, endTime);
            }

            beginPercent = beginTime / totalTimeLength;
            endPercent = endTime / totalTimeLength;
        }

        public void OnTimeLengthChangeFixBeginTime(float newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            endTime = beginTime + newTimeLength;
            endTime = Mathf.Clamp(endTime, beginTime, totalTimeLength);

            endPercent = endTime / totalTimeLength;
        }

        public void OnTimeLengthChangeFixEndTime(float newTimeLength)
        {
            if (MathX.ApproximatelyZero(totalTimeLength)) return;
            beginTime = endTime - newTimeLength;
            beginTime = Mathf.Clamp(beginTime, 0, endTime);

            beginPercent = beginTime / totalTimeLength;
        }
    }
}
