using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.Helper;
using XCSJ.LitJson;

namespace XCSJ.EditorTools.Windows.Packages
{
    /// <summary>
    /// 导出文件
    /// </summary>
    [Import]
    public class ExportFile
    {
        /// <summary>
        /// 导出列表
        /// </summary>
        public List<string> exportList = new List<string>();

        /// <summary>
        /// 忽略列表
        /// </summary>
        public List<string> ignoreList = new List<string>();

        /// <summary>
        /// 导出后的文件名
        /// </summary>
        [Json(false)]
        public string name;

        /// <summary>
        /// 导出
        /// </summary>
        [Json(false)]
        public bool export = true;

        /// <summary>
        /// 目录
        /// </summary>
        [Json(false)]
        public const string Dir = "Assets/XDreamer/EditorResources/Tools/Packages/";

        /// <summary>
        /// 扩展名
        /// </summary>
        [Json(false)]
        public const string Ext = ".json";

        /// <summary>
        /// 路径
        /// </summary>
        [Json(false)]
        public string path => Path.Combine(Dir, name + Ext);

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            FileHelper.OutputFile(path, JsonHelper.ToJson(this, true));
        }

        /// <summary>
        /// 加载全部
        /// </summary>
        /// <returns></returns>
        public static List<ExportFile> LoadAll()
        {
            var files = new List<ExportFile>();
            foreach (var p in Directory.GetFiles(Dir, "*" + Ext))
            {
                if (JsonHelper.ToObject<ExportFile>(FileHelper.InputFile(p)) is ExportFile file)
                {
                    file.name = Path.GetFileNameWithoutExtension(p);
                    files.Add(file);
                }
            }
            return files;
        }
    }
}
