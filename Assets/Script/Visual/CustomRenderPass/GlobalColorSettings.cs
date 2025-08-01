using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dida.Rendering
{
    [Serializable, VolumeComponentMenu("Visual Settings/Colors")]
    public sealed class GlobalColorSettings : VolumeComponent
    {
        public ColorParameter Color1 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color2 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color3 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color4 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color5 = new ColorParameter(Color.black, false, false, true);
        public ColorParameter Color6 = new ColorParameter(Color.black, false, false, true);
        public BoolParameter UseVariantTextIndex = new BoolParameter(false);
        
        public ColorParameter ExterColor = new(Color.black, false, true, true);
    }
}