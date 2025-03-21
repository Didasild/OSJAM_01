using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dida.Rendering
{
    public class GridRenderPass : ScriptableRenderPass
    {
        #region CACHED PROPERTIES IDs
        
        private static readonly int SHADER_DISTORSION_INTENSITY = Shader.PropertyToID("_DistorsionIntensity");
        private static readonly int SHADER_GRID_GLITCH_INTENSITY = Shader.PropertyToID("_GridGlitchIntensity");

        #endregion CACHED PROPERTIES IDs

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            GridAmbianceSettings gridAmbianceSettings = VolumeManager.instance.stack.GetComponent<GridAmbianceSettings>();
                
            Shader.SetGlobalFloat(SHADER_DISTORSION_INTENSITY, gridAmbianceSettings.GridDistortion.value);
            Shader.SetGlobalFloat(SHADER_GRID_GLITCH_INTENSITY, gridAmbianceSettings.GridGlitch.value);
        }
    }
}
