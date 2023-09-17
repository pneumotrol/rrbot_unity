using System;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using JointStateMsg = RosMessageTypes.Sensor.JointStateMsg;

public class RosPublisher : MonoBehaviour
{
    [SerializeField]
    GameObject robot;

    public string topicName = "/joint_states";

    bool hasInit = false;
    ROSConnection ros;
    ArticulationBody[] bodies;
    JointStateMsg joint_states;

    void Start()
    {
        // start ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);

        // get ArticulationBody from GameObject
        bodies = robot.GetComponentsInChildren<ArticulationBody>(true);

        // initialize /joint_states message
        joint_states = new JointStateMsg();
        Array.Resize(ref joint_states.name, bodies.Length);
        Array.Resize(ref joint_states.position, bodies.Length);
        Array.Resize(ref joint_states.velocity, bodies.Length);
        Array.Resize(ref joint_states.effort, bodies.Length);

        hasInit = true;

        // publish current /joint_states message
        PublishState();
    }

    void FixedUpdate()
    {
        // publish current /joint_states message once per Fixed Timestep
        // The timestep can be changed from Edit -> Project Settings -> Time
        PublishState();
    }

    bool PublishState()
    {
        if (!hasInit)
        {
            return false;
        }

        // substitute current phisical states calculated by Unity into /joint_states message
        for (int i = 0; i < bodies.Length; i++)
        {
            joint_states.name[i] = bodies[i].name;

            switch (bodies[i].jointType)
            {
                case ArticulationJointType.PrismaticJoint:
                case ArticulationJointType.RevoluteJoint:
                    joint_states.position[i] = bodies[i].jointPosition[0];
                    joint_states.velocity[i] = bodies[i].jointVelocity[0];
                    joint_states.effort[i] = bodies[i].jointForce[0];

                    break;

                case ArticulationJointType.FixedJoint:
                case ArticulationJointType.SphericalJoint:
                default:
                    joint_states.position[i] = 0.0;
                    joint_states.velocity[i] = 0.0;
                    joint_states.effort[i] = 0.0;

                    break;
            }
        }

        ros.Publish(topicName, joint_states);

        return true;
    }
}
