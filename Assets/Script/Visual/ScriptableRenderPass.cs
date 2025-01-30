using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dida.Rendering
{
    public class GlobalColorsRenderPass : ScriptableRenderPass
    {
        #region CACHED PROPERTIES IDs

        private static readonly int MY_SHADER_PROP = Shader.PropertyToID("_ExterAlpha");
        private static readonly int MY_SHADER_COLOR1 = Shader.PropertyToID("_Color1");
        private static readonly int MY_SHADER_COLOR2 = Shader.PropertyToID("_Color2");
        private static readonly int MY_SHADER_COLOR3 = Shader.PropertyToID("_Color3");
        private static readonly int MY_SHADER_COLOR4 = Shader.PropertyToID("_Color4");
        private static readonly int MY_SHADER_COLOR5 = Shader.PropertyToID("_Color5");

        #endregion CACHED PROPERTIES IDs

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            VisualSettings mySettings = VolumeManager.instance.stack.GetComponent<VisualSettings>();

            Shader.SetGlobalFloat(MY_SHADER_PROP, mySettings.ExterAlpha.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR1, mySettings.Color1.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR2, mySettings.Color2.value);
            Shader.SetGlobalVector(MY_SHADER_COLOR3, mySettings.Color3.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR4, mySettings.Color4.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR5, mySettings.Color5.value);
        }
    }
}