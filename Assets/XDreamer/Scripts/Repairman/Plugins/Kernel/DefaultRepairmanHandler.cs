﻿using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Exam;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginRepairman.CNScripts;
using XCSJ.PluginRepairman.Study;
using XCSJ.PluginRepairman.Task;
using XCSJ.PluginRepairman.UI;
using XCSJ.Scripts;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginRepairman.Kernel
{
    /// <summary>
    /// 默认拆装处理器
    /// </summary>
    public class DefaultRepairmanHandler : InstanceClass<DefaultRepairmanHandler>, IRepairmanHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            RepairmanHandler.handler = instance;
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public List<Script> GetScripts(RepairmanManager manager) => Script.GetScriptsOfEnum<EScriptID>(manager);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnValue RunScript(RepairmanManager manager, int id, ScriptParamList param)
        {
            switch ((EScriptID)id)
            {
                case EScriptID.SetItemSelectedMaxCount:
                    {
                        LimitedSelection.maxCount = (int)param[1];
                        return ReturnValue.Yes;
                    }
                case EScriptID.SetItemPickedEnable:
                    {
                        var system = manager.GetComponent<RepairmanInputSystem>();
                        if (!system) break;
                        system.enabled = ((EBool2)param[1]) == EBool2.Yes;
                        return ReturnValue.Yes;
                    }
                case EScriptID.ClearSelectedTool:
                    {
                        ToolSelection.Clear();
                        return ReturnValue.Yes;
                    }
                case EScriptID.SetDevicePartState:
                    {
                        Device device = param[1] as Device;
                        if (!device) break;

                        device.SetPartState((EPartState)param[2]);
                        return ReturnValue.Yes;
                    }
                case EScriptID.CreatePartList:
                    {
                        GUIPartList partList = param[1] as GUIPartList;
                        if (!partList) break;

                        partList.CreatePartList();
                        return ReturnValue.Yes;
                    }
                case EScriptID.GetRepairStepClickEnable:
                    {
                        return RepairStep.isOnClick ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.SetRepairStepClickEnable:
                    {
                        RepairStep.isOnClick = ((EBool2)param[1]) == EBool2.Yes;

                        return ReturnValue.Yes;
                    }
                case EScriptID.GetRepairStepAutoSelectPartAndTool:
                    {
                        return RepairStep.autoSelectPartAndTool ? ReturnValue.Yes : ReturnValue.No;
                    }
                case EScriptID.SetRepairStepAutoSelectPartAndTool:
                    {
                        RepairStep.autoSelectPartAndTool = ((EBool2)param[1]) == EBool2.Yes;

                        return ReturnValue.Yes;
                    }
                case EScriptID.OperateStudy:
                    {
                        RepairStudy repairStudy = param[1] as RepairStudy;
                        if (!repairStudy) break;

                        switch (param[2] as string)
                        {
                            case "提示":
                                {
                                    repairStudy.Help();
                                    return ReturnValue.Yes;
                                }
                            default: break;
                        }
                        break;
                    }
                case EScriptID.OperateExam:
                    {
                        RepairExam repairExam = param[1] as RepairExam;
                        if (!repairExam) break;

                        switch (param[2] as string)
                        {
                            case "回答问题":
                                {
                                    repairExam.Answer();
                                    return ReturnValue.Yes;
                                }
                            case "跳过":
                                {
                                    repairExam.Skip();
                                    return ReturnValue.Yes;
                                }
                            default: break;
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }
    }
}
