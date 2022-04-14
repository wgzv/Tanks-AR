using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    /// <summary>
    /// 基础变量集合检查器
    /// </summary>
    [CustomEditor(typeof(BaseVarCollection), true)]
    public class BaseVarCollectionInspector : BaseInspector<BaseVarCollection>
    {
        private const string VariableListName = "_" + nameof(BaseVarCollection.variableList);

        /// <summary>
        /// 可记录链表管理器
        /// </summary>
        public ReorderableListManager rlManager = new ReorderableListManager();

        private SerializedProperty scriptVarList = null;

        /// <summary>
        /// 变量名
        /// </summary>
        public string variableName = "";

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            rlManager.Init();
            if(target)
            {
                scriptVarList = serializedObject.FindProperty(VariableListName);
            }
        }

        /// <summary>
        /// 当检测是否需要绘制对象的成员时回调
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="memberProperty">成员属性对象</param>
        /// <param name="arrayElementInfo">数组元素信息对象</param>
        /// <returns>成员需要绘制返回True；否则返回False</returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case VariableListName:
                    {
                        UICommonFun.VariableListOnGUI(targetObject, serializedObject, targetObject.variableList, scriptVarList, ref variableName, CommonFun.NameTooltip(target.GetType(), VariableListName), rlManager);
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
