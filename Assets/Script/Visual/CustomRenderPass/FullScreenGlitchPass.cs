using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dida.Rendering
{
    public class FullScreenGlitchPass : ScriptableRenderPass
    {
        #region CACHED PROPERTIES IDs
        
        private static readonly int SHADER_GLITCH_INTENSITY = Shader.PropertyToID("_GlitchIntensity");

        #endregion CACHED PROPERTIES IDs

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            FullScreenGlitchSettings fullScreenGlitchSettings = VolumeManager.instance.stack.GetComponent<FullScreenGlitchSettings>();
                
            Shader.SetGlobalFloat(SHADER_GLITCH_INTENSITY, fullScreenGlitchSettings.glitchIntensity.value);
        }
    }
}
