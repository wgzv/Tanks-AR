using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 数据记录器
    /// </summary>
    public abstract class DataRecorder : StateComponent
    {
        /// <summary>
        /// 数据记录模式:用于标识数据记录模式，即记录状态组件生命周期中哪些时刻的数据；
        /// </summary>
        [Name("数据记录模式")]
        [Tip("用于标识数据记录模式，即记录状态组件生命周期中哪些时刻的数据；")]
        [EnumPopup]
        public EDataRecordMode dataRecordMode = EDataRecordMode.Default;

        /// <summary>
        /// 可恢复的数据记录器对象
        /// </summary>
        public abstract Dictionary<EDataRecordMode, IRecoverableDataRecorder> recorders { get; }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="dataRecoveryMode"></param>
        public virtual void Recover(EDataRecordMode dataRecoveryMode, EDataRecoveryRule dataRecoveryRule = EDataRecoveryRule.All, string dataRecoveryRuleValue = "")
        {
            if (recorders.TryGetValue(dataRecoveryMode, out IRecoverableDataRecorder recorder) && recorder != null)
            {
                recorder.Recovery(dataRecoveryRule, dataRecoveryRuleValue);
            }
        }

        /// <summary>
        /// 是否处于完成态
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;
    }

    /// <summary>
    /// 可恢复的数据记录器
    /// </summary>
    public interface IRecoverableDataRecorder : IRecorder
    {
        void Recovery(EDataRecoveryRule dataRecoveryRule, string dataRecoveryRuleValue);
    }

    /// <summary>
    /// 可恢复的数据记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRecoverableDataRecorder<T> : IRecoverableDataRecorder, IRecorder<T>
    {
    }

    /// <summary>
    /// 数据记录器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRecorder"></typeparam>
    public  class DataRecorder<T, TRecorder> : DataRecorder
        where T : DataRecorder<T, TRecorder>
        where TRecorder : class, IRecoverableDataRecorder<T>, new()
    {
        /// <summary>
        /// 自身对象
        /// </summary>
        public T self => (T)this;

        /// <summary>
        /// 记录器对象
        /// </summary>
        public override Dictionary<EDataRecordMode, IRecoverableDataRecorder> recorders { get; } = new Dictionary<EDataRecordMode, IRecoverableDataRecorder>();

        /// <summary>
        /// 创建携带当前状态组件的普通状态
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的普通状态会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static NormalState CreateNormalState(IGetStateCollection obj, Action<NormalState> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateNormalState<T>(init, stateComponentTypes);
        }

        /// <summary>
        /// 创建携带当前状态组件的子状态机
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的子状态机会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static SubStateMachine CreateSubStateMachine(IGetStateCollection obj, Action<SubStateMachine> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateSubStateMachine<T>(init, stateComponentTypes);
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="dataRecordMode"></param>
        protected void Record(EDataRecordMode dataRecordMode)
        {
            if ((this.dataRecordMode & dataRecordMode) != 0)
            {
                var recorder = new TRecorder();
                recorder.Record(self);
                recorders[dataRecordMode] = recorder;
            }           
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            Record(EDataRecordMode.Init);
            return base.Init(data);
        }

        /// <summary>
        /// 进入激活态前回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);
            Record(EDataRecordMode.OnBeforeEntry);
        }

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            Record(EDataRecordMode.OnEntry);
        }

        /// <summary>
        /// 进入激活态后回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterEntry(StateData stateData)
        {
            base.OnAfterEntry(stateData);
            Record(EDataRecordMode.OnAfterEntry);
        }

        /// <summary>
        /// 更新时回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            Record(EDataRecordMode.OnUpdate);
        }

        /// <summary>
        /// 退出激活态前回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeExit(StateData stateData)
        {
            base.OnBeforeExit(stateData);
            Record(EDataRecordMode.OnBeforeExit);
        }

        /// <summary>
        /// 退出激活态回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            Record(EDataRecordMode.OnExit);
        }

        /// <summary>
        /// 退出激活态后回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);
            Record(EDataRecordMode.OnAfterExit);
        }

        /// <summary>
        /// 重置时调用
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);

            switch (data.dataRule)
            {
                case EDataRule.Init:
                    {
                        Recover(EDataRecordMode.Init);
                        break;
                    }
                case EDataRule.Entry:
                    {
                        Recover(EDataRecordMode.OnEntry);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 数据记录模式
    /// </summary>
    [Name("数据记录模式")]
    [Flags]
    public enum EDataRecordMode
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Name("初始化")]
        [EnumFieldName("初始化")]
        Init = 1 << 0,

        /// <summary>
        /// 预进入
        /// </summary>
        [Name("预进入")]
        [EnumFieldName("预进入")]
        OnBeforeEntry = 1 << 1,

        /// <summary>
        /// 进入
        /// </summary>
        [Name("进入")]
        [EnumFieldName("进入")]
        OnEntry = 1 << 2,

        /// <summary>
        /// 已进入
        /// </summary>
        [Name("已进入")]
        [EnumFieldName("已进入")]
        OnAfterEntry = 1 << 3,

        /// <summary>
        /// 更新
        /// </summary>
        [Name("更新")]
        [EnumFieldName("更新")]
        OnUpdate = 1 << 4,

        /// <summary>
        /// 预退出
        /// </summary>
        [Name("预退出")]
        [EnumFieldName("预退出")]
        OnBeforeExit = 1 << 5,

        /// <summary>
        /// 退出
        /// </summary>
        [Name("退出")]
        [EnumFieldName("退出")]
        OnExit = 1 << 6,

        /// <summary>
        /// 已退出
        /// </summary>
        [Name("已退出")]
        [EnumFieldName("已退出")]
        OnAfterExit = 1 << 7,

        /// <summary>
        /// 默认:包括初始化、进入、退出
        /// </summary>
        [Name("默认")]
        [Tip("包括初始化、进入、退出")]
        [EnumFieldName("默认")]
        Default = Init | OnEntry | OnExit,
    }
}
