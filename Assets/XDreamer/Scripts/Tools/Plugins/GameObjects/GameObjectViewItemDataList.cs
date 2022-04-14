using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.UI;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象视图列表
    /// </summary>
    [Name("游戏对象视图列表")]
    [RequireManager(typeof(ToolsManager))]
    public class GameObjectViewItemDataList : ViewItemDataCollectionMB
    {
        /// <summary>
        /// 渲染贴图相机
        /// </summary>
        [Name("渲染贴图相机")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RenderTextureCamera _renderTextureCamera;

        /// <summary>
        /// 渲染相机
        /// </summary>
        public Camera renderCamera => _renderTextureCamera ? _renderTextureCamera.renderCamera : (Camera.main ?? Camera.current);

        /// <summary>
        /// 图像尺寸
        /// </summary>
        [Name("图像尺寸")]
        public Vector2Int _size = new Vector2Int(GameObjectPhotograph.DefaultTextureWidth, GameObjectPhotograph.DefaultTextureHeight);

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        [Name("游戏对象列表")]
        public List<GameObjectViewItemData> _gameObjectList = new List<GameObjectViewItemData>();

        /// <summary>
        /// 数据集合
        /// </summary>
        public override IEnumerable<IViewItemData> datas => _gameObjectList.Cast<IViewItemData>();

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            if (!_renderTextureCamera)
            {
                _renderTextureCamera = FindObjectOfType<RenderTextureCamera>();
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="datas"></param>
        public override void AddDatas(IEnumerable<IViewItemData> datas)
        {
            base.AddDatas(datas);

            if (!renderCamera || _size == Vector2Int.zero) return;

            datas.Foreach(d =>
            {
                if (d is IGameObjectViewItemData goViewItemData)
                {
                    goViewItemData.RenderGameObjectView(renderCamera, _size);
                }
            });
        }

        private bool Contains(GameObject go)
        {
            return false;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(DelayLoad());
        }

        private IEnumerator DelayLoad()
        {
            yield return new WaitForSeconds(0.2f);
            Load();
        }

    }

}
