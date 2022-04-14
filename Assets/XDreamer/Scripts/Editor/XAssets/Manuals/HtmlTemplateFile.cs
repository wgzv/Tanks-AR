using XCSJ.Attributes;
using XCSJ.Interfaces;

namespace XCSJ.EditorExtension.XAssets.Manuals
{
    /// <summary>
    /// 网页模版文件
    /// </summary>
    [Name("网页模版文件")]
    public class HtmlTemplateFile : IExpand
    {
        /// <summary>
        /// 索引
        /// </summary>
        public string index = "";
        public string index_params = "";

        public string api = "";
        public string table = "";

        public string field_index = "";

        public string property_index = "";

        public string method_index = "";
        public string method_param = "";
        public string method_submethod = "";

        public string transit = "";
        public string transit_class_param = "";
        public string transit_namaspace_param = "";

        public bool expand { get; set; } = false;

        private static string GetFullPath(string name)
        {
            return ManualHelper.HtmlTemplates + "/" + name.Replace("_", ".") + ManualHelper.DotExt;
        }

        public void InitFile()
        {
            index = GetFullPath(nameof(index));
            index_params = GetFullPath(nameof(index_params));

            api = GetFullPath(nameof(api));
            table = GetFullPath(nameof(table));

            field_index = GetFullPath(nameof(field_index));
            property_index = GetFullPath(nameof(property_index));
            method_index = GetFullPath(nameof(method_index));
            method_param = GetFullPath(nameof(method_param));
            method_submethod = GetFullPath(nameof(method_submethod));

            transit = GetFullPath(nameof(transit));
            transit_class_param = GetFullPath(nameof(transit_class_param));
            transit_namaspace_param = GetFullPath(nameof(transit_namaspace_param));
        }
    }
}
