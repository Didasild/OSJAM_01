using Dida.Rendering;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

public class SetPaletteToVolume : MonoBehaviour
{
    public ColorPaletteScriptable paletteToAppply;
    public Volume volume;
    public VolumeProfile profile;
    public VisualSettings _visualSettings;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume = GetComponent<Volume>();
        profile = volume.profile;
    }

    public void ApplyPaletteToVolume()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
