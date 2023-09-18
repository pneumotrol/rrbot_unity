using System;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.UrdfImporter;
using UnityEngine;
using JointStateMsg = RosMessageTypes.Sensor.JointStateMsg;

public class RosPublisher : MonoBehaviour
{
    [SerializeField]
    GameObject robot;

    public string topicName = "/joint_states";

    bool hasInit = false;
    ROSConnection ros;
    UrdfJoint[] joints;
    JointStateMsg joint_states;

    void Start()
    {
        // start ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);

        // get revolute or prismatic joint as UrdfJoint from GameObject
        List<UrdfJoint> joints_list = new();
        joints_list.AddRange(robot.GetComponentsInChildren<UrdfJointRevolute>(true));
        joints_list.AddRange(robot.GetComponentsInChildren<UrdfJointPrismatic>(true));
        joints = joints_list.ToArray();

        // initialize /joint_states message
        joint_states = new JointStateMsg();
        Array.Resize(ref joint_states.name, joints.Length);
        Array.Resize(ref joint_states.position, joints.Length);
        Array.Resize(ref joint_states.velocity, joints.Length);
        Array.Resize(ref joint_states.effort, joints.Length);

        hasInit = true;
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
        for (int i = 0; i < joints.Length; i++)
        {
            joint_states.name[i] = joints[i].jointName;
            joint_states.position[i] = joints[i].GetPosition();
            joint_states.velocity[i] = joints[i].GetVelocity();
            joint_states.effort[i] = joints[i].GetEffort();
        }

        ros.Publish(topicName, joint_states);

        return true;
    }
}
