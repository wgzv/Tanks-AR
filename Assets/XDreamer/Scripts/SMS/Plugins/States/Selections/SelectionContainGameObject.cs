using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Selections
{
    /// <summary>
    /// 选择集包含游戏对象:选择集包含游戏对象组件是用于判断游戏对象是否在选择集内的触发器。如果条件成立，则切换为完成态。
    /// </summary>
    [ComponentMenu("选择集/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(SelectionContainGameObject))]
    [Tip("选择集包含游戏对象组件是用于判断游戏对象是否在选择集内的触发器。如果条件成立，则切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33668)]
    [RequireComponent(typeof(GameObjectSet))]
    public class SelectionContainGameObject : Trigger<SelectionContainGameObject>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "选择集包含游戏对象";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("选择集", typeof(SMSManager))]
        [StateComponentMenu("选择集/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(SelectionContainGameObject))]
        [Tip("选择集包含游戏对象组件是用于判断游戏对象是否在选择集内的触发器。如果条件成立，则切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("规则")]
        [EnumPopup]
        public ESelectionRule rule = ESelectionRule.Contain;

        [Name("变化时检测")]
        public bool checkOnChanged = true;

        private GameObjectSet gameObjectSet = null;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InitGameObjectSet();

            Selection.selectionChanged += OnSelectionChanged;
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (!checkOnChanged)
            {
                Check();
            }
        }

        public override void OnExit(StateData data)
        {
            Selection.selectionChanged -= OnSelectionChanged;

            base.OnExit(data);
        }

        private void InitGameObjectSet()
        {
            if (!gameObjectSet)
            {
                gameObjectSet = GetComponent<GameObjectSet>();
            }   
        }

        /// <summary>
        /// 当选择集变更时回调
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected void OnSelectionChanged(GameObject[] oldSelections, bool flag) => Check();
        
        private void Check()
        {
            switch (rule)
            {
                case ESelectionRule.Contain:
                    {
                        finished = gameObjectSet.objects.All(s => Selection.selections.Contains(s));
                        break;
                    }
                case ESelectionRule.Equal:
                    {
                        if (Selection.selections.Length == gameObjectSet.objects.Count)
                        {
                            finished = gameObjectSet.objects.All(s => Selection.selections.Contains(s));
                        }
                        break;
                    }
                case ESelectionRule.NotContain:
                    {
                        finished = gameObjectSet.objects.All(s => !Selection.selections.Contains(s));
                        break;
                    }
                case ESelectionRule.Contained:
                    {
                        finished = Selection.selections.All(s => gameObjectSet.objects.Contains(s));
                        break;
                    }
                case ESelectionRule.Any:
                    {
                        finished = gameObjectSet.objects.Any(s => Selection.selections.Contains(s));
                        break;
                    }
                case ESelectionRule.None: finished = true; break;
                default: break;
            }
        }
    }

    /// <summary>
    /// 选择集规则
    /// </summary>
    public enum ESelectionRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 包含：如选择集中任意一个对象有在游戏对象集合中，则包含成立
        /// </summary>
        [Name("包含")]
        [Tip("游戏对象集合都在选择集中，则包含成立")]
        Contain = 1,

        /// <summary>
        /// 相等：如选择集中每一个对象对象都在游戏对象集合中且两集合中对象数目相同，则相等成立
        /// </summary>
        [Name("相等")]
        [Tip("选择集中每一个对象对象都在游戏对象集合中且两集合中对象数目相同，则相等成立")]
        Equal = 2,

        /// <summary>
        /// 不包含：如选择集中任意一个对象有均不在在游戏对象集合中，则不包含成立
        /// </summary>
        [Name("不包含")]
        [Tip("选择集中任意一个对象有均不在在游戏对象集合中，则不包含成立(集合不相交)")]
        NotContain = 3,

        /// <summary>
        /// 被包含：如游戏对象集合都在选择集中，则包含成立
        /// </summary>
        [Name("被包含")]
        [Tip("游戏对象集合都在选择集中，则被包含成立")]
        Contained = 5,

        /// <summary>
        /// 任意：选择集中任意一个对象在游戏对象集中，则任意成立
        /// </summary>
        [Name("任意")]
        [Tip("选择集中任意一个对象在游戏对象集中，则任意成立(集合相交)")]
        Any = 4,
    }
}

