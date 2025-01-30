using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace Dida.Rendering
{
    public class ShaderGlobalVariablesFeature : ScriptableRendererFeature
    {
        private List<ScriptableRenderPass> _passes;

        public override void Create()
        {
            _passes = new List<ScriptableRenderPass>();
            _passes.Add(new GlobalColorsRenderPass());
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            foreach (ScriptableRenderPass pass in _passes)
            {
                renderer.EnqueuePass(pass);
            }
        }
    }
}