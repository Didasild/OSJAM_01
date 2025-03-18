using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class FullScreenFeedbackController : MonoBehaviour
{
    public GameObject fullScreenObject;
    public Material _fullScreenMaterial;
    private VisualManager _visualManager;

    private string _spikeDissolve;
    private string _spikeDensity;
    
    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        _fullScreenMaterial = fullScreenObject.GetComponent<Renderer>().material;
        
        _spikeDissolve = "_SpikeDissolve";
        _spikeDensity = "_SpikeDensity";
    }

    [Button]
    public void HitFeedback()
    {
        Debug.Log("Hit Feedback");
        _fullScreenMaterial.SetFloat(_spikeDissolve, 0);
        _visualManager.FadeProperty(_fullScreenMaterial, _spikeDissolve, 0.5f, 0.05f, 0f, Ease.OutBounce);
        _visualManager.FadeProperty(_fullScreenMaterial, _spikeDissolve, 0f, 1f, 0.05f);
    }
}
