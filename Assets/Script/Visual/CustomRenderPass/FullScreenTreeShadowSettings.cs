using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dida.Rendering
{
    [Serializable, VolumeComponentMenu("Visual Settings/FullScreen/TreeShadows")]
    public sealed class FullScreenTreeShadowSettings : VolumeComponent
    {
        public FloatParameter shadowIntensity = new ClampedFloatParameter(0f, 0f, 1f);
    }
}