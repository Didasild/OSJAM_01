using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dida.Rendering
{
    public class FullScreenTreeShadowPass : ScriptableRenderPass
    {
        #region CACHED PROPERTIES IDs
        
        private static readonly int SHADER_SHADOWS_INTENSITY = Shader.PropertyToID("_TreeShadows");

        #endregion CACHED PROPERTIES IDs

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            FullScreenTreeShadowSettings fullScreenTreeShadowSettings = VolumeManager.instance.stack.GetComponent<FullScreenTreeShadowSettings>();
                
            Shader.SetGlobalFloat(SHADER_SHADOWS_INTENSITY, fullScreenTreeShadowSettings.shadowIntensity.value);
        }
    }
}
