using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XCSJ.PluginPeripheralDevice
{
    public interface IInfo
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        string deviceName { get; }
    }

    public interface IInput
    {
        /// <summary>
        /// 按键是否触发，只要当前键按下状态未改变，一直为true
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        bool GetButton(string buttonName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        bool GetButtonDown(string buttonName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        bool GetButtonUp(string buttonName);

        /// <summary>
        /// 获取滑动轴的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        float GetAxis(string axisName);

        /// <summary>
        /// 获取滑动轴的值,不连续的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        float GetRawAxis(string axisName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetKey(string keyName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetKey(KeyCode key);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetKeyDown(string keyName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetKeyDown(KeyCode key);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetKeyUp(string keyName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetKeyUp(KeyCode key);

        /// <summary>
        /// 检测鼠标按键是否有操作
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool GetMouseButton(int button);

        /// <summary>
        /// 鼠标按键是否按下
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool GetMouseButtonDown(int button);

        /// <summary>
        /// 鼠标按键是否弹起
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool GetMouseButtonUp(int button);

        /// <summary>
        /// 获取按键值
        /// </summary>
        /// <param name="triggerName"></param>
        /// <returns></returns>
        float GetTriggerValue(string triggerName);
    }

    public interface ICheckRay
    {
        /// <summary>
        /// 射线检测
        /// </summary>
        /// <param name="position">碰撞点坐标</param>
        /// <param name="colliderObject">碰撞物体</param>
        /// <returns></returns>
        bool CheckRay(out Vector3 position, out GameObject colliderObject);
    }

    public interface IHapticPulse
    {
        /// <summary>
        /// 震动
        /// </summary>
        /// <param name="amplitude">振幅</param>
        /// <param name="duration">持续时间</param>
        void TriggerHapticPulse(float amplitude, float duration);
    }

    public interface IProcess
    {
        /// <summary>
        /// 处理函数
        /// </summary>
        void Process();
    }

    public interface IPointerEnter
    {
        void OnPointerEnter(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IPointerExit
    {
        void OnPointerExit(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IDrag
    {
        void OnDrag(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IDrop
    {
        void OnDrop(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IBeginDrag
    {
        void OnBeginDrag(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IEndDrag
    {
        void OnEndDrag(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IPointerDown
    {
        void OnPointerDown(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IPointerUp
    {
        void OnPointerUp(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IPointerClick
    {
        void OnPointerClick(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface ISelect
    {
        void OnSelect(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IDeselect
    {
        void OnDeselect(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IScroll
    {
        void OnScroll(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IMove
    {
        void OnMove(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IUpdateSelected
    {
        void OnUpdateSelected(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface IInitializePotentialDrag
    {
        void OnInitializePotentialDrag(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface ISubmit
    {
        void OnSubmit(BaseInputSource baseInputSource, BaseEventData eventData);
    }

    public interface ICancel
    {
        void OnCancel(BaseInputSource baseInputSource, BaseEventData eventData);
    }
}
