using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Manuals
{
    /// <summary>
    /// 手册数据JS
    /// </summary>
    public class ManualDataJS
    {
        public Data root = Root();
        public Data mainData = Main();
        public Dictionary<string, Data> datas = new Dictionary<string, Data>();
        public static List<Data> dataList = new List<Data>();

        public void Init()
        {
            root = Root();
            mainData = Main();
            datas.Clear();
            dataList.Clear();
            Add(root);
            Add(root, mainData);
        }

        private void Add(Data parent, Data son)
        {
            Add(son);
            parent.children.Add(son);
        }

        private void Add(Data data)
        {
            datas[data.key] = data;
            dataList.Add(data);
        }

        public void Insert(Type type)
        {
            if (datas.TryGetValue(Data.IndexKey(type), out Data data))
            {
                data.type = type;
                return;
            }
            data = Data.Index(type);
            var parent = GetData(data.GetParentKey());
            Add(parent, data);
        }

        public Data GetData(string key)
        {
            if (string.IsNullOrEmpty(key)) return root;
            if (datas.TryGetValue(key, out Data data)) return data;

            data = Data.Key(key);
            Add(GetData(Data.ParentKey(key)), data);
            return data;
        }

        public string ToDataJS(HtmlTemplate htmlTemplate, string name = "manualdata")
        {
            UpdateData();
            return DataJSFile<Data>.ToString(name, root);
        }

        private void UpdateData()
        {
            foreach (var datat in dataList)
            {
                UpdateData(datat);
            }
        }

        private void UpdateData(Data data)
        {
            if (!data.id.StartsWith(root.text + "/"))
            {
                data.id = root.text + "/" + data.id;
            }
            if (!data.a_attr.href.StartsWith(root.text + "/"))
            {
                data.a_attr.href = root.text + "/" + data.a_attr.href;
            }
        }

        private static Data Main()
        {
            var data = new Data()
            {
                key = ManualHelper.DataName + "." + nameof(XCSJ),
                id = ManualHelper.DataName + "/" + nameof(XCSJ) + "/" + ManualHelper.IndexFile,
                text = nameof(XCSJ),
            };
            data.state.opened = false;
            data.a_attr.href = data.id;
            return data;
        }

        private static Data Root()
        {
            var data = new Data()
            {
                key = ManualHelper.DataName,
                id = ManualHelper.DataName + "/" + ManualHelper.IndexFile,
                text = ManualHelper.DataName,
                output = false,
            };
            data.state.opened = false;
            data.a_attr.href = data.id;
            return data;
        }
    }

    [Import]
    public class Data : Data<Data>
    {
        /// <summary>
        /// 数据内部检索使用，层级之间用.间隔
        /// </summary>
        [Json(false)]
        public string key = "";

        /// <summary>
        /// 数据关联的类型
        /// </summary>
        [Json(false)]
        public Type type = null;

        /// <summary>
        /// 关联类型的命名空间
        /// </summary>
        [Json(false)]
        public string nameSpace => type != null ? type.Namespace : key.Replace(ManualHelper.DataName + ".", "");

        /// <summary>
        /// 是否输出到磁盘
        /// </summary>
        [Json(false)]
        public bool output = true;

        public string GetParentKey() => ParentKey(key);

        public string GetValidParentKey()
        {
            var pk = GetParentKey();
            return (string.IsNullOrEmpty(pk) || pk == ManualHelper.DataName) ? pk : pk.Replace(ManualHelper.DataName + ".", "");
        }

        public static string ParentKey(string key) => Path.GetFileNameWithoutExtension(key);
        public static string IndexKey(Type type) => ManualHelper.DataName + "." + ManualHelper.GetFullName(type);

        public static Data Key(string key)
        {
            var data = new Data()
            {
                key = key,
                id = ManualHelper.GetPage(key),
                text = Path.GetExtension(key).TrimStart('.')
            };
            data.a_attr.href = data.id;
            return data;
        }
        public static Data Index(Type type)
        {
            var data = new Data()
            {
                key = IndexKey(type),
                type = type,
                id = ManualHelper.GetPage(type),
                text = CommonFun.Name(type)
            };
            data.a_attr.href = data.id;
            data.a_attr.text_en = type.Name;
            return data;
        }
    }
}
