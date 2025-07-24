using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FullScreenFeedbackController : MonoBehaviour
{
    public GameObject fullScreenObject;
    public GameObject fullScreenUIObject;
    private Material _fullScreenMaterial;
    private Material _fullScreenUIMaterial;
    public Volume deathFeedbackVolume;
    private VisualManager _visualManager;

    private string _spikeDissolve;
    private string _spikeDensity;
    private string _spikePulse;
    private string _NS_Progression;
    private string _EW_Progression;
    
    private Tweener _currentWeightTween;
    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        _fullScreenMaterial = fullScreenObject.GetComponent<Renderer>().material;
        _fullScreenUIMaterial = fullScreenUIObject.GetComponent<Image>().material;
        
        _spikeDissolve = "_SpikeDissolve";
        _spikeDensity = "_SpikeDensity";
        _spikePulse = "_SpikePulse";
        
        _NS_Progression = "_NS_Progression";
        _EW_Progression = "_EW_Progression";
        
        fullScreenUIObject.SetActive(false);
    }
    
    public void HitFeedback()
    {
        _fullScreenMaterial.SetFloat(_spikeDissolve, 0);
        _visualManager.FadeProperty(_fullScreenMaterial, _spikeDissolve, 0.5f, 0.05f, 0f, Ease.OutBounce);
        _visualManager.FadeProperty(_fullScreenMaterial, _spikeDissolve, 0f, 1f, 0.05f);
    }

    public void LowLifeFeedback(bool enable)
    {
        _fullScreenMaterial.SetFloat(_spikePulse, enable ? 1 : 0);
    }

    public void DeathCloseScreenFeedback(float duration = 0.3f)
    {
        //VOLUME TRANSITION
        _currentWeightTween?.Kill();
        _currentWeightTween = VisualUtils.DOWeight(deathFeedbackVolume, 1f, duration)
            .SetEase(Ease.Linear);
        
        //SCREEN TRANSITION
        fullScreenUIObject.SetActive(true);
        _fullScreenUIMaterial.SetFloat(_NS_Progression, 0);
        _fullScreenUIMaterial.SetFloat(_EW_Progression, 0);
        _visualManager.FadeProperty(_fullScreenUIMaterial, _NS_Progression,0.5f, duration*2, 0f, Ease.InOutExpo);
        _visualManager.FadeProperty(_fullScreenUIMaterial, _EW_Progression, 0.5f, duration, duration, Ease.InCubic);
    }

    //VOIR POUR MERGE EN UNE SEULE FONCTION?
    public void RebootOpenFeedback(float duration = 0.3f)
    {
        //VOLUME TRANSITION
        _currentWeightTween?.Kill();
        _currentWeightTween = VisualUtils.DOWeight(deathFeedbackVolume, 0f, duration)
            .SetEase(Ease.Linear)
            .SetDelay(duration);
        
        //SCREEN TRANSITION
        fullScreenUIObject.SetActive(true);
        _fullScreenUIMaterial.SetFloat(_NS_Progression, 0.5f);
        _fullScreenUIMaterial.SetFloat(_EW_Progression, 0.5f);
        _visualManager.FadeProperty(_fullScreenUIMaterial, _EW_Progression, 0f, duration, 0f, Ease.OutCubic);
        _visualManager.FadeProperty(_fullScreenUIMaterial, _NS_Progression,0f, duration*2, 0f, Ease.InOutExpo);

        DOVirtual.DelayedCall(duration * 2, () =>
        {
            fullScreenUIObject.SetActive(false);
            _fullScreenUIMaterial.SetFloat(_NS_Progression, 0);
            _fullScreenUIMaterial.SetFloat(_EW_Progression, 0);
        });
    }
    
}
