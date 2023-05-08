using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

using Random = UnityEngine.Random;

public class DroneController : Agent
{
    public float upForce;
    private float MovementForwardSpeed = 500f;
    private float tilteAmountForward = 0;
    private float tilteVelocityForward;

    private float wantedYRotation;
    private float currentYRotation;
    private float rotationAmount = 2.5f;
    private float rotaionYVelocoty;

    Rigidbody ourDrone;

    [SerializeField] private Transform targetTransform;


    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-90f, 80f), 0f, Random.Range(-120f,0f)); // 0, 0, -95);
        targetTransform.localPosition = new Vector3(Random.Range(-75f, 50f), 0f, Random.Range(-80f, -5f)); // 0, 0, -95);

    }



    public override void CollectObservations(VectorSensor sensor)
    {

        // The position of the agent
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(transform.localPosition.z);

        // The position of the treasure prefab
        sensor.AddObservation(targetTransform.localPosition.x);
        sensor.AddObservation(targetTransform.localPosition.y);
        sensor.AddObservation(targetTransform.localPosition.z);


        // The distance between the agent and the treasure
        sensor.AddObservation(Vector3.Distance(targetTransform.localPosition, transform.localPosition));

    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0] * 10;
        float moveY = actions.ContinuousActions[1] * 10;
        float moveZ = actions.ContinuousActions[2] * 10;
        //float upForce_up = upForce + actions.ContinuousActions[0]; ;
        //float upForce_down = upForce + actions.ContinuousActions[1]; ; 

        //ourDrone.AddRelativeForce(Vector3.up * upForce_up);

       // ourDrone.rotation = Quaternion.Euler(new Vector3(tilteAmountForward, currentYRotation, ourDrone.rotation.z));

        float moveSpeed = 10f;

        transform.localPosition += new Vector3(moveX, moveY, moveZ) * Time.deltaTime * moveSpeed;
        //tilteAmountForward = Mathf.SmoothDamp(tilteAmountForward, 20 * Input.GetAxis("Vertical"), ref tilteVelocityForward, 0.1f);

        AddReward(-0.01f);
    }

    public override void Heuristic (in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[2] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            EndEpisode();
        }

    }

   

    void MovmentForward()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * MovementForwardSpeed);
        }
    }

    void Rotation()
    {
        if (Input.GetKey(KeyCode.L))
        {
            wantedYRotation += rotationAmount;
        }
        if (Input.GetKey(KeyCode.J))
        {
            wantedYRotation -= rotationAmount;

        }
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotaionYVelocoty, 0.25f);

    }
}
