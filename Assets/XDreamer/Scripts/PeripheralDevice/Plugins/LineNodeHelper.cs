using UnityEngine;

namespace XCSJ.PluginPeripheralDevice
{
    public class LineNodeHelper
    {
        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planeNormal">垂直于平面的的向量</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns></returns>
        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float dis = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);

            return dis * direct.normalized + point;
        }
    }
}
