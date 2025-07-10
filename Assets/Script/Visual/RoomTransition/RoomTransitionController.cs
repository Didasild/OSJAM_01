using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class RoomTransitionController : MonoBehaviour
{
    public GameObject grid;
    public GameObject gridIndicatorParent;
    public GameObject roomParent;
    
    private TransformOffset _RoomParentOffsetScript;
    private bool _roomTransitionComplete;
    private float visualTransitionDuration;
    private List<TransformOffset> _gridIndicatorOffsetScript;
    private Material _gridMaterial;
    private VisualManager _visualManager;

    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        _gridMaterial = grid.GetComponent<Renderer>().material;
        _gridIndicatorOffsetScript = GetTransformOffsets(gridIndicatorParent);
        _RoomParentOffsetScript = roomParent.GetComponent<TransformOffset>();
        visualTransitionDuration = _visualManager.roomTransitionDuration;
    }
    
    private List<TransformOffset> GetTransformOffsets(GameObject gameObjectParent)
    {
        return new List<TransformOffset>(gameObjectParent.GetComponentsInChildren<TransformOffset>());
    }
    
    public void RoomOffsetTransition(Vector2Int roomDirection, RoomData nextRoom)
    {
        int roomXDirection = roomDirection.x * 3;
        int roomYDirection = roomDirection.y * 3;
        _roomTransitionComplete = false;
        
        //Animation de la room
        if (roomXDirection != 0)
        {
            DOFloat(() => _RoomParentOffsetScript.primaryOffSetValue, x => _RoomParentOffsetScript.primaryOffSetValue = x,
                    roomXDirection > 0 ? - 1f : 1f, visualTransitionDuration)
                .SetEase(Ease.Linear);
        }

        if (roomYDirection != 0)
        {
            DOFloat(() => _RoomParentOffsetScript.secondaryOffSetValue, x => _RoomParentOffsetScript.secondaryOffSetValue = x,
                    roomYDirection > 0 ? -1f : 1f, visualTransitionDuration)
                .SetEase(Ease.Linear);
        }
        
        //Animation des room indicator
        foreach (TransformOffset transformOffset in _gridIndicatorOffsetScript)
        {
            if (!transformOffset.verticalOffset)
            {
                AnimateRoomTransitionValue(roomXDirection, visualTransitionDuration / Mathf.Abs(roomXDirection),
                    value => transformOffset.primaryOffSetValue = value,
                    () => transformOffset.primaryOffSetValue = 0f);
            }
            else
            {
                AnimateRoomTransitionValue(roomYDirection, visualTransitionDuration / Mathf.Abs(roomYDirection),
                    value => transformOffset.primaryOffSetValue = value,
                    () => transformOffset.primaryOffSetValue = 0f);
            }
        }
        
        //Animation de la Grid
        AnimateRoomTransitionValue(-roomXDirection, visualTransitionDuration / Mathf.Abs(roomXDirection),
            value => _gridMaterial.SetFloat("_GridXOffset", value * 11f),
            () =>
            {
                _gridMaterial.SetFloat("_GridXOffset", 0);
                CompleteRoomTransition(nextRoom);
            });
        AnimateRoomTransitionValue(-roomYDirection, visualTransitionDuration / Mathf.Abs(roomYDirection),
            value => _gridMaterial.SetFloat("_GridYOffset", value * 11f),
            () =>
            {
                _gridMaterial.SetFloat("_GridYOffset", 0);
                CompleteRoomTransition(nextRoom);
            });
        
    }

    private Tween AnimateRoomTransitionValue(int targetValue, float duration, Action<float> onUpdate, Action onComplete = null)
    {
        if (targetValue == 0)
        {
            return null;
        }

        int absLoops = Mathf.Abs(targetValue);
        float finalTarget = targetValue > 0 ? 1f : -1f;
        Sequence seq = DOTween.Sequence();
        
        float animValue = 0f;

        // Tweens intermédiaires
        for (int i = 1; i < absLoops - 1; i++)
        {
            seq.Append(
                CreateTweenForValue(() => 0f, x => { animValue = x; onUpdate?.Invoke(x); }, finalTarget, duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { animValue = 0f; onUpdate?.Invoke(0f); })
            );
        }

        // Dernière tween avec un easing de sortie
        seq.Append(
            CreateTweenForValue(() => animValue, x => animValue = x, finalTarget, visualTransitionDuration * 2, onUpdate)
                .SetEase(Ease.OutBack)
        );

        seq.OnComplete(() =>
        {
            onUpdate?.Invoke(0f);
            onComplete?.Invoke();
        });

        return seq;
    }
    
    private static Tween CreateTweenForValue(DOGetter<float> getter, DOSetter<float> setter, float finalTarget, float duration, Action<float> onUpdate = null)
    {
        return DOTween.To(getter, x => { setter(x); onUpdate?.Invoke(x); }, finalTarget, duration);
    }
    
    private static Tweener DOFloat(DOGetter<float> getter, DOSetter<float> setter, float endValue, float duration)
    {
        return DOTween.To(getter, setter, endValue, duration);
    }

    private void CompleteRoomTransition(RoomData nextRoom)
    {
        if (_roomTransitionComplete)
        {
            return;
        }
        _roomTransitionComplete = true;
        
        _visualManager.UpdateRoomID(nextRoom);
        
        _RoomParentOffsetScript.ResetOffset();
        
        GameManager.Instance.FloorManager.ChangeRoomOut(nextRoom);
    }
}
