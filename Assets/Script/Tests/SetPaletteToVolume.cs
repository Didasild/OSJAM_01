using Dida.Rendering;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;
using DG.Tweening;

public class SetPaletteToVolume : MonoBehaviour
{
    [Expandable] public ColorPaletteScriptable paletteToApply;
    public Volume volume;
    public VolumeProfile profile;
    public VisualSettings _visualSettings;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume = GetComponent<Volume>();
        profile = volume.profile;
    }

    [Button]
    public void ApplyPaletteToVolume()
    {
        if (!profile.TryGet(out _visualSettings)) return;

        _visualSettings.Color1.value = paletteToApply.colors[0];
        _visualSettings.Color2.value = paletteToApply.colors[1];
        _visualSettings.Color3.value = paletteToApply.colors[2];
        _visualSettings.Color4.value = paletteToApply.colors[3];
        _visualSettings.Color5.value = paletteToApply.colors[4];
        _visualSettings.Color6.value = paletteToApply.colors[5];
    }
}
