using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Float64MultiArrayMsg = RosMessageTypes.Std.Float64MultiArrayMsg;
using System.Collections.Generic;
using Unity.Robotics.UrdfImporter;

public class RosSubscriber : MonoBehaviour
{
    [SerializeField]
    GameObject robot;

    public string topicName = "/commands";

    UrdfJoint[] joints;

    void Start()
    {
        // start ROS connection
        ROSConnection.GetOrCreateInstance().Subscribe<Float64MultiArrayMsg>(topicName, SubscribeCommand);

        // get revolute or prismatic joint as UrdfJoint from GameObject
        List<UrdfJoint> joints_list = new();
        joints_list.AddRange(robot.GetComponentsInChildren<UrdfJointRevolute>(true));
        joints_list.AddRange(robot.GetComponentsInChildren<UrdfJointPrismatic>(true));
        joints = joints_list.ToArray();
    }


    void SubscribeCommand(Float64MultiArrayMsg commands)
    {
        // TODO
        // - create control mode (torque, speed, position)
        if (joints.Length != commands.data.Length)
        {
            Debug.Log("Expected " + topicName + " length: " + joints.Length + ", but received: " + commands.data.Length);
            return;
        }

        // apply force/torque which from /commands message to Unity model joints
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].GetComponent<ArticulationBody>().jointForce = new ArticulationReducedSpace((float)commands.data[i]);
        }
    }
}
