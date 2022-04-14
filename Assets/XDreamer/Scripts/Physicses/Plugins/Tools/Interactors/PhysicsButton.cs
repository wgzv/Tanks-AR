using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base.Limits;
using XCSJ.PluginTools;

namespace XCSJ.PluginPhysicses.Tools.Interactors
{
    /// <summary>
    /// ����ť��ʹ�ÿ���������ģ�ⰴť
    /// </summary>
    [Name("����ť")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    public class PhysicsButton : BasePhysicsMB
    {
        /// <summary>
        /// �����ýڵ��޶���
        /// </summary>
        [Name("�����ýڵ��޶���")]
        public ConfigurableJointLimiter _configurableJointLimiter = new ConfigurableJointLimiter();

        /// <summary>
        /// ���»ص�����
        /// </summary>
        [Name("����")]
        public UnityEvent _onPressed;

        /// <summary>
        /// ����ص�����
        /// </summary>
        [Name("����")]
        public UnityEvent _onReleased;

        /// <summary>
        /// �л�������
        /// </summary>
        protected SwitchCalculator switchCalculator
        {
            get
            {
                if (_switchCalculator == null)
                {
                    _switchCalculator = new SwitchCalculator(_configurableJointLimiter);
                }
                return _switchCalculator;
            }
        }
        private SwitchCalculator _switchCalculator = null;

        /// <summary>
        /// ����ť���»ص��¼�
        /// </summary>
        public static event Action<PhysicsButton, bool> onPressed;

        /// <summary>
        /// ����
        /// </summary>
        protected void Reset()
        {
            _configurableJointLimiter._configurableJoint = GetComponent<ConfigurableJoint>();
        }

        /// <summary>
        /// ����
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            SwitchCalculator.onValueChanged += OnToggleValueChanged;
        }

        /// <summary>
        /// ����
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            SwitchCalculator.onValueChanged -= OnToggleValueChanged;
        }

        /// <summary>
        /// ����
        /// </summary>
        protected void Update()
        {
            switchCalculator.Calculate();
        }

        private void OnToggleValueChanged(SwitchCalculator toggleLimiter, bool isOn)
        {
            if (this.switchCalculator == toggleLimiter)
            {
                onPressed?.Invoke(this, isOn);

                if (isOn)
                {
                    _onPressed.Invoke();
                }
                else
                {
                    _onReleased.Invoke();
                }
            }
        }
    }
}
