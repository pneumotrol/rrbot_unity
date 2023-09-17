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
        ROSConnection.GetOrCreateInstance().Subscribe<Float64MultiArrayMsg>(topicName, SubscribeCommand);

        // get ArticulationBody from GameObject
        bodies = robot.GetComponentsInChildren<ArticulationBody>(true);
    }


    void SubscribeCommand(Float64MultiArrayMsg commands)
    {
        // TODO
        // - create control mode (torque, speed, position)
        if (bodies.Length != commands.data.Length)
        {
            Debug.Log("Expected " + topicName + " length: " + bodies.Length + ", but received: " + commands.data.Length);
            return;
        }

        // apply force/torque which from /commands message to Unity model joints
        for (int i = 0; i < bodies.Length; i++)
        {
            switch (bodies[i].jointType)
            {
                case ArticulationJointType.PrismaticJoint:
                case ArticulationJointType.RevoluteJoint:
                    bodies[i].jointForce = new ArticulationReducedSpace((float)commands.data[i]);

                    break;

                case ArticulationJointType.FixedJoint:
                case ArticulationJointType.SphericalJoint:
                default:

                    break;
            }
        }
    }
}
