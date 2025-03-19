using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dida.Rendering
{
    public class GlobalColorRenderPass : ScriptableRenderPass
    {
        #region CACHED PROPERTIES IDs

        //private static readonly int MY_SHADER_PROP = Shader.PropertyToID("_ExterAlpha");
        private static readonly int MY_SHADER_COLOR_EXT = Shader.PropertyToID("_ExterColor");
        
        private static readonly int MY_SHADER_COLOR1 = Shader.PropertyToID("_Color1");
        private static readonly int MY_SHADER_COLOR2 = Shader.PropertyToID("_Color2");
        private static readonly int MY_SHADER_COLOR3 = Shader.PropertyToID("_Color3");
        private static readonly int MY_SHADER_COLOR4 = Shader.PropertyToID("_Color4");
        private static readonly int MY_SHADER_COLOR5 = Shader.PropertyToID("_Color5");
        private static readonly int MY_SHADER_COLOR6 = Shader.PropertyToID("_Color6");

        #endregion CACHED PROPERTIES IDs

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            GlobalColorSettings mainAmbianceSettings = VolumeManager.instance.stack.GetComponent<GlobalColorSettings>();
                
            Shader.SetGlobalColor(MY_SHADER_COLOR1, mainAmbianceSettings.Color1.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR2, mainAmbianceSettings.Color2.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR3, mainAmbianceSettings.Color3.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR4, mainAmbianceSettings.Color4.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR5, mainAmbianceSettings.Color5.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR6, mainAmbianceSettings.Color6.value);
            
            //Shader.SetGlobalFloat(MY_SHADER_PROP, mainAmbianceSettings.ExterAlpha.value);
            Shader.SetGlobalColor(MY_SHADER_COLOR_EXT, mainAmbianceSettings.ExterColor.value);
        }
    }
}