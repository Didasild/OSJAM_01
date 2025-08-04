using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dida.Rendering
{
    [Serializable, VolumeComponentMenu("Visual Settings/FullScreen/Glitchs")]
    public sealed class FullScreenGlitchSettings : VolumeComponent
    {
        public FloatParameter glitchIntensity = new ClampedFloatParameter(0f, 0f, 1.1f);
    }
}
