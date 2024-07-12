using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private TextMeshPro counter;

    [SerializeField] private float duration;

    private void Update()
    {
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            counter.text = duration.ToString("F2");
        }
    }
}
