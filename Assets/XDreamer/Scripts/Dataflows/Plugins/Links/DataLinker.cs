using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Links;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginDataflows.Links
{
    /// <summary>
    /// 数据连接类
    /// 实现源与目标对象的变化通知关联
    /// </summary>
    [Serializable]
    public class DataLinker : ILinker, IDataLink
    {
        #region 源数据

        /// <summary>
        /// 源绑定器别名
        /// </summary>
        [Name("源别名")]
        public string _sourceBinderAlias = "";

        public string sourceBinderAlias
        {
            get => _sourceBinderAlias;
            set
            {
                if (_sourceBinderAlias != value)
                {
                    Unbind();
                    _sourceBinderAlias = value;
                    _sourceBindData = null;
                    Bind();
                }
            }
        }

        protected ITypeMemberDataBinder _sourceBindData = null;

        /// <summary>
        /// 源绑定数据
        /// </summary>
        public ITypeMemberDataBinder sourceBindData
        {
            get
            {
                if (_sourceBindData == null)
                {
                    _sourceBindData = DataBinderHelper.CreateIBindData(sourceBinderAlias, binderGetter);
                }
                return _sourceBindData;
            }
        }

        #endregion

        #region 连接

        /// <summary>
        /// 连接模式
        /// </summary>
        [Name("连接模式")]
        [EnumPopup]
        public EDataLinkMode _dataLinkMode = EDataLinkMode.ToTarget;

        public EDataLinkMode dataLinkMode 
        { 
            get => _dataLinkMode; 
            set
            {
                if (_dataLinkMode != value)
                {
                    _dataLinkMode = value;
                    UpdateValue();
                }
            }
        }

        public bool isBind { get; private set; }

        public void OnValidate()
        {
            UpdateValue();
        }

        #endregion

        #region 目标数据

        /// <summary>
        /// 目标绑定器别名
        /// </summary>
        [Name("目标别名")]
        public string _targetBinderAlias = "";

        public string targetBinderAlias
        {
            get => _targetBinderAlias;
            set
            {
                if (_targetBinderAlias != value)
                {
                    Unbind();
                    _targetBinderAlias = value;
                    _targetBindData = null;
                    Bind();
                }
            }
        }

        protected ITypeMemberDataBinder _targetBindData = null;

        /// <summary>
        /// 目标绑定数据
        /// </summary>
        public ITypeMemberDataBinder targetBindData
        {
            get
            {
                if (_targetBindData == null)
                {
                    _targetBindData = DataBinderHelper.CreateIBindData(targetBinderAlias, binderGetter);
                }
                return _targetBindData;
            }
        }

        #endregion

        private IBinderGetter[] binderGetter = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="getTypeBinder"></param>
        public void Init(params IBinderGetter[] binderGetter)
        {
            this.binderGetter = binderGetter;
        }

        /// <summary>
        /// 更新值
        /// </summary>
        public void UpdateValue()
        {
            if (!isBind) return;

            switch (_dataLinkMode)
            {
                case EDataLinkMode.ToTarget:
                case EDataLinkMode.Both:
                    sourceBindData.InitValue();
                    break;
                case EDataLinkMode.ToSource:
                    targetBindData.InitValue();
                    break;
            }
        }

        /// <summary>
        /// 绑定
        /// </summary>
        public virtual void Bind()
        {
            if (isBind) return;

            if (sourceBindData != null && targetBindData != null)
            {
                // 首先设置标志量，防止外围函数递归调用时重复添加回调函数
                isBind = true;

                sourceBindData.Bind(OnReceiveSourceEvent);
                targetBindData.Bind(OnReceiveTargetEvent);
                UpdateValue();
            }
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        public virtual void Unbind()
        {
            if (!isBind) return;

            isBind = false;

            sourceBindData?.Unbind(OnReceiveSourceEvent);
            targetBindData?.Unbind(OnReceiveTargetEvent);

            _sourceBindData = null;
            _targetBindData = null;
        }

        /// <summary>
        /// 响应源事件
        /// </summary>
        /// <param name="eventArgs"></param>
        public virtual void OnReceiveSourceEvent(XValueEventArg eventArgs)
        {
            switch (dataLinkMode)
            {
                case EDataLinkMode.ToTarget:
                case EDataLinkMode.Both:
                    {
                        targetBindData.Set(sourceBindData, eventArgs);
                        break;
                    }
            }
        }

        /// <summary>
        /// 响应目标事件
        /// </summary>
        /// <param name="eventArgs"></param>
        public virtual void OnReceiveTargetEvent(XValueEventArg eventArgs)
        {
            switch (dataLinkMode)
            {
                case EDataLinkMode.ToSource:
                case EDataLinkMode.Both:
                    {
                        sourceBindData.Set(targetBindData, eventArgs);
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置源或目标数据的主体对象 ：用于动态实例化主体对象
        /// </summary>
        /// <param name="mainObject"></param>
        /// <returns></returns>
        public bool SetDataMainObject(object mainObject)
        {
            var rs = sourceBindData.SetMainObject(mainObject);
            if (!rs)
            {
                rs = targetBindData.SetMainObject(mainObject);
            }

            // 设置值成功，更新数值
            if (rs)
            {
                UpdateValue();
            }
            return rs;
        }
    }

    /// <summary>
    /// 链接接口
    /// </summary>
    public interface ILinker
    {
        /// <summary>
        /// 源绑定器别名
        /// </summary>
        string sourceBinderAlias { get; }

        /// <summary>
        /// 目标绑定器别名
        /// </summary>
        string targetBinderAlias { get; }

        /// <summary>
        /// 数据链接模式
        /// </summary>
        EDataLinkMode dataLinkMode { get; }
    }
}
