using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Spinning_Trap : MonoBehaviour
{
    [SerializeField] private Transform spin;

    [SerializeField] private float speed;
    [SerializeField] private float duration;

    private void Start()
    {
        spin.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
    }
}
