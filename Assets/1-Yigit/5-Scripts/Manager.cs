using Unity.MLAgents;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField] private Agent_Main agent;
    
    [SerializeField] private TextMeshPro timer;
    [SerializeField] private TextMeshPro counter;

    private void Update()
    {
        timer.text = agent.Timer.ToString("F2");
        counter.text = agent.Step.ToString();
    }
}
