using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Dida.Rendering
{
    [Serializable, VolumeComponentMenu("Visual Settings")]
    public sealed class VisualSettings : VolumeComponent
    {
        public ColorParameter Color1 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color2 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color3 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color4 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color5 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color6 = new ColorParameter(Color.black, false, false, true);
        
        public ColorParameter ExterColor = new(Color.black, false, true, true);
        
        //public ClampedFloatParameter ExterAlpha = new(0.5f, 0f, 1f);
    }
}