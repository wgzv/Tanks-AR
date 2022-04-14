using System;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 旋转矩阵顺序:使用A、B、R分别代表第1，第2，第3元素旋转顺序及其对应元素的旋转角度；
    /// </summary>
    public enum ERotateMatrixType
	{
		/// <summary>
		/// 未知
		/// </summary>
		[Name("未知")]
		Unknow = -1,

		#region Proper Euler angles

		XZX,

		XYX,

		YXY,

		YZY,

		ZYZ,

		ZXZ,

		#endregion

		#region Tait-Bryan angles

		XZY,

		XYZ,

		/// <summary>
		/// Unity外旋顺序-世界旋转矩阵顺序;Unity中类型<see cref="Matrix4x4"/>使用本顺序构建旋转组件；
		/// </summary>
		YXZ,

		YZX,

		ZYX,

		/// <summary>
		/// Unity内旋顺序-本地旋转矩阵顺序；
		/// </summary>
		ZXY,

		#endregion
	}

	/// <summary>
	/// 旋转矩阵组手
	/// </summary>
	public static class RotateMatrixHelper
	{
		/// <summary>
		/// 默认坐标系
		/// </summary>
		public const ECoordinateSystem DefaultCoordinateSystem = ECoordinateSystem.XR_YU_ZF;

		/// <summary>
		/// 获取将源坐标系转为默认坐标系的旋转矩阵
		/// </summary>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Matrix4x4 GetRotateMatrixToDefault(this ECoordinateSystem srcCoordinateSystem)
		{
			switch (srcCoordinateSystem)
			{
				case ECoordinateSystem.XR_YU_ZF: return Matrix4x4.identity;
				case ECoordinateSystem.XL_YU_ZF: return Matrix4x4.Scale(new Vector3(-1, 1, 1));
				case ECoordinateSystem.XR_YD_ZF: return Matrix4x4.Scale(new Vector3(1, -1, 1));
				case ECoordinateSystem.XR_YU_ZB: return Matrix4x4.Scale(new Vector3(1, 1, -1));
				case ECoordinateSystem.XR_YF_ZU: return Matrix4x4.Scale(new Vector3(1, 1, -1)) * Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 0));
				case ECoordinateSystem.XL_YF_ZD: return Matrix4x4.Scale(new Vector3(-1, 1, 1)) * Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));
				default: throw new ArgumentException("无效坐标系" + srcCoordinateSystem.ToString(), nameof(srcCoordinateSystem));
			}
		}

		/// <summary>
		/// 将源坐标系的旋转矩阵转为默认坐标系的旋转矩阵
		/// </summary>
		/// <param name="srcRotateMatrix">源坐标系的旋转矩阵</param>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Matrix4x4 ToDefaultCoordinateSystem(this Matrix3x3 srcRotateMatrix, ECoordinateSystem srcCoordinateSystem) => ConvertRotateMatrixToDefault(srcRotateMatrix.ToMatrix4x4(), srcCoordinateSystem);

		/// <summary>
		/// 将指定坐标系的旋转矩阵转为默认坐标系的旋转矩阵
		/// </summary>
		/// <param name="srcRotateMatrix">源坐标系的旋转矩阵</param>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Matrix4x4 ConvertRotateMatrixToDefault(this Matrix4x4 srcRotateMatrix, ECoordinateSystem srcCoordinateSystem)
		{
			switch (srcCoordinateSystem)
			{
				case ECoordinateSystem.XR_YU_ZF: return Matrix4x4.identity;
				default:
                    {
						var r = srcCoordinateSystem.GetRotateMatrixToDefault();
						return r * srcRotateMatrix * r.inverse;
					}
			}
		}

		/// <summary>
		/// 获取将源坐标系的源位置点转为默认坐标系的位置点
		/// </summary>
		/// <param name="srcPosition">源位置点</param>
		/// <param name="srcCoordinateSystem">源坐标系</param>
		/// <returns></returns>
		public static Vector3 ConvertPositionToDefalut(this Vector3 srcPosition, ECoordinateSystem srcCoordinateSystem) => srcCoordinateSystem.GetRotateMatrixToDefault().MultiplyPoint3x4(srcPosition);
	}
}
