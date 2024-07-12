using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Agent_Main : Agent
{
    public float Timer;

    public int Step;

    [SerializeField] private int duration;

    [SerializeField] private Transform plane;

    [Header("Jump")]

    [SerializeField] private bool isGround;

    [SerializeField] private float jumpForce;

    [Header("Raycast")]

    [SerializeField] private int hitLength;

    [SerializeField] private Transform outHit;

    [Header("Movement")]

    [SerializeField] private float speed;

    [SerializeField] private Transform target;

    private Rigidbody agentRigidbody;

    [SerializeField] private RayPerceptionSensorComponent3D rayPerception;

    public override void Initialize()
    {
        agentRigidbody = GetComponent<Rigidbody>();

        Timer = duration;

        Step = 0;
    }

    public override void OnEpisodeBegin()
    {
        agentRigidbody.angularVelocity = Vector3.zero;
        agentRigidbody.velocity = Vector3.zero;

        transform.localPosition = new Vector3(0, 0.5f, 0);

        float scale_x = plane.transform.localScale.x - 1;
        float scale_z = plane.transform.localScale.z - 1;

        switch (Step)
        {
            case 1000:
                Timer = duration / 2;
                break;
            case 2000:
                Timer = duration / 3;
                break;
            case 3000:
                Timer = duration / 4;
                break;
            default:
                Timer = duration;
                break;
        }

        Step++;

        target.localPosition = new Vector3(Random.value * scale_x - (scale_x / 2), -plane.localPosition.y, Random.value * scale_z - (scale_z / 2)) + plane.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(target.localPosition);

        var rayOutput = rayPerception.RaySensor.RayPerceptionOutput;
        foreach (var ray in rayOutput.RayOutputs)
        {
            sensor.AddObservation(ray.HitFraction);
            sensor.AddObservation(ray.HitTaggedObject);
        }

        //if (Physics.Raycast(outHit.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, hitLength))
        //{
        //    Debug.DrawRay(outHit.position, transform.TransformDirection(Vector3.forward) * hitLength, Color.green);

        //    sensor.AddObservation(hit.distance);
        //}
        //else
        //{
        //    Debug.DrawRay(outHit.position, transform.TransformDirection(Vector3.forward) * hitLength, Color.red);

        //    sensor.AddObservation(hitLength);
        //}

        sensor.AddObservation(isGround);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        //Debug.LogWarning(controlSignal);
        Vector3 move = speed * Time.deltaTime * transform.TransformDirection(controlSignal);
        agentRigidbody.MovePosition(agentRigidbody.position + move);
        //transform.Rotate(0f, controlSignal.x * speed, 0f, Space.Self);

        //if (isGround && actions.DiscreteActions[0] == 1)
        //{
        //    agentRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //}

        var rayOutput = rayPerception.RaySensor.RayPerceptionOutput;
        foreach (var ray in rayOutput.RayOutputs)
        {
            if (ray.HitTaggedObject && ray.HitGameObject.CompareTag("Trap") && ray.HitFraction < 0.2f)
            {
                float obstacleHeight = ray.HitGameObject.transform.localScale.y;
                float agentHeight = transform.localScale.y;

                if (isGround)
                {
                    if (isGround && obstacleHeight > agentHeight * 0.5f && agentRigidbody.velocity.y < 0.2f)
                    {
                        agentRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    }
                }
            }
            //if (ray.HitTaggedObject && ray.HitGameObject.CompareTag("Target") && ray.HitFraction < 0.2f)
            //{
            //    Debug.LogWarning((ray.HitFraction * -1) + 0.3f);
            //    SetReward((ray.HitFraction * -1) + 0.3f);
            //}
        }

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        if (distanceToTarget < 0.5f)
        {
            SetReward(2.0f);
            EndEpisode();
        }

        if (transform.localPosition.y < -0.5f)
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

        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = false;
        }
    }
}
