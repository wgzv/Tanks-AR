using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 激活游戏对象信息
    /// </summary>
    [Serializable]
    public class ActiveGameObjectInfo : IDisplayAsArrayElement
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        public GameObject _gameObject;

        /// <summary>
        /// 激活
        /// </summary>
        [Name("激活")]
        [EnumPopup]
        public EBool _active = EBool.Yes;

        /// <summary>
        /// 激活
        /// </summary>
        public void Active()
        {
            _gameObject.XSetActive(_active);
        }

        /// <summary>
        /// 获取GUI内容文本
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetGUIContentText(int index) => (index + 1).ToString() + "." + (_gameObject ? _gameObject.name : "") + "." + CommonFun.Name(_active);

        /// <summary>
        /// 获取GUI内容提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetGUIContentTooltip(int index) => (index + 1).ToString() + "." + (_gameObject ? _gameObject.name : "") + "." + CommonFun.Name(_active);
    }

    /// <summary>
    /// 激活游戏对象信息列表
    /// </summary>
    /// <typeparam name="TEnumEvent"></typeparam>
    public class ActiveGameObjectInfoList<TEnumEvent> : IDisplayAsArrayElement
    {
        /// <summary>
        /// 枚举事件
        /// </summary>
        [Name("枚举事件")]
        [EnumPopup]
        public TEnumEvent _enumEvent = default;

        /// <summary>
        /// 激活信息列表
        /// </summary>
        [Name("激活信息列表")]
        [OnlyMemberElements]
        public List<ActiveGameObjectInfo> _infos = new List<ActiveGameObjectInfo>();

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="active"></param>
        public virtual void Add(GameObject gameObject, EBool active)
        {
            if (!gameObject) return;
            var info = _infos.FirstOrNew(i => i._gameObject == gameObject, i =>
            {
                i._gameObject = gameObject;
                _infos.Add(i);
            });
            info._active = active;
        }

        /// <summary>
        /// 移除全部
        /// </summary>
        /// <param name="gameObject"></param>
        public virtual void RemoveAll(GameObject gameObject)
        {
            _infos.RemoveAll(i => i._gameObject == gameObject);
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="enumEvent"></param>
        public virtual bool Active(TEnumEvent enumEvent)
        {
            if (EqualityComparer<TEnumEvent>.Default.Equals(_enumEvent, enumEvent))
            {
                Active();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 激活
        /// </summary>
        public virtual void Active()
        {
            foreach (var info in _infos)
            {
                info.Active();
            }
        }

        /// <summary>
        /// 获取GUI内容文本
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual string GetGUIContentText(int index) => (index + 1).ToString() + "." + CommonFun.Name(typeof(TEnumEvent), _enumEvent.ToString());

        /// <summary>
        /// 获取GUI内容提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual string GetGUIContentTooltip(int index) => (index + 1).ToString() + "." + CommonFun.Name(typeof(TEnumEvent), _enumEvent.ToString());
    }

    /// <summary>
    /// 激活游戏对象事件集合
    /// </summary>
    /// <typeparam name="TEnumEvent"></typeparam>
    /// <typeparam name="TInfoList"></typeparam>
    public class ActiveGameObjectEventSet<TEnumEvent, TInfoList> where TInfoList : ActiveGameObjectInfoList<TEnumEvent>
    {
        /// <summary>
        /// 信息列表
        /// </summary>
        [Name("信息列表")]
        public List<TInfoList> _infoLists = new List<TInfoList>();

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="enumEvent"></param>
        public virtual void Active(TEnumEvent enumEvent)
        {
            foreach (var info in _infoLists)
            {
                info.Active(enumEvent);
            }
        }
    }
}
