using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private TextMeshPro counter;

    [SerializeField] private float duration;

    [SerializeField] private float time;

    private void Start()
    {
        time = duration;
    }
    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            counter.text = time.ToString("F2");
        }
    }
}
