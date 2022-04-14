using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Dataflows.Links;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataflows.Models;

namespace XCSJ.PluginDataflows.Links
{
    /// <summary>
    /// 数据连接器
    /// </summary>
    [Name("数据连接器")]
    public class LinkerMB : BaseDataflowMB, IDataLinkSet
    {
        /// <summary>
        /// 别名数据源
        /// </summary>
        [Name("别名数据源")]
        [EnumPopup]
        public EAliasDataSource _aliasDataSource = EAliasDataSource.Global;

        /// <summary>
        /// 别名源
        /// </summary>
        public EAliasDataSource aliasDataSource { get => _aliasDataSource; set => _aliasDataSource = value; }

        /// <summary>
        /// 数据连接列表
        /// </summary>
        [Name("数据连接列表")]
        [OnlyMemberElements]
        public DataLinkerList _linkerList = new DataLinkerList();

        /// <summary>
        /// 数据连接集合接口
        /// </summary>
        public IEnumerable<IDataLink> links => _linkerList._linkers;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            // 注册绑定器接口
            _linkerList.Init(AliasCache.instance.GetBinderGetter(_aliasDataSource, gameObject));
        }

        /// <summary>
        /// 有效改动
        /// </summary>
        public void OnValidate()
        {
            _linkerList.OnValidate();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            AliasCache.instance.onAddedAlias += OnAliasRegister;
            AliasCache.instance.onClearedAlias += OnAliasClear;
            _linkerList.Bind();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            AliasCache.instance.onAddedAlias -= OnAliasRegister;
            AliasCache.instance.onClearedAlias -= OnAliasClear;
            _linkerList.Unbind();
        }

        /// <summary>
        /// 别名注册回调
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="typeMemberBinder"></param>
        protected virtual void OnAliasRegister(string alias, ITypeMemberBinder typeMemberBinder)
        {
            if (enabled)
            {
                _linkerList.Bind();
            }
        }

        /// <summary>
        /// 别名清理回调
        /// </summary>
        protected virtual void OnAliasClear()
        {
            _linkerList.Unbind();
        }
    }

    /// <summary>
    /// 数据连接列表
    /// </summary>
    [Serializable]
    public class DataLinkerList
    {
        /// <summary>
        /// 数据连接列表
        /// </summary>
        [Name("数据连接列表")]
        public List<DataLinker> _linkers = new List<DataLinker>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="getTypeBinder"></param>
        public void Init(params IBinderGetter[] getTypeBinder)
        {
            foreach (var link in _linkers)
            {
                link.Init(getTypeBinder);
            }
        }

        /// <summary>
        /// 绑定
        /// </summary>
        public virtual void Bind() => _linkers.ForEach(dl => dl.Bind());

        /// <summary>
        /// 解除绑定
        /// </summary>
        public virtual void Unbind() => _linkers.ForEach(dl => dl.Unbind());

        /// <summary>
        /// 有效
        /// </summary>
        public void OnValidate() => _linkers.ForEach(dl => dl.OnValidate());
    }
}
