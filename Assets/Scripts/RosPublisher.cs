using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using JointStateMsg = RosMessageTypes.Sensor.JointStateMsg;
using Unity.Robotics.UrdfImporter;
using UnityEditor;
using System;

public class RosPublisher : MonoBehaviour
{
    [SerializeField]
    GameObject robot;

    public string topicName = "/joint_states";
    public float publishMessageFrequency = 0.5f;

    ROSConnection ros;
    float timeElapsed = 0.0f;
    ArticulationBody[] bodies;
    JointStateMsg joint_states;

    void Start()
    {
        // start ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);

        // get ArticulationBody from GameObject
        bodies = robot.GetComponentsInChildren<ArticulationBody>(true);

        // initialize joint_states message
        joint_states = new JointStateMsg();
        Array.Resize(ref joint_states.name, bodies.Length);
        Array.Resize(ref joint_states.position, bodies.Length);
        Array.Resize(ref joint_states.velocity, bodies.Length);
        Array.Resize(ref joint_states.effort, bodies.Length);

        for (int i = 0; i < bodies.Length; i++)
        {
            joint_states.name[i] = bodies[i].name;
            joint_states.position[i] = (bodies[i].jointPosition.dofCount == 1) ? bodies[i].jointPosition[0] : 0.0;
            joint_states.velocity[i] = (bodies[i].jointVelocity.dofCount == 1) ? bodies[i].jointVelocity[0] : 0.0;
            joint_states.effort[i] = (bodies[i].jointForce.dofCount == 1) ? bodies[i].jointForce[0] : 0.0; ;
        }

        ros.Publish(topicName, joint_states);
    }

    // Update is called once per frame
    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            for (int i = 0; i < bodies.Length; i++)
            {
                joint_states.name[i] = bodies[i].name;
                joint_states.position[i] = (bodies[i].jointPosition.dofCount == 1) ? bodies[i].jointPosition[0] : 0.0;
                joint_states.velocity[i] = (bodies[i].jointVelocity.dofCount == 1) ? bodies[i].jointVelocity[0] : 0.0;
                joint_states.effort[i] = (bodies[i].jointForce.dofCount == 1) ? bodies[i].jointForce[0] : 0.0; ;
            }

            ros.Publish(topicName, joint_states);

            timeElapsed = 0.0f;
        }
    }
}
