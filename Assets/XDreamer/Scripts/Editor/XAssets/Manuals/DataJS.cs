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
    /// 数据JS文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Import]
    public class DataJSFile<T> : IInit where T : Data<T>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Json(false)]
        public string name = "";

        /// <summary>
        /// 列表数据
        /// </summary>
        public List<T> listdata = new List<T>();

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("var {0} = {{ {1}: {2} }}", name, nameof(listdata), JsonHelper.ToJson(listdata, true));
            return sb.ToString();
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static string ToString(string name, params T[] datas)
        {
            var file = new DataJSFile<T>() { name = name };
            file.listdata.AddRange(datas);
            return file.ToString();
        }

        /// <summary>
        /// 尝试加载
        /// </summary>
        /// <param name="str"></param>
        /// <param name="dataJSFile"></param>
        /// <returns></returns>
        public static bool TryLoad(string str, out DataJSFile<T> dataJSFile)
        {
            dataJSFile = new DataJSFile<T>();
            if (string.IsNullOrEmpty(str)) return false;
            var index = str.IndexOf("[");
            if (index <= 0) return false;
            var names = str.Substring(0, index);
            var datas = str.Substring(index).Trim().TrimEnd('}');

            var nameArray = names.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            dataJSFile.name = nameArray?.ElementAtOrDefault(1);

            dataJSFile.listdata = JsonHelper.ToObject<List<T>>(datas);

            dataJSFile.Init();
            return !string.IsNullOrEmpty(dataJSFile.name) && dataJSFile.listdata != null;
        }

        /// <summary>
        /// 尝试加载
        /// </summary>
        /// <param name="str"></param>
        /// <param name="listdata"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryLoad(string str, out List<T> listdata, out string name)
        {
            if(TryLoad(str, out DataJSFile<T> dataJSFile))
            {
                listdata = dataJSFile.listdata;
                name = dataJSFile.name;
                return true;
            }
            listdata = null;
            name = null;
            return false;  
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            if (listdata == null) listdata = new List<T>();
            foreach (var d in listdata)
            {
                d.Init(this as T);
            }
            return true;
        }
    }

    /// <summary>
    /// 数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Import]
    public class Data<T> : IInit<T> where T : Data<T>
    {
        /// <summary>
        /// 索引ID
        /// </summary>
        [Tip("必备，唯一，用于搜索")]
        public string id = "";

        /// <summary>
        /// 文本
        /// </summary>
        [Tip("文本显示")]
        public string text = "";

        /// <summary>
        /// 图标
        /// </summary>
        [Tip("小图标，复制即可")]
        public string icon = "index_data/images/xdreamer.png";

        /// <summary>
        /// 状态
        /// </summary>
        public State state = new State();

        /// <summary>
        /// 属性
        /// </summary>
        public A_Attr a_attr = new A_Attr();

        /// <summary>
        /// 子对象
        /// </summary>
        [Tip("子对象")]
        public List<T> children = new List<T>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool Init(T data)
        {
            if (children == null) children = new List<T>();
            foreach (var d in children)
            {
                d.Init(this as T);
            }
            return true;
        }
    }

    /// <summary>
    /// 状态
    /// </summary>
    [Import]
    public class State
    {
        /// <summary>
        /// 展开
        /// </summary>
        [Tip("是否折叠")]
        public bool opened = false;

        /// <summary>
        /// 选择
        /// </summary>
        [Tip("是否选择")]
        public bool selected {get ;private set;}= false;
    }

    /// <summary>
    /// 属性
    /// </summary>
    [Import]
    public class A_Attr
    {
        /// <summary>
        /// 链接信息
        /// </summary>
        [Tip("链接信息")]
        public string href = "";

        /// <summary>
        /// 提示信息
        /// </summary>
        [Tip("提示信息")]
        public string text_en = "";
    }
}
