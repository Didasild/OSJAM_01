using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Dida.Rendering
{
    [Serializable, VolumeComponentMenu("Shaders/My Settings")]
    public sealed class MySettings : VolumeComponent
    {
        public ClampedFloatParameter ExterAlpha = new(0.5f, 0f, 1f);
        public ColorParameter Color1 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color2 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color3 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color4 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color5 = new ColorParameter(Color.black, false, false, true);
    }
    
}