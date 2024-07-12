using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Agent_Main : Agent
{
    [SerializeField] private float speed;

    [SerializeField] private Transform target;

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
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(agentRigidbody.velocity);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity))
        {
            Debug.LogWarning(hit.distance);
            Debug.LogWarning(hit.collider.gameObject.name);

            sensor.AddObservation(hit.distance);
        }
        else
        {
            sensor.AddObservation(0);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        agentRigidbody.AddForce(controlSignal * speed);

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
