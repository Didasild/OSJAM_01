using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Dida.Rendering
{
    public class GridRenderPass : ScriptableRenderPass
    {
        #region CACHED PROPERTIES IDs

        //private static readonly int MY_SHADER_PROP = Shader.PropertyToID("_ExterAlpha");
        private static readonly int MY_SHADER_DISTORSION_INTENSITY = Shader.PropertyToID("_DistorsionIntensity");

        #endregion CACHED PROPERTIES IDs

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            GridAmbianceSettings gridAmbianceSettings = VolumeManager.instance.stack.GetComponent<GridAmbianceSettings>();
                
            Shader.SetGlobalFloat(MY_SHADER_DISTORSION_INTENSITY, gridAmbianceSettings.GridDistortion.value);
        }
    }
}
