using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Float64MultiArrayMsg = RosMessageTypes.Std.Float64MultiArrayMsg;

public class RosSubscriber : MonoBehaviour
{
    [SerializeField]
    GameObject robot;

    public string topicName = "/commands";

    ArticulationBody[] bodies;

    void Start()
    {
        // start ROS connection
        ROSConnection.GetOrCreateInstance().Subscribe<Float64MultiArrayMsg>(topicName, Callback);

        // get ArticulationBody from GameObject
        bodies = robot.GetComponentsInChildren<ArticulationBody>(true);
    }


    void Callback(Float64MultiArrayMsg commands)
    {
        // TODO
        // - add torque to body by messages
        // - create control mode (torque, speed, position)
        if (bodies.Length == commands.data.Length)
        {
            Debug.Log("Equal");
        }
        else
        {
            Debug.Log("Not equal" + commands.data.Length);
        }
    }
}
