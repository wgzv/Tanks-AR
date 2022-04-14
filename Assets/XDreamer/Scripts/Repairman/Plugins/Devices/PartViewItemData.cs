
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.PluginSMS.States.Dataflows.DataModel;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginRepairman.Devices
{
    /// <summary>
    /// 零件视图
    /// </summary>
    public class PartViewItemData : ViewItemData, IGameObjectViewItemData
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        public GameObject prototype => part.gameObject;

        /// <summary>
        /// 游戏对象数量
        /// </summary>
        public int count
        {
            get => _count;
            set
            {
                _count = value;
                SendDataChanged();
            }
        }
        private int _count = 0;

        /// <summary>
        /// 组
        /// </summary>
        public ISingleGroup group { get; }

        public Part part 
        { 
            get
            {
                // 查找非装配对象
                if (!_part || _part.state == Machines.EPartState.Assembly)
                {
                    _part = groupParts.Find(p => p.state!= Machines.EPartState.Assembly);
                }
                return _part;
            }
        }
        private Part _part;

        /// <summary>
        /// 同组零件列表
        /// </summary>
        public List<Part> groupParts { get; private set; } = new List<Part>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="part"></param>
        /// <param name="group"></param>
        /// <param name="enable"></param>
        public PartViewItemData(Part part, ISingleGroup group = null, bool enable = false) : this(new List<Part>() { part }, group, enable)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="group"></param>
        /// <param name="enable"></param>
        public PartViewItemData(List<Part> parts, ISingleGroup group = null, bool enable = false)
        {
            groupParts.AddRange(parts);

            this.group = group;
            this.enable = enable;
            title = group != null ? group.groupName : part.showName; // 有组优先使用组名称

            var p = part;
            if (p && p.texture2D) this.sprite = XGUIHelper.Texture2DToSprite(p.texture2D);
            count = groupParts.Count;
        }

        /// <summary>
        /// 渲染游戏对象
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="size"></param>
        public void RenderGameObjectView(Camera camera, Vector2Int size)
        {
            if (!sprite)
            {
                sprite = XGUIHelper.Texture2DToSprite(GameOjectViewInfoMB.GetTexture(camera, size, prototype));
            }
        }

        /// <summary>
        /// 零件装配回调
        /// </summary>
        /// <param name="part"></param>
        public void OnPartAssembly(Part part)
        {
            if (groupParts.Contains(part))
            {
                groupParts.Remove(part);
                --count;
            }
        }
    }
}
