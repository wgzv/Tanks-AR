using System.Collections.Generic;
using UnityEngine;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Recorders
{
    public class RendererRecorder : Recorder<Renderer, RendererRecorder.Info>, IRecord<GameObject>, IRecord<IEnumerable<GameObject>>
    {
        public override void Record(Renderer obj)
        {
            if (!obj)
            {
                //Debug.Log("无效Renderer");
                return;
            }
            base.Record(obj);
        }

        public void Record(GameObject gameObject)
        {
            if(gameObject) Record(gameObject.GetComponent<Renderer>());
        }

        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        public void Record(IEnumerable<Transform> transforms)
        {
            if (transforms == null) return;
            foreach (var t in transforms)
            {
                Record(t.gameObject);
            }
        }

        public class Info : ISingleRecord<Renderer>
        {
            public Renderer renderer;

            public bool enabled;

            public uint renderingLayerMask;

            public List<Material> materials = new List<Material>();

            public List<Color> colors = new List<Color>();
            
            public void Record(Renderer renderer)
            {
                this.renderer = renderer;
                if (renderer)
                {
                    this.enabled = renderer.enabled;
                    this.renderingLayerMask = renderer.renderingLayerMask;
                    foreach (var mat in renderer.materials)
                    {
                        materials.Add(mat);
                        colors.Add(mat.color);
                    }
                }
            }

            public void Recover()
            {
                RecoverEnabled();
                RecoverRenderingLayerMask();
                RecoverMaterial();
                RecoverColor();
            }

            public void RecoverEnabled()
            {
                if (renderer)
                {
                    renderer.enabled = enabled;
                }
            }

            public void RecoverRenderingLayerMask()
            {
                if (renderer)
                {
                    renderer.renderingLayerMask = renderingLayerMask;
                }
            }

            public void RecoverMaterial()
            {
                if (renderer)
                {
                    renderer.materials = materials.ToArray();
                }
            }

            public void RecoverColor()
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        renderer.materials[i].color = colors[i];
                    }
                }
            }

            public void SetPercent(Percent percent, Color color)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        renderer.materials[i].color = Color.Lerp(colors[i], color, percent.percent01OfWorkCurve);
                    }
                }
            }

            public void SetColor(Color color)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        renderer.materials[i].color = color;
                    }
                }
            }

            public void SetPercent(Percent percent, float alpha)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        var color = colors[i];
                        var dstColor = new Color(color.r, color.g, color.b, alpha);
                        renderer.materials[i].color = Color.Lerp(color, dstColor, percent.percent01OfWorkCurve);
                    }
                }
            }

            public void SetAlpha(float alpha)
            {
                if (renderer)
                {
                    for (int i = 0; i < renderer.materials.Length; ++i)
                    {
                        var dstColor = colors[i];
                        dstColor.a = alpha;
                        renderer.materials[i].color = dstColor;
                    }
                }
            }

            public void SetMaterial(Material[] materials)
            {
                if (renderer)
                {
                    renderer.materials = materials;
                }
            }

            public void FillMaterialSize(Material[] materials)
            {
                if (!renderer) return;

                if (materials.Length == 0)
                {
                    renderer.materials = materials;
                }
                else
                {
                    if (renderer.materials.Length != materials.Length)
                    {
                        Material[] newMaterials = new Material[renderer.materials.Length];
                        int length = materials.Length;
                        for (int i = 0; i < renderer.materials.Length; ++i)
                        {
                            newMaterials[i] = (materials[i % length]);
                        }
                        materials = newMaterials;
                    }

                    renderer.materials = materials;
                }
            }

            public void SetEnabled(bool enabled)
            {
                if (renderer)
                {
                    renderer.enabled = enabled;
                }
            }

            public void SetEnabled(EBool enabled)
            {
                if (renderer)
                {
                    renderer.enabled = CommonFun.BoolChange(renderer.enabled, enabled);
                }
            }
        }
    }

    /// <summary>
    /// 渲染层记录器：仅记录渲染层信息，提高记录和还原效率
    /// </summary>
    public class RendererRenderingLayerMaskRecorder : Recorder<Renderer, RendererRenderingLayerMaskRecorder.Info>
    {
        public class Info : ISingleRecord<Renderer>
        {
            public Renderer renderer;

            public uint renderingLayerMask;

            public void Record(Renderer renderer)
            {
                this.renderer = renderer;
                if (this.renderer)
                {
                    renderingLayerMask = this.renderer.renderingLayerMask;
                }
            }

            public void Recover()
            {
                if (renderer)
                {
                    renderer.renderingLayerMask = renderingLayerMask;
                }
            }
        }
    }
}
