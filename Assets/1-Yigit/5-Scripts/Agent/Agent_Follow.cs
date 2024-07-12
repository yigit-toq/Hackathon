using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Agent_Follow : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform plane;

    [SerializeField] private float multiplier;

    private Rigidbody agentRigidbody;

    private void Awake()
    {
        agentRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y < 0)
        {
            agentRigidbody.angularVelocity = Vector3.zero;
            agentRigidbody.velocity = Vector3.zero;

            transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        float scale_x = plane.transform.localScale.x * 9;
        float scale_z = plane.transform.localScale.z * 9;

        target.localPosition = new Vector3(Random.value * scale_x - (scale_x / 2), 0.5f, Random.value * scale_z - (scale_z / 2));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(agentRigidbody.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        agentRigidbody.AddForce(controlSignal * multiplier);

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        if (distanceToTarget < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (transform.localPosition.y < 0)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
