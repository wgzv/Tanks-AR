using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.XAssets.Manuals;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets
{
    /// <summary>
    /// XDreamer资产商店
    /// </summary>
    public class XAssetStore
    {
        #region 基础定义

        /// <summary>
        /// 扩展名 html
        /// </summary>
        public const string Ext = ManualHelper.Ext;

        /// <summary>
        /// .扩展名 .html
        /// </summary>
        public const string DotExt = ManualHelper.DotExt;

        /// <summary>
        /// 索引文件 index.html
        /// </summary>
        public const string IndexFile = ManualHelper.IndexFile;

        /// <summary>
        /// 错误文件 error.html
        /// </summary>
        public const string ErrorFile = ManualHelper.ErrorFile;

        #endregion

        #region 离线

        /// <summary>
        /// XDreamer资产的全路径;本地磁盘全路径；
        /// </summary>
        public static string XAssets => UICommonFun.GetFullPath(EFolder._Assets);

        /// <summary>
        /// 离线主页的全路径;本地磁盘全路径；
        /// </summary>
        public static string MainPage => XAssets + "/offline" + DotExt;

        /// <summary>
        /// 离线手册主页的全路径;本地磁盘全路径；
        /// </summary>
        public static string ManualPage => ManualHelper.ManualPage;

        /// <summary>
        /// 离线错误页面的全路径本地磁盘全路径；
        /// </summary>
        public static string ErrorPage = XAssets + "/" + ErrorFile;

        #endregion

        #region 在线

        /// <summary>
        /// 在线主页URL
        /// </summary>
        public static string MainPageOnline => Product.URL + "/" + IndexFile;

        /// <summary>
        /// 在线产品页面URL
        /// </summary>
        public static string ProductPageOnline => Product.URL;

        /// <summary>
        /// 在线手册页面URL
        /// </summary>
        public static string ManualPageOnline => Product.URL + "/" + nameof(ManualHelper.Manual) + "/" + IndexFile;

        #endregion
    }
}
