using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dida.Rendering
{
    [Serializable, VolumeComponentMenu("Visual Settings/Glitch")]
    public sealed class FullScreenSettings : VolumeComponent
    {
        public FloatParameter glitchIntensity = new ClampedFloatParameter(0f, 0f, 1.1f);
    }
}
