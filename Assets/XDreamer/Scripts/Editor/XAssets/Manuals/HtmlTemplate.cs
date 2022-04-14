using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.XAssets.Compilers;
using XCSJ.Extension;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Manuals
{
    /// <summary>
    /// 网页模版
    /// </summary>
    [Name("网页模版")]
    public class HtmlTemplate : HtmlTemplateFile
    {
        public string workDirectory { get; private set; } = "";

        public void Init(HtmlTemplateFile file, string workDirectory)
        {
            index = FileHelper.InputFile(UICommonFun.ToFullPath(file.index));
            index_params = FileHelper.InputFile(UICommonFun.ToFullPath(file.index_params));

            api = FileHelper.InputFile(UICommonFun.ToFullPath(file.api));
            table = FileHelper.InputFile(UICommonFun.ToFullPath(file.table));

            field_index = FileHelper.InputFile(UICommonFun.ToFullPath(file.field_index));
            property_index = FileHelper.InputFile(UICommonFun.ToFullPath(file.property_index));
            method_index = FileHelper.InputFile(UICommonFun.ToFullPath(file.method_index));
            method_param = FileHelper.InputFile(UICommonFun.ToFullPath(file.method_param));
            method_submethod = FileHelper.InputFile(UICommonFun.ToFullPath(file.method_submethod));

            transit = FileHelper.InputFile(UICommonFun.ToFullPath(file.transit));
            transit_class_param = FileHelper.InputFile(UICommonFun.ToFullPath(file.transit_class_param));
            transit_namaspace_param = FileHelper.InputFile(UICommonFun.ToFullPath(file.transit_namaspace_param));


            this.workDirectory = workDirectory + "/";
        }

        #region Type-类型页面

        private string CreateFile(MemberInfo memberInfo)
        {
            var fullPath = workDirectory + ManualHelper.GetPage(memberInfo);
            DirectoryHelper.Create(Path.GetDirectoryName(fullPath));
            return fullPath;
        }
        private T[] Sort<T>(T[] ms) where T : MemberInfo
        {
            Array.Sort(ms, (x, y) => string.CompareOrdinal(x.Name, y.Name));
            return ms;
        }
        private string ToUrl(MemberInfo from, Type to)
        {
            if (!NeedOutput(to)) return "";
            return ToUrl(ManualHelper.GetPageDir(from), ManualHelper.GetPage(to, false));
        }
        private string ToUrl(string from, string to)
        {
            if (string.IsNullOrEmpty(from)) from = "";
            if (string.IsNullOrEmpty(to)) to = "";
            var depth = from.Count(c => c == '/');
            while (depth-- > 0)
            {
                to = "../" + to;
            }
            return to;
        }

        private FieldInfo[] GetFields(Type type)
        {
            var list = new List< FieldInfo>();
            
            while(type!=null)
            {
                list.AddRangeWithDistinct(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.IsPublic || AttributeHelper.Exist<SerializeField>(m)), (f1, f2) => f1.Name == f2.Name);

                type = type.BaseType;
            }
            return list.ToArray();
        }

        private string ToIndex(Type type)
        {
            return ToIndex(index, type);
        }

        private const string ParamsBegin = "<!-- 参数列表 开始-->";
        private const string ParamsEnd = "<!-- 参数列表 结束-->";

        /// <summary>
        /// 对应index.html文件
        /// </summary>
        /// <param name="template"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ToIndex(string template, Type type)
        {
            var content = template.Replace("{中文名}", CommonFun.Name(type));
            content = content.Replace("{英文名}", type.Name);
            content = content.Replace("{简介}", CommonFun.Tip(type));

            var start = content.IndexOf(ParamsBegin);
            var end = content.IndexOf(ParamsEnd);
            if (start >= 0 && end >= 0)
            {
                var left = content.Substring(0, start + ParamsBegin.Length);
                var right = content.Substring(end);
                return left + ToIndexParams(type) + right;
            }
            else
            {
                return content.Replace("{参数列表}", ToIndexParams(type));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ToIndexParams(Type type)
        {
            StringBuilder sb = new StringBuilder();
            var ms = Sort(GetFields(type));
            foreach (var m in ms)
            {
                //Debug.Log(m.Name);
                if (!NeedOutput(m)) continue;
                sb.Append(ToIndexParams(m));
            }
            return sb.ToString();            
        }

        /// <summary>
        /// 对应index.params.html文件
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private string ToIndexParams(FieldInfo memberInfo)
        {
            var content = index_params.Replace("{名称}", CommonFun.Name(memberInfo));
            content = content.Replace("{名称.href}", ManualHelper.ApiData + "/" + memberInfo.Name);
            content = content.Replace("{功能描述}", CommonFun.Tip(memberInfo));
            return content;
        }

        /// <summary>
        /// 对应api.html文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ToApi(Type type)
        {
            var content = api.Replace("{中文名}", CommonFun.Name(type));
            content = content.Replace("{英文名}", type.Name);
            content = content.Replace("{类路径}", type.Namespace);
            //content = content.Replace("{类路径.href}", (type.DeclaringType != null ? ToUrl(type, type.DeclaringType) : ToUrl(XAssetStore.GetPageDir(type), XAssetStore.GetPage(type.Namespace))));
            content = content.Replace("{基类}", type.BaseType.Name);
            content = content.Replace("{基类.href}", ToUrl(type, type.BaseType));
            content = content.Replace("{提示}", CommonFun.Tip(type));

            var note = NoteHelper.GetNote(type);
            content = content.Replace("{概述}", note.summary.ToString());
            content = content.Replace("{字段}", ToField(type));
            content = content.Replace("{属性}", ToProperty(type));
            content = content.Replace("{方法}", ToMethod(type));

            return content;
        }

        /// <summary>
        /// 对应table.html文件
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private string ToTable(MemberInfo memberInfo)
        {
            var content = table.Replace("{成员名称}", memberInfo.Name);
            content = content.Replace("{成员名称.href}", ManualHelper.ApiData + "/" + memberInfo.Name);
            content = content.Replace("{成员中文名称}", CommonFun.Name(memberInfo));
            content = content.Replace("{成员描述}", CommonFun.Tip(memberInfo));
            return content;
        }

        private string ToField(Type type)
        {
            StringBuilder sb = new StringBuilder();
            var ms = Sort(GetFields(type));
            foreach (var m in ms)
            {
                if (!NeedOutput(m)) continue;

                FileHelper.OutputFile(CreateFile(m), ToField(m));

                sb.Append(ToTable(m));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 对应field.index.html文件
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private string ToField(FieldInfo memberInfo)
        {
            var content = field_index.Replace("{字段.英文名}", memberInfo.Name);
            content = content.Replace("{字段.中文名}", CommonFun.Name(memberInfo));
            content = content.Replace("{字段.类名称}", memberInfo.ReflectedType.FullName);
            content = content.Replace("{字段.提示}", CommonFun.Tip(memberInfo));
            content = content.Replace("{字段.概述}", NoteHelper.GetNote(memberInfo).summary.ToString());

            content = content.Replace("{字段.类型}", memberInfo.FieldType.Name);
            content = content.Replace("{字段.类型.href}", ToUrl(memberInfo, memberInfo.FieldType));
            return content;
        }

        private string ToProperty(Type type)
        {
            StringBuilder sb = new StringBuilder();
            var ms = Sort(type.GetProperties());
            foreach (var m in ms)
            {
                if (!NeedOutput(m)) continue;

                FileHelper.OutputFile(CreateFile(m), ToProperty(m));

                sb.Append(ToTable(m));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 对应property.index.html文件
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private string ToProperty(PropertyInfo memberInfo)
        {
            var content = property_index.Replace("{属性.英文名}", memberInfo.Name);
            content = content.Replace("{属性.中文名}", CommonFun.Name(memberInfo));
            content = content.Replace("{属性.类名称}", memberInfo.ReflectedType.FullName);
            content = content.Replace("{属性.提示}", CommonFun.Tip(memberInfo));
            content = content.Replace("{属性.概述}", NoteHelper.GetNote(memberInfo).summary.ToString());

            content = content.Replace("{属性.类型}", memberInfo.PropertyType.Name);
            content = content.Replace("{属性.类型.href}", ToUrl(memberInfo, memberInfo.PropertyType));
            return content;
        }

        private string ToMethod(Type type)
        {
            StringBuilder sb = new StringBuilder();
            var ms = Sort(type.GetMethods());
            foreach (var mg in ms.ToLookup(mi => mi.Name))
            {
                var mis = mg.Where(m1 => NeedOutput(m1));
                if (mis.Count() == 0) continue;

                var m = mis.First();

                FileHelper.OutputFile(CreateFile(m), ToMethods(mis.ToArray()));

                sb.Append(ToTable(m));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 对应method.index.html文件
        /// </summary>
        /// <param name="memberInfos"></param>
        /// <returns></returns>
        private string ToMethods(MethodInfo[] memberInfos)
        {
            var sb = new StringBuilder();
            foreach (var m in memberInfos)
            {
                sb.AppendLine(ToMethod(m));
            }
            var content = method_index.Replace("{方法.内容}", sb.ToString());
            return content;
        }

        /// <summary>
        /// 对应method.submethod.html文件
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private string ToMethod(MethodInfo memberInfo)
        {
            var content = method_submethod.Replace("{方法.名称}", memberInfo.Name);
            content = content.Replace("{方法.全名称}", memberInfo.ToString());
            content = content.Replace("{方法.类名称}", memberInfo.ReflectedType.FullName);
            content = content.Replace("{方法.提示}", CommonFun.Tip(memberInfo));

            var note = NoteHelper.GetNote(memberInfo);
            content = content.Replace("{方法.概述}", note.summary.ToString());
            content = content.Replace("{方法.返回值}", note.returns.ToString());
            content = content.Replace("{方法.返回值类型.href}", ToUrl(memberInfo, memberInfo.ReturnType));
            content = content.Replace("{方法.返回值类型}", memberInfo.ReturnType.Name);

            var sb = new StringBuilder();
            foreach (var p in memberInfo.GetParameters())
            {
                sb.AppendLine(ToMethodParam(p, note));
            }
            content = content.Replace("{方法.参数}", sb.ToString());

            return content;
        }

        /// <summary>
        /// 对应method.param.html文件
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        private string ToMethodParam(ParameterInfo parameterInfo, Note note)
        {
            var content = method_param.Replace("{方法.参数名称}", parameterInfo.Name);
            content = content.Replace("{方法.参数缺省值}", parameterInfo.HasDefaultValue ? Converter.instance.ConvertTo<string>(parameterInfo.DefaultValue) : "");
            content = content.Replace("{方法.参数描述}", note.param[parameterInfo.Name].ToString());

            content = content.Replace("{方法.参数类型}", parameterInfo.ParameterType.Name);
            content = content.Replace("{方法.参数类型.href}", ToUrl(parameterInfo.Member, parameterInfo.ParameterType));
            return content;
        }

        #endregion

        #region Data-中间层命名空间页面

        private string GetName(Data data)
        {
            return data.key.Contains('.') ? Path.GetExtension(data.key).TrimStart('.') : data.key;
        }

        /// <summary>
        /// 对应transit.html文件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ToData(Data data)
        {
            var content = transit.Replace("{命名空间.英文名}", GetName(data));
            content = content.Replace("{命名空间.路径}", data.GetValidParentKey());
            content = content.Replace("{概述}", data.nameSpace);

            content = content.Replace("{命名空间.子命名空间}", ToNamespaces(data));
            content = content.Replace("{命名空间.子类型}", ToTypes(data));

            return content;
        }

        private string ToNamespaces(Data data)
        {
            var namespaces = data.children.Where(d => d.type == null).ToList();
            namespaces.Sort((x, y) => string.Compare(x.key, y.key));
            StringBuilder sb = new StringBuilder();
            foreach (var d in namespaces)
            {
                sb.Append(ToNamespace(d));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 对应transit.namaspace.param.html文件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ToNamespace(Data data)
        {
            var name = GetName(data);
            var content = transit_namaspace_param.Replace("{命名空间.名称}", name);

            content = content.Replace("{命名空间.名称.href}", ManualHelper.GetPage(name));
            content = content.Replace("{命名空间.描述}", data.key);

            return content;
        }

        private string ToTypes(Data data)
        {
            var types = data.children.Where(d => d.type != null).ToList();
            types.Sort((x, y) => string.Compare(x.key, y.key));
            StringBuilder sb = new StringBuilder();
            foreach (var d in types)
            {
                sb.Append(ToType(d));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 对应transit.class.param.html文件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ToType(Data data)
        {
            var content = transit_class_param.Replace("{类.英文名}", data.type.Name);

            content = content.Replace("{类.英文名.href}", ManualHelper.GetPage(data.type.Name));
            content = content.Replace("{类.中文名}", CommonFun.Name(data.type));
            content = content.Replace("{类.描述}", CommonFun.Tip(data.type));

            return content;
        }

        #endregion

        public event Func<MemberInfo, bool> needOutput;

        public bool NeedOutput(MemberInfo memberInfo) => needOutput.All(memberInfo);

        public void Output(Type type)
        {
            DirectoryHelper.Create(workDirectory + ManualHelper.GetPageDir(type));
            var indexFile = workDirectory + ManualHelper.GetPage(type, true);

            //不再生成index_data与index_data/images文件夹
            //DirectoryHelper.Create(Path.Combine(Path.GetDirectoryName(indexFile), ManualHelper.IndexData));
            //DirectoryHelper.Create(Path.Combine(Path.GetDirectoryName(indexFile), ManualHelper.IndexData_Images));

            //无则添加，有则更新
            //if(FileHelper.Exists(indexFile))
            //{
            //    var file = FileHelper.InputFile(indexFile);
            //    FileHelper.OutputFile(indexFile, ToIndex(file, type));
            //}
            //else
            {
                //每次输出均为强制覆盖
                FileHelper.OutputFile(indexFile, ToIndex(type));
            }
            FileHelper.OutputFile(workDirectory + ManualHelper.GetPage(type, false), ToApi(type));
        }

        public void Output(Data data)
        {
            if (data.type != null || !data.output) return;
            DirectoryHelper.Create(workDirectory + ManualHelper.GetPageDir(data.key));

            FileHelper.OutputFile(workDirectory + ManualHelper.GetPage(data.key), ToData(data));
        }
    }
}
