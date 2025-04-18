using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Dida.Rendering
{
    [Serializable, VolumeComponentMenu("Visual Settings/Grid")]
    public sealed class GridAmbianceSettings : VolumeComponent
    {
        public ClampedFloatParameter GridDistortion = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter GridGlitch = new ClampedFloatParameter(0f, 0f, 2f);
    }
}