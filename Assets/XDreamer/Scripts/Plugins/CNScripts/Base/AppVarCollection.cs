using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// App变量集合
    /// </summary>
    [Name(Title)]
    [Tip("应用程序运行期、非运行期均有效的变量；即变量的作用域在整个应用程序安装后不论运行与否均有效；不会因应用程序重启、场景切换等情况导致失效；本变量会序列化保存在物理磁盘中；")]
    [Tool(CNScriptHelper.Title, nameof(ScriptManager))]
    [RequireComponent(typeof(ScriptManager))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class AppVarCollection : BaseVarCollection
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "App变量集合";

        /// <summary>
        /// 键：用于序列化存储的键名
        /// </summary>
        public const string Key = nameof(XDreamer) + "_" + nameof(AppVarCollection);

        /// <summary>
        /// 静态层级变量字典
        /// </summary>
        protected static VarDictionary _varDictionary = new VarDictionary();

        /// <summary>
        /// 层级变量字典
        /// </summary>
        public override VarDictionary varDictionary => _varDictionary;

        /// <summary>
        /// 保存规则
        /// </summary>
        [Name("保存规则")]
        [Tip("对App变量保存时会影响程序性能")]
        public enum ESaveRule
        {
            /// <summary>
            /// 无:不处理
            /// </summary>
            [Name("无")]
            [Tip("不处理")]
            None,

            /// <summary>
            /// 当任意修改:当变量发生任意修改时保存
            /// </summary>
            [Name("当任意修改")]
            [Tip("当变量发生任意修改时保存")]
            OnAnyChange,

            /// <summary>
            /// 当修改计数:当变量发生修改的次数超过指定值时保存
            /// </summary>
            [Name("当修改计数")]
            [Tip("当变量发生修改的次数超过指定值时保存")]
            OnChangeCount,

            /// <summary>
            /// 当延后更新:当延后更新时保存
            /// </summary>
            [Name("当延后更新")]
            [Tip("当延后更新时保存")]
            OnLateUpdate,

            /// <summary>
            /// 当禁用:当对象禁用时保存
            /// </summary>
            [Name("当禁用")]
            [Tip("当对象禁用时保存")]
            OnDisable,

            /// <summary>
            /// 当销毁:当对象销毁时保存
            /// </summary>
            [Name("当销毁")]
            [Tip("当对象销毁时保存")]
            OnDestroy,

            /// <summary>
            /// 定时的:每过指定时间定时保存一次
            /// </summary>
            [Name("定时的")]
            [Tip("每过指定时间定时保存一次")]
            Timed,

            /// <summary>
            /// 综合的:在综合考量修改计数、定时、对象禁用等情况下的保存机制
            /// </summary>
            [Name("综合的")]
            [Tip("在综合考量修改计数、定时、对象禁用等情况下的保存机制")]
            Overall,
        }

        /// <summary>
        /// 保存规则
        /// </summary>
        [Name("保存规则")]
        [EnumPopup]
        public ESaveRule _saveRule = ESaveRule.Overall;

        /// <summary>
        /// 修改计数:当变量发生修改的次数超过指定值时保存
        /// </summary>
        [Name("修改计数")]
        [Tip("当变量发生修改的次数超过指定值时保存")]
        public int _changeCount = 10;

        private int counted = 0;

        /// <summary>
        /// 保存时间间隔:定时保存App变量的时间间隔；单位：秒；
        /// </summary>
        [Name("保存时间间隔")]
        [Tip("定时保存App变量的时间间隔；单位：秒；")]
        [Range(0.01f, 300)]
        public float _saveInteval = 3;

        private float passingTime = 0;

        /// <summary>
        /// App变量列表
        /// </summary>
        [Name("App变量列表")]
        [Tip("应用程序运行期、非运行期均有效的变量；即变量的作用域在整个应用程序安装后不论运行与否均有效；不会因应用程序重启、场景切换等情况导致失效；本变量会序列化保存在物理磁盘中；")]
        public List<Variable> _variableList = new List<Variable>();

        /// <summary>
        /// 变量作用域
        /// </summary>
        public override EVarScope varScope => EVarScope.App;

        /// <summary>
        /// 变量列表
        /// </summary>
        public override List<Variable> variableList => _variableList;

        private bool SaveIfTimed()
        {
            passingTime += Time.deltaTime;
            if (passingTime >= _saveInteval)
            {
                Save();
                return true;
            }
            return false;
        }

        private bool SaveIfCountd()
        {
            if (counted >= _changeCount)
            {
                Save();
                return true;
            }
            return false;
        }

        private void Save(ESaveRule saveRule)
        {
            if (saveRule == this._saveRule) Save();
        }

        private void Save()
        {
            try
            {
                counted = 0;
                passingTime = 0;

                var appVariables = _varDictionary.Values.ToList();
                PlayerPrefs.SetString(Key, JsonHelper.ToJson(appVariables));
                PlayerPrefs.Save();

                //Debug.Log("AppVarCollection Save Value: " + JsonHelper.ToJson(appVariables));
            }
            //catch (Exception ex) { Debug.LogException(ex); }
            catch { }
        }

        private static bool loaded = false;

        private void Load()
        {
            //Debug.Log("AppVarCollection Load 0");
#if UNITY_EDITOR
            if (loaded && Application.isPlaying) return;
#else
            if (loaded) return;
#endif
            //Debug.Log("AppVarCollection Load 1");
            try
            {
                var value = PlayerPrefs.GetString(Key);
                //Debug.Log("AppVarCollection Load Value: " + value);
                var appVariables = JsonHelper.ToObject<List<Variable>>(value);
                foreach (var variable in appVariables)
                {
                    _variableList.RemoveAll(v =>
                    {
                        if (v.name == variable.name)
                        {
                            v.MarkDirty();
                            return true;
                        }
                        return false;
                    });
                    _variableList.Add(variable);
                    _varDictionary[variable.name] = variable;
                }
            }
            //catch (Exception ex) { Debug.LogException(ex); }
            catch { }
            finally
            {
                loaded = true;
            }
        }

        private void OnHierarchyVarChanged(HierarchyVar hierarchyVar)
        {
            if (hierarchyVar.varScope == varScope)
            {
                counted++;
                Save(ESaveRule.OnAnyChange);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            Load();
            UpdateVariable();
            counted = 0;
            passingTime = 0;
            HierarchyVar.onChanged += OnHierarchyVarChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            HierarchyVar.onChanged -= OnHierarchyVarChanged;
            Save(ESaveRule.OnDisable);
            Save(ESaveRule.Overall);
        }

        private void Awake() => Load();

        private void OnDestroy() => Save(ESaveRule.OnDestroy);

        private void LateUpdate()
        {
            switch (_saveRule)
            {
                case ESaveRule.OnLateUpdate:
                    {
                        Save();
                        break;
                    }
                case ESaveRule.Timed:
                    {
                        SaveIfTimed();
                        break;
                    }
                case ESaveRule.OnChangeCount:
                    {
                        SaveIfCountd();
                        break;
                    }
                case ESaveRule.Overall:
                    {
                        if (SaveIfTimed() || SaveIfCountd()) { }
                        break;
                    }
            }
        }

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        public override void OnAfterDeserialize()
        {
            //base.OnAfterDeserialize();
        }
    }
}
