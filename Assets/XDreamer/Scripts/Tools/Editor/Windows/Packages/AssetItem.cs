using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.Packages
{
    public enum EEnableState
    {
        NotSet = -1,

        None = 0,

        All,

        Mixed,
    }

    [Import]
    public class AssetItem : ITreeNodeGraph, INamePath
    {
        public AssetItem() { }

        public AssetItem(ExportPackageItem exportPackageItem)
        {
            this.assetPath = exportPackageItem.assetPath;
            this.guid = exportPackageItem.guid;
            this.isFolder = exportPackageItem.isFolder;
            this.enabledStatus = exportPackageItem.enabledStatus;
        }

        public string assetPath = "";

        public int enabledStatus = 0;

        [Json(false)]
        public EEnableState enabledState
        {
            get => (EEnableState)enabledStatus;
            set => enabledStatus = (int)value;
        }

        [Json(false)]
        public string guid = "";

        [Json(false)]
        public bool isFolder = false;

        [Json(false)]
        public AssetItem parent;

        [Json(false)]
        public List<AssetItem> assetItems = new List<AssetItem>();

        #region ITreeNodeGraph

        public GUIContent display => CommonFun.TempContent(name);

        public bool enable { get => enabledStatus > 0; set => enabledState = value ? EEnableState.All : EEnableState.None; }
        public bool visible { get; set; } = true;

        public int depth => parent == null ? 0 : parent.depth + 1;

        public bool expanded { get; set; } = false;
        public bool selected { get; set; } = false;

        ITreeNodeGraph ITreeNodeGraph.parent => parent;

        public ITreeNodeGraph[] children => assetItems.Cast(i => (ITreeNodeGraph)i).ToArray();

        public string displayName => name;

        ITreeNode ITreeNode.parent => parent;

        ITreeNode[] ITreeNode.children => assetItems.Cast(i => (ITreeNode)i).ToArray();

        public string name { get => Path.GetFileName(assetPath); set { } }

        INamePath INamePath.parent => parent;

        public void OnClick()
        {
            //
        }

        #endregion

        /// <summary>
        /// 递归遍历当前项即当前项所有子项
        /// </summary>
        /// <param name="action"></param>
        public void Foreach(Action<AssetItem> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            action(this);
            assetItems.ForEach(i => i.Foreach(action));
        }

        public bool Any(Func<AssetItem, bool> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (func(this)) return true;
            return AnyChildren(func);
        }

        public bool AnyChildren(Func<AssetItem, bool> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            return assetItems.Any(i => func(i) || i.AnyChildren(func));
        }

        public void SetEnable(bool enable)
        {
            TreeView.Enable(this, enable, true);

            //更新父级状态
            var parent = this.parent;
            while (parent != null)
            {
                var hasEnable = parent.AnyChildren(i => i.enable);
                var hasDisable = parent.AnyChildren(i => !i.enable);
                if (hasEnable && hasDisable)//有启用有禁用
                {
                    parent.enabledState = EEnableState.Mixed;
                }
                else if (hasEnable && !hasDisable)//有启用无禁用
                {
                    parent.enabledState = EEnableState.All;
                }
                else if (!hasEnable && hasDisable)//无启用有禁用
                {
                    parent.enabledState = EEnableState.None;
                }
                else//无启用无禁用-即为空
                {
                    parent.enabledState = EEnableState.None;
                }
                parent = parent.parent;
            }
        }

        public AssetItem FindByNamePath(string namePath) => this.Find(namePath, i1 => i1.assetItems);
    }
}
