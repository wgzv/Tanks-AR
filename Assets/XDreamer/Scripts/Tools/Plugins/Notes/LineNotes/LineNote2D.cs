using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    public abstract class LineNote2D : LineNote
    {
        [Group("2D圆形")]
        [Name("圆半径")]
        [Tip("以注释目标对象为圆心，画出的线段长度")]
        [Range(0, float.MaxValue)]
        public float lineLength = 1;

        [Name("角度")]
        [Tip("以屏幕右侧为起始轴，逆时针转动的角度")]
        [Range(0, 360)]
        public float lineAngle = 90;

        public override Vector3 endPoint
        {
            get
            {
                // 使用相机朝向以及右向量构成的平面做运算
                Vector3 dir = Quaternion.AngleAxis(lineAngle, cam.transform.forward) * cam.transform.right;
                return beginPoint + dir.normalized * lineLength;
            }
        }
    }
}
