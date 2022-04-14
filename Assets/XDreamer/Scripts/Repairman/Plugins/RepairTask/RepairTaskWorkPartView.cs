
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Dataflows.DataModel;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginTools.GameObjects;

namespace XCSJ.PluginRepairman.Task
{
    /// <summary>
    /// 拆装任务零件视图：用于辅助拆装的零件列表
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairTaskWorkPartView))]
    [XCSJ.Attributes.Icon(EIcon.Task)]
    [DisallowMultipleComponent]
    [Tip("用于生成装配任务步骤中相关的UI零件列表，追踪零件装配的完成度;动态的生成对应的模块视图")]
    [RequireComponent(typeof(RepairTaskWork))]
    public sealed class RepairTaskWorkPartView : StateComponent<RepairTaskWorkPartView>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "拆装任务零件视图";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Name(Title, nameof(RepairTaskWorkPartView))]
        [Tip("用于生成装配任务步骤中相关的UI零件列表，追踪零件装配的完成度;动态的生成对应的模块视图")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateTaskWork(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(RepairTaskWorkPartView)), null, typeof(RepairTaskWorkPartView));
        }

        /// <summary>
        /// 零件列表界面
        /// </summary>
        [Name("零件列表界面")]
        [Tip("用于将设备对象中的零件生成列表的管理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public GameObjectViewItemDataList _gameObjectViewItemDataList;

        /// <summary>
        /// 进入时零件非激活
        /// </summary>
        [Name("未装配零件非激活")] 
        public bool _inActiveDisassemblyPart = true;

        /// <summary>
        /// 修理任务
        /// </summary>
        public RepairTaskWork repairTaskWork
        {
            get
            {
                if (!_repairTaskWork)
                {
                    _repairTaskWork = GetComponent<RepairTaskWork>();
                }
                return _repairTaskWork;
            }
        }
        private RepairTaskWork _repairTaskWork;

        /// <summary>
        /// 当前任务内所有拆装步骤
        /// </summary>
        private List<RepairStepByMatchPosition> _repairSteps = new List<RepairStepByMatchPosition>();

        private SingleGroupCache<Part> groupCache = new SingleGroupCache<Part>();

        /// <summary>
        /// 零件视图数据链表
        /// </summary>
        private List<PartViewItemData> _viewItemDatas = new List<PartViewItemData>();

        /// <summary>
        /// 游戏对象激活记录器
        /// </summary>
        private GameObjectRecorder _gameObjectRecorder = new GameObjectRecorder();

        /// <summary>
        /// 进度
        /// </summary>
        public float taskProgress { get => _taskProgress; private set => _taskProgress = value; }
        private float _taskProgress = 0;

        private int totalPartCount = 0;
        private int finishPartCount = 0;

        /// <summary>
        /// 重置状态
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!_gameObjectViewItemDataList)
            {
                _gameObjectViewItemDataList = FindObjectOfType<GameObjectViewItemDataList>();
            }
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            if (DataValidity())
            {
                // 查找所有拆装步骤及关联零件和模块
                InitStep();

                // 记录零件与模块的拆装状态，并设置为以拆卸
                InitDisassembly();

                // 创建零件组与零件视图
                CreatePartGroup();
                CreatePartViewItemDatas();
                CreatePartViewItem();

                // 设置零件关联游戏对象非激活状态
                InactiveParts();

                // 创建模块组, 因为组后面步骤动态创建的是图，因此需在零件视图创建之后
                CreateModuleGroup();

                // 增加零件拆装状态改变监听
                Part.onPartStateChanged += OnPartStateChanged;
                StepGroup.onStepActive += OnStepActive;
                StepGroup.onStepFinish += OnStepFinish;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            if (DataValidity())
            {
                // 移除零件拆装状态改变监听
                Part.onPartStateChanged -= OnPartStateChanged;
                StepGroup.onStepActive -= OnStepActive;
                StepGroup.onStepFinish -= OnStepFinish;

                // 恢复零件拆装状态
                repairTaskWork.parts.ForEach(p => p.RecoverPartState());
                repairTaskWork.modules.ForEach(p => p.RecoverPartState());

                // 移除所有零件视图
                DestroyPartViewItem();

                // 还原所有零件游戏对象激活状态
                _gameObjectRecorder.Recover();

                // 清除所有组字典
                groupCache.Clear();
            }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _gameObjectViewItemDataList;
        }

        #region 事件回调

        /// <summary>
        /// 零件状态回调
        /// </summary>
        /// <param name="part"></param>
        /// <param name="oldState"></param>
        private void OnPartStateChanged(Part part, EPartState oldState)
        {
            if (!repairTaskWork.parts.Contains(part) && !repairTaskWork.modules.Contains(part))
            {
                return;
            }

            ++finishPartCount;
            taskProgress = 1.0f * finishPartCount / totalPartCount; // 这里不需要判断除0错误

            // 判断零件完成后修改ViewItemData计数
            if (part.state == EPartState.Assembly)
            {
                foreach (var d in _viewItemDatas)
                {
                    d.OnPartAssembly(part);
                }
            }

            // 构成模块的所有子零件都完成了装配，模块本身未装配，则生成零件视图
            foreach (var m in repairTaskWork.modules)
            {
                if (m.itemListOnInit.Contains(part) && m.childrenState == EPartState.Assembly && m.state == EPartState.Disassembly)
                {
                    // 生成模块贴图
                    if (groupCache.memberGroupMap.TryGetValue(m, out ISingleGroup group))
                    {
                        var data = _viewItemDatas.Find(d => d.group == group);
                        if (data != null)
                        {
                            ++data.count;
                            return;
                        }
                    }
                    if (_inActiveDisassemblyPart)
                    {
                        m.gameObject.SetActive(false);
                    }
                    AddPartViewItem(new PartViewItemData(m, group));
                    break;
                }
            }
        }

        private void OnStepActive(StepGroup stepGroup, Step step) => SetPartViewItemDataEnable(stepGroup, step, true);

        private void OnStepFinish(StepGroup stepGroup, Step step) => SetPartViewItemDataEnable(stepGroup, step, false);

        private void SetPartViewItemDataEnable(StepGroup stepGroup, Step step, bool enable)
        {
            if (stepGroup != repairTaskWork || !_repairSteps.Exists(p => p.repairStep == step))
            {
                return;
            }

            var repairStep = step as RepairStep;
            foreach (var part in repairStep.selectedParts)
            {
                _viewItemDatas.ForEach(d =>
                {
                    // 同零件或者同组
                    if (d.part == part ||
                        (d.group != null && groupCache.memberGroupMap.TryGetValue(part, out ISingleGroup group) && d.group == group))
                    {
                        d.enable = enable;
                    }
                });
            }
        }

        #endregion
        
        #region 初始化步骤零件、模块和分组

        private void InitStep()
        {
            _repairSteps.Clear();

            foreach (var s in repairTaskWork.repairSteps)
            {
                var matchPositionStep = s.GetComponent<RepairStepByMatchPosition>();
                if (!matchPositionStep)
                {
                    continue;
                }
                _repairSteps.Add(matchPositionStep);
            }

            totalPartCount = repairTaskWork.parts.Count + repairTaskWork.modules.Count;
            finishPartCount = 0;
        }

        private void InitDisassembly()
        {
            repairTaskWork.parts.ForEach(p =>
            {
                p.RecordPartState();
                p.state = EPartState.Disassembly;
            });
            repairTaskWork.modules.ForEach(m =>
            {
                m.RecordPartState();
                m.state = EPartState.Disassembly;
            });
        }

        /// <summary>
        /// 非激活零件
        /// </summary>
        private void InactiveParts()
        {
            if (_inActiveDisassemblyPart)
            {
                _gameObjectRecorder.Clear();
                repairTaskWork.parts.ForEach(p =>
                {
                    if (p.gameObject)
                    {
                        _gameObjectRecorder.Record(p.gameObject);
                        p.gameObject.SetActive(false);
                    }
                });
            }
        }

        private void CreatePartGroup()
        {
            groupCache.Clear();
            groupCache.Create(repairTaskWork.parts);
        }

        private void CreateModuleGroup()
        {
            groupCache.Create(repairTaskWork.modules);
        }

        #endregion

        #region 零件视图

        private void CreatePartViewItemDatas()
        {
            var itemList = new List<Item>();
            itemList.AddRange(repairTaskWork.parts);

            // 首先创建有组的零件
            foreach (var item in groupCache.groupMemberMap)
            {
                var groupItemList = item.Value;
                _viewItemDatas.Add(new PartViewItemData(groupItemList.ConvertAll<Part>(i => i as Part), item.Key, false));
                groupItemList.ForEach(p => itemList.Remove(p)); // 移除成组的零件
            }

            // 创建未编组的零件
            foreach (var item in itemList)
            {
                _viewItemDatas.Add(new PartViewItemData(item as Part));
            }
        }

        /// <summary>
        /// 创建零件视图
        /// </summary>
        private void CreatePartViewItem()
        {
            _gameObjectViewItemDataList.AddDatas(_viewItemDatas);
        }

        /// <summary>
        /// 添加零件视图
        /// </summary>
        /// <param name="partViewItemData"></param>
        private void AddPartViewItem(PartViewItemData partViewItemData)
        {
            _viewItemDatas.Add(partViewItemData);
            if (_gameObjectViewItemDataList)
            {
                _gameObjectViewItemDataList.AddData(partViewItemData);
            }
        }

        /// <summary>
        /// 移除零件款式图
        /// </summary>
        private void DestroyPartViewItem()
        {
            _gameObjectViewItemDataList.RemoveDatas(_viewItemDatas);
            _viewItemDatas.Clear();
        } 

        #endregion
    }

}
