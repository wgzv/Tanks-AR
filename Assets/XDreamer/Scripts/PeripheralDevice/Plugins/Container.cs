using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.PluginPeripheralDevice.Raycasters;

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// InputSource 容器
    /// </summary>
    [Serializable]
    public class InputSourceContainer
    {
        /// <summary>
        /// 输入源列表
        /// </summary>
        private List<BaseInputSource> _inputSources;

        /// <summary>
        /// 基础输入
        /// </summary>
        private BaseInput _baseInput;

        /// <summary>
        /// 添加输入源
        /// </summary>
        /// <param name="baseInputSource"></param>
        public void AddInputSource(BaseInputSource baseInputSource)
        {
            if (_inputSources.Exists(source => source == baseInputSource)) return;
            if (baseInputSource.baseInput != _baseInput) baseInputSource.baseInput = _baseInput;
            _inputSources.Add(baseInputSource);
        }

        /// <summary>
        /// 获取输入源列表
        /// </summary>
        /// <returns>输入源列表</returns>
        public List<BaseInputSource> GetInputSources()
        {
            return _inputSources;
        }

        /// <summary>
        /// 移除指定输入源
        /// </summary>
        /// <param name="baseInputSource">输入源</param>
        public void RemoveInputSource(BaseInputSource baseInputSource)
        {
            if (!_inputSources.Exists(source => source == baseInputSource)) return;
            _inputSources.Remove(baseInputSource);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseInput">基础输入</param>
        public InputSourceContainer(BaseInput baseInput)
        {
            _inputSources = new List<BaseInputSource>();
            _baseInput = baseInput;
        }
    }

    /// <summary>
    /// 射线容器
    /// 每个BaseInput对应一个RaycasterCantainer，RaycasterCantainer对应若干个BaseRaycaster
    /// BaseRaycaster可以处理基于相机的射线检测，或者基于空间的射线检测
    /// </summary>
    [Serializable]
    public class RaycasterContainer
    {
        /// <summary>
        /// 射线检测集
        /// </summary>
        private List<BaseRaycaster> _raycasters;

        /// <summary>
        /// 输入源
        /// </summary>
        private BaseInputSource _baseInputSource;

        /// <summary>
        /// 添加射线检测
        /// </summary>
        /// <param name="baseRaycaster">射线检测</param>
        public void AddRaycaster(BaseRaycaster baseRaycaster)
        {
            if (_raycasters.Exists(ray => ray == baseRaycaster)) return;
            if (baseRaycaster.baseInputSource != _baseInputSource) baseRaycaster.baseInputSource = _baseInputSource;
            _raycasters.Add(baseRaycaster);
        }

        /// <summary>
        /// 获取射线检测集
        /// </summary>
        /// <returns>射线检测集</returns>
        public List<BaseRaycaster> GetRaycasters()
        {
            return _raycasters;
        }

        /// <summary>
        /// 移除指定射线检测
        /// </summary>
        /// <param name="baseRaycaster">射线检测</param>
        public void RemoveRaycasters(BaseRaycaster baseRaycaster)
        {
            if (!_raycasters.Exists(ray => ray == baseRaycaster)) return;
            _raycasters.Remove(baseRaycaster);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseInputSource">输入源</param>
        public RaycasterContainer(BaseInputSource baseInputSource)
        {
            _raycasters = new List<BaseRaycaster>();
            _baseInputSource = baseInputSource;
        }
    }
}
