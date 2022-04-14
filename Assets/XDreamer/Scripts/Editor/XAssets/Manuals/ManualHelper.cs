using System;
using System.IO;
using System.Reflection;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Manuals
{
    /// <summary>
    /// 手册组手类
    /// </summary>
    public static class ManualHelper
    {
        #region 基础定义

        /// <summary>
        /// 扩展名 html
        /// </summary>
        public const string Ext = "html";

        /// <summary>
        /// .扩展名 .html
        /// </summary>
        public const string DotExt = "." + Ext;

        /// <summary>
        /// 索引文件 index.html
        /// </summary>
        public const string IndexFile = "index" + DotExt;

        /// <summary>
        /// API文件 api.html
        /// </summary>
        public const string ApiFile = "api" + DotExt;

        /// <summary>
        /// 错误文件 error.html
        /// </summary>
        public const string ErrorFile = "error" + DotExt;

        /// <summary>
        /// URL参数
        /// </summary>
        public const string UrlParam = "?id=";

        /// <summary>
        /// 数据名称；手册生成器动态生成后存储的文件夹名称；
        /// </summary>
        public const string DataName = "详细参考";

        /// <summary>
        /// API数据；API文件关联的数据文件存储使用的文件夹名称；
        /// </summary>
        public const string ApiData = "api_data";

        /// <summary>
        /// 索引数据；索引文件关联的数据文件存储使用的文件夹名称；
        /// </summary>
        public const string IndexData = "index_data";

        /// <summary>
        /// 索引数据-图片；索引文件关联的图片文件存储使用的文件夹路径；
        /// </summary>
        public const string IndexData_Images = IndexData + "/images";

        /// <summary>
        /// 数据JS文件相对手册目录的路径
        /// </summary>
        public const string DataJSInManual = "/index_data/js/data.js";

        /// <summary>
        /// 数据手册JS文件相对手册目录的路径
        /// </summary>
        public const string DataManualJSInManual = "/index_data/js/data.manual.js";

        #endregion

        #region 全路径信息

        /// <summary>
        /// 数据JS文件的全路径;本地磁盘全路径；
        /// </summary>
        public static string DataJS => Manual + DataJSInManual;

        /// <summary>
        /// 数据手册JS文件的全路径;本地磁盘全路径；
        /// </summary>
        public static string DataManualJS => Manual + DataManualJSInManual;

        /// <summary>
        /// XDreamer手册的全路径;本地磁盘全路径；
        /// </summary>
        public static string Manual => UICommonFun.ToFullPath(XAssetStoreOption.weakInstance.offlineManualDirectory);

        /// <summary>
        /// HTML模版的全路径;本地磁盘全路径；
        /// </summary>
        public static string HtmlTemplates => Manual + "/" + nameof(HtmlTemplates);

        /// <summary>
        /// XDreamer手册数据的全路径;本地磁盘全路径；
        /// </summary>
        public static string Data => Manual + "/" + DataName;

        /// <summary>
        /// 离线手册主页的全路径;本地磁盘全路径；
        /// </summary>
        public static string ManualPage => Manual + "/" + IndexFile;

        /// <summary>
        /// 离线手册错误主页的全路径本地磁盘全路径；
        /// </summary>
        public static string ErrorPageOfManual = Manual + "/" + ErrorFile;

        /// <summary>
        /// 判断手册目录是否存在
        /// </summary>
        /// <returns></returns>
        public static bool ManualDirectoryExists() => Directory.Exists(Manual);

        #endregion

        #region 成员信息（包括类型、函数、字段、属性）与手册页面

        private static string ToFullPathOfManual(string path) => Manual + "/" + path;

        /// <summary>
        /// 获取成员信息对应的XDreamer手册页面路径;路径是相对于Assets文件的路径；
        /// </summary>
        /// <param name="member"></param>
        /// <param name="indexOrApi"></param>
        /// <returns></returns>
        public static string GetPathOfManualInAssets(MemberInfo member, bool indexOrApi = true)
        {
            if (member == null) return ErrorFile;
            var path = GetPage(member, indexOrApi);
            var fullPath = ToFullPathOfManual(path);
            if (FileHelper.Exists(fullPath)) return path;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                case MemberTypes.Method:
                    {
                        return GetPage(member.ReflectedType);
                    }
            }
            return path;
        }

        /// <summary>
        /// 获取基于ManualInAssets的有效相对路径
        /// </summary>
        /// <param name="member"></param>
        /// <param name="indexOrApi"></param>
        /// <returns></returns>
        public static string GetValidPathOfManualInAssets(MemberInfo member, bool indexOrApi = true)
        {
            if (member == null) return ErrorFile;
            var path = GetPage(member, indexOrApi);
            var fullPath = ToFullPathOfManual(path);
            if (FileHelper.Exists(fullPath)) return path;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                case MemberTypes.Method:
                    {
                        return GetPage(member.DeclaringType);
                    }
            }
            return path;
        }

        /// <summary>
        /// 获取成员信息对应的XDreamer手册页面全路径
        /// </summary>
        /// <param name="member"></param>
        /// <param name="indexOrApi"></param>
        /// <returns></returns>
        public static string GetFullPathOfManual(MemberInfo member, bool indexOrApi = true)
        {
            if (member == null) return ErrorPageOfManual;
            return ToFullPathOfManual(GetPage(member, indexOrApi));
        }

        /// <summary>
        /// 全名称转名称路径；例如：XCSJ.EditorExtension.XAssets.Manuals -> XCSJ/EditorExtension/XAssets/Manuals
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string ToNamePath(string fullName) => fullName.Replace('.', '/');

        /// <summary>
        /// 名称路径转全名称；例如：XCSJ/EditorExtension/XAssets/Manuals -> XCSJ.EditorExtension.XAssets.Manuals
        /// </summary>
        /// <param name="namePath"></param>
        /// <returns></returns>
        public static string ToFullName(string namePath) => namePath.Replace('/', '.');

        /// <summary>
        /// 获取类型的名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetName(Type type)
        {
            if (type.IsGenericType)
            {

            }
            return type.Name;
        }

        /// <summary>
        /// 获取类型的全名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFullName(Type type)
        {
            if (type == null) return "";
            if (type.IsNested)
            {
                return GetFullName(type.DeclaringType) + "." + GetName(type); ;
            }
            return type.Namespace + "." + GetName(type);
        }

        /// <summary>
        /// 获取类型关联的XDreamer手册页面目录名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPageDirName(Type type) => GetName(type);

        /// <summary>
        /// 根据全名称获取XDreamer手册页面目录
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string GetPageDir(string fullName) => string.IsNullOrEmpty(fullName) ? "" : (ToNamePath(fullName) + "/");

        /// <summary>
        /// 根据类型获取XDreamer手册页面目录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPageDir(Type type) => type == null ? "" : (DataName + "/" + ToNamePath(GetFullName(type)) + "/");

        /// <summary>
        /// 根据成员信息获取XDreamer手册页面目录
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetPageDir(MemberInfo member)
        {
            if (member == null) return "";
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                case MemberTypes.Method:
                    {
                        return GetPageDir(member.ReflectedType) + ApiData + "/";
                    }
                default:
                    {
                        if (member is Type type)
                        {
                            return GetPageDir(type);
                        }
                        break;
                    }
            }
            return "";
        }

        /// <summary>
        /// 根据全名称获取XDreamer手册页面路径
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="indexOrApi"></param>
        /// <returns></returns>
        public static string GetPage(string fullName, bool indexOrApi = true)
        {
            return GetPageDir(fullName) + (indexOrApi ? IndexFile : ApiFile);
        }

        /// <summary>
        /// 根据成员信息获取XDreamer手册页面路径
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetPage(MemberInfo member, bool indexOrApi = true)
        {
            if (member == null) return ErrorFile;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                case MemberTypes.Method:
                    {
                        return GetPageDir(member) + member.Name + DotExt;
                    }
                default:
                    {
                        if (member is Type type)
                        {
                            return GetPageDir(type) + (indexOrApi ? IndexFile : ApiFile);
                        }
                        break;
                    }
            }
            return ErrorFile;
        }

        #endregion

        #region 手册URL处理

        /// <summary>
        /// 将主URL与URL参数合并成页面URL
        /// </summary>
        /// <param name="mainUrl"></param>
        /// <param name="urlParam"></param>
        /// <returns></returns>
        public static string ToPageUrl(string mainUrl, string urlParam)
        {
            return mainUrl + UrlParam + urlParam;
        }

        /// <summary>
        /// 获取URL中的主页面URL；使用UrlParam做字符串拆分，并返回UrlParam之前的字符串；
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetMainPageUrl(string url)
        {
            var index = url.IndexOf(UrlParam);
            return index >= 0 ? url.Substring(0, index) : url;
        }

        /// <summary>
        /// 获取URL中的URL参数；使用UrlParam做字符串拆分，并返回UrlParam之后的字符串；
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlParam(string url)
        {
            var index = url.IndexOf(UrlParam);
            if (index >= 0)
            {
                return url.Substring(index + UrlParam.Length);
            }
            return "";
        }

        /// <summary>
        /// 获取URL中的子页面URL；使用UrlParam做字符串拆分后，对主页面URL与URL参数做去重合并，返回URL参数中指向的子页面URL；
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetSubPageUrl(string url)
        {
            var sub = GetUrlParam(url);
            return string.IsNullOrEmpty(sub) ? "" : PathHelper.Format(Path.Combine(Path.GetDirectoryName(GetMainPageUrl(url)), sub));
        }

        #endregion

        private const string BeginFormat = "<!-- {0} 开始-->";
        private const string EndFormat = "<!-- {1} 结束-->";

        public static string Begin(string name) => string.Format(BeginFormat, name);
        public static string End(string name) => string.Format(EndFormat, name);

        public static string Replace(string src, string name, string newValue) => ReplaceAll(src, Begin(name), End(name), newValue);

        /// <summary>
        /// 将源字符串src中，将从第一个begin起始字符串开始之后到第一个end结束字符串结束之前的字符串替换为newValue新字符串；返回结果保留begin起始字符串与end结束字符串；会重复调用将所有满足条件的全部替换；
        /// 如果有任意参数为空字符串或null，抛出ArgumentNullException异常；
        /// 如果begin与end相同，抛出ArgumentException异常；
        /// 如果begin与end有重复或交叉，则异常InvalidDataException异常；
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="begin">起始字符串</param>
        /// <param name="end">结束字符串</param>
        /// <param name="newValue">新字符串</param>
        /// <returns></returns>
        public static string ReplaceAll(string src, string begin, string end, string newValue)
        {
            if (string.IsNullOrEmpty(src)) throw new ArgumentNullException(nameof(src));
            if (string.IsNullOrEmpty(begin)) throw new ArgumentNullException(nameof(begin));
            if (string.IsNullOrEmpty(end)) throw new ArgumentNullException(nameof(end));
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));

            if (begin == end) throw new ArgumentException("参数[" + nameof(newValue) + "]与参数[" + nameof(newValue) + "]的字符串不可相同！");

            var dst = src;
            while (true)
            {
                var beginIndex = dst.IndexOf(begin);
                if (beginIndex < 0) break;

                var endIndex = dst.IndexOf(end);
                if (endIndex < 0) break;

                if (beginIndex >= endIndex || endIndex < beginIndex + begin.Length) throw new InvalidDataException("起始字符串与结束字符串不可有重复或交叉字符串！");

                dst = ReplaceInternal(dst, beginIndex + begin.Length, endIndex, newValue);
            }
            return dst;
        }

        /// <summary>
        /// 将源字符串src中，将从第一个begin起始字符串开始之后到第一个end结束字符串结束之前的字符串替换为newValue新字符串；返回结果保留begin起始字符串与end结束字符串；
        /// 如果有任意参数为空字符串或null，抛出ArgumentNullException异常；
        /// 如果begin与end相同，抛出ArgumentException异常；
        /// 如果src中未找到begin或end,抛出InvalidOperationException异常；
        /// 如果begin与end有重复或交叉，则异常InvalidDataException异常；
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="begin">起始字符串</param>
        /// <param name="end">结束字符串</param>
        /// <param name="newValue">新字符串</param>
        /// <returns></returns>
        public static string ReplaceFirst(string src, string begin, string end, string newValue)
        {
            if (string.IsNullOrEmpty(src)) throw new ArgumentNullException(nameof(src));
            if (string.IsNullOrEmpty(begin)) throw new ArgumentNullException(nameof(begin));
            if (string.IsNullOrEmpty(end)) throw new ArgumentNullException(nameof(end));
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));

            if (begin == end) throw new ArgumentException("参数[" + nameof(newValue) + "]与参数[" + nameof(newValue) + "]的字符串不可相同！");

            var beginIndex = src.IndexOf(begin);
            if (beginIndex < 0) throw new InvalidOperationException("从源字符串中未找到起始字符串");
            var endIndex = src.IndexOf(end);
            if (endIndex < 0) throw new InvalidOperationException("从源字符串中未找到结束字符串");

            if (beginIndex >= endIndex || endIndex < beginIndex + begin.Length) throw new InvalidDataException("起始字符串与结束字符串不可有重复或交叉字符串！");

            return ReplaceInternal(src, beginIndex + begin.Length, endIndex, newValue);
        }

        /// <summary>
        /// 将源字符串src中，将从最后一个begin起始字符串开始之后到最后一个end结束字符串结束之前的字符串替换为newValue新字符串；返回结果保留begin起始字符串与end结束字符串；
        /// 如果有任意参数为空字符串或null，抛出ArgumentNullException异常；
        /// 如果begin与end相同，抛出ArgumentException异常；
        /// 如果src中未找到begin或end,抛出InvalidOperationException异常；
        /// 如果begin与end有重复或交叉，则异常InvalidDataException异常；
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="begin">起始字符串</param>
        /// <param name="end">结束字符串</param>
        /// <param name="newValue">新字符串</param>
        /// <returns></returns>
        public static string ReplaceLast(string src, string begin, string end, string newValue)
        {
            if (string.IsNullOrEmpty(src)) throw new ArgumentNullException(nameof(src));
            if (string.IsNullOrEmpty(begin)) throw new ArgumentNullException(nameof(begin));
            if (string.IsNullOrEmpty(end)) throw new ArgumentNullException(nameof(end));
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));

            if (begin == end) throw new ArgumentException("参数[" + nameof(newValue) + "]与参数[" + nameof(newValue) + "]的字符串不可相同！");

            var beginIndex = src.LastIndexOf(begin);
            if (beginIndex < 0) throw new InvalidOperationException("从源字符串中未找到起始字符串");
            var endIndex = src.LastIndexOf(end);
            if (endIndex < 0) throw new InvalidOperationException("从源字符串中未找到结束字符串");

            if (beginIndex >= endIndex || endIndex < beginIndex + begin.Length) throw new InvalidDataException("起始字符串与结束字符串不可有重复或交叉字符串！");

            return ReplaceInternal(src, beginIndex + begin.Length, endIndex, newValue);
        }

        /// <summary>
        /// 将源字符串src中，将从第一个begin起始字符串开始之后到最后一个end结束字符串结束之前的字符串替换为newValue新字符串；返回结果保留begin起始字符串与end结束字符串；
        /// 如果有任意参数为空字符串或null，抛出ArgumentNullException异常；
        /// 如果begin与end相同，抛出ArgumentException异常；
        /// 如果src中未找到begin或end,抛出InvalidOperationException异常；
        /// 如果begin与end有重复或交叉，则异常InvalidDataException异常；
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="begin">起始字符串</param>
        /// <param name="end">结束字符串</param>
        /// <param name="newValue">新字符串</param>
        /// <returns></returns>
        public static string Replace(string src, string begin, string end, string newValue)
        {
            if (string.IsNullOrEmpty(src)) throw new ArgumentNullException(nameof(src));
            if (string.IsNullOrEmpty(begin)) throw new ArgumentNullException(nameof(begin));
            if (string.IsNullOrEmpty(end)) throw new ArgumentNullException(nameof(end));
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));

            if (begin == end) throw new ArgumentException("参数[" + nameof(newValue) + "]与参数[" + nameof(newValue) + "]的字符串不可相同！");

            var beginIndex = src.IndexOf(begin);
            if (beginIndex < 0) throw new InvalidOperationException("从源字符串中未找到起始字符串");
            var endIndex = src.LastIndexOf(end);
            if (endIndex < 0) throw new InvalidOperationException("从源字符串中未找到结束字符串");

            if (beginIndex >= endIndex || endIndex < beginIndex + begin.Length) throw new InvalidDataException("起始字符串与结束字符串不可有重复或交叉字符串！");

            return ReplaceInternal(src, beginIndex + begin.Length, endIndex, newValue);
        }

        /// <summary>
        /// 将源字符串src中，将从索引begin(含)开始之后索引end(不含)结束之前的字符串替换为newValue新字符串；即返回：[0,begin)+newValue+[end,src.Count]的字符串；
        /// 如果src为空字符串或null，抛出ArgumentNullException异常；
        /// 如果newValue为null，抛出ArgumentNullException异常；
        /// 如果begin小于0或者超过源字符串的长度，抛出ArgumentOutOfRangeException异常；
        /// 如果end小于0或者大于源字符串的最大索引长度，抛出ArgumentOutOfRangeException异常；
        /// 如果end小于begin,则异常InvalidDataException异常；
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="begin">起始字符串</param>
        /// <param name="end">结束字符串</param>
        /// <param name="newValue">新字符串</param>
        /// <returns></returns>
        public static string Replace(string src, int begin, int end, string newValue)
        {
            if (string.IsNullOrEmpty(src)) throw new ArgumentNullException(nameof(src));
            if (newValue == null) throw new ArgumentNullException(nameof(newValue));
            var count = src.Length;
            if (begin < 0 || begin > count) throw new ArgumentOutOfRangeException(nameof(begin));
            if (end < 0 || end >= count) throw new ArgumentOutOfRangeException(nameof(end));

            if (end < begin) throw new InvalidDataException("结束索引值不得小于起始索引值！");

            return ReplaceInternal(src, begin, end, newValue);
        }

        internal static string ReplaceInternal(string src, int begin, int end, string newValue)
        {
            var left = src.Substring(0, begin);
            var right = src.Substring(end);
            return left + newValue + right;
        }
    }
}
