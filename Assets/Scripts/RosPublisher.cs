using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using JointStateMsg = RosMessageTypes.Sensor.JointStateMsg;

public class RosPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "joint_states";
    public float publishMessageFrequency = 0.5f;

    private float timeElapsed = 0.0f;

    // start ROS connection
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);
    }

    // Update is called once per frame
    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            JointStateMsg joint_states= new JointStateMsg();

            ros.Publish(topicName, joint_states);

            timeElapsed = 0.0f;
        }
    }
}
