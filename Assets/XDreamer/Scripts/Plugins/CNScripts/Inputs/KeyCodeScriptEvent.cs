using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    /// <summary>
    /// 脚本KeyCode事件集合类
    /// </summary>
    [Serializable]
    public class KeyCodeScriptEventSet : BaseScriptEventSet<KeyCode>
    {
        //
    }

    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class KeyCodeScriptEvent : BaseScriptEvent<KeyCode, KeyCodeScriptEventSet>
    {
        public const string Title = "键码脚本事件";

        public override void UpdateFunction()
        {
            //Debug.Log("ScriptKeyCodeEvent.UpdateEventListToDictionary");
            //base.UpdateEventListToDictionary();
            eventDictionary.Clear();
            foreach (var tset in functionList)
            {
                //if (eventDictionary.ContainsKey(tset.type)) Debug.Log(tset.type.ToString() + " 重复了!");
                switch (tset.type)
                {
                    case KeyCode.LeftApple:
                    case KeyCode.RightApple:
                        {
                            break;
                        }
                    default:
                        {
                            eventDictionary[tset.type] = tset;
                            break;
                        }
                }
            }
        }

        public override void Update()
        {
            //float begin = Time.realtimeSinceStartup; DateTime dtb = DateTime.Now;            
            //foreach (var e in Enum.GetValues(typeof(KeyCode)))
            if (Input.anyKey || Input.anyKeyDown)
            {
                foreach (var kv in eventDictionary)
                {
                    KeyCode kc = kv.Key;
                    if (Input.GetKeyDown(kc))
                    {
                        RunFunction(kv.Value, "按键按下");
                    }
                    else if (Input.GetKey(kc))
                    {
                        RunFunction(kv.Value, "按键按下中");
                    }
                    else if (Input.GetKeyUp(kc))//按键弹起动作与按键按下中动作不可能同一frame内操作，所以加个else~
                    {
                        RunFunction(kv.Value, "按键弹起");
                    }
                }
            }
            //Debug.Log("Update 耗时:" + (Time.realtimeSinceStartup - begin).ToString() + " , DateTime: " + (DateTime.Now - dtb).TotalMilliseconds);
            //base.Update();
        }
    }
}
