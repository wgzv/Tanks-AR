using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 坐标系统枚举
    /// </summary>
    public enum ECoordinateSystem
	{
		/// <summary>
		/// 未知
		/// </summary>
		[Name("未知")]
		Unknow = -1,

		#region 左手坐标系

		/// <summary>
		/// X右Y上Z前:左手坐标系；Unity中默认的坐标系，也是本程序集中使用的默认坐标系；在本枚举定义范围内，提到默认坐标系时，均指本枚举对应的坐标系；
		/// </summary>
		[Name("X右Y上Z前")]
		[Tip("左手坐标系；Unity中默认的坐标系，也是本程序集中使用的默认坐标系；在本枚举定义范围内，提到默认坐标系时，均指本枚举对应的坐标系；")]
		XR_YU_ZF,

		#endregion

		#region 右手坐标系

		/// <summary>
		/// X做Y上Z前:右手坐标系；与默认坐标系X轴取反；
		/// </summary>
		[Name("X左Y上Z前")]
		[Tip("右手坐标系；与默认坐标系X轴取反；")]
		XL_YU_ZF = 100,

		/// <summary>
		/// X右Y下Z前:右手坐标系；与默认坐标系Y轴取反；
		/// </summary>
		[Name("X右Y下Z前")]
		[Tip("右手坐标系；与默认坐标系Y轴取反；")]
		XR_YD_ZF,

		/// <summary>
		/// X右Y上Z后:右手坐标系；与默认坐标系Z轴取反；教课书中多使用本坐标系做空间几何的讲解；
		/// </summary>
		[Name("X右Y上Z后")]
		[Tip("右手坐标系；与默认坐标系Z轴取反；教课书中多使用本坐标系做空间几何的讲解；")]
		XR_YU_ZB,

		/// <summary>
		/// X右Y前Z上:右手坐标系；
		/// </summary>
		[Name("X右Y前Z上")]
		[Tip("右手坐标系；")]
		XR_YF_ZU,

		/// <summary>
		/// X左Y前Z下:右手坐标系；
		/// </summary>
		[Name("X左Y前Z下")]
		[Tip("右手坐标系；")]
		XL_YF_ZD,

		#endregion
	}
}
