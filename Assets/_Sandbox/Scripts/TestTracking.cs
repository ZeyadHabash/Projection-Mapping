using UnityEngine;
using extOSC;

public class TestTracking : MonoBehaviour
{
    private OSCReceiver temp;
    [SerializeField] private MeshRenderer r_rend;
    [SerializeField] private Transform r_cubeTransform;
    [SerializeField] private MeshRenderer l_rend;
    [SerializeField] private Transform l_cubeTransform;
    float rx;
    float ry;
    float lx;
    float ly;


    private float handYOffset = 4.5f * 2;
    private static float UNITS_RATIO = 10;
    
    void Awake()
    {
        temp = GetComponent<OSCReceiver>();
    }

    void Start() {
        temp.Bind("/p1/hand_r_closed", TestMethod);
        temp.Bind("/p1/hand_r:tx", MoveHandRx);
        temp.Bind("/p1/hand_r:ty", MoveHandRy);
        temp.Bind("/p1/hand_l_closed", TestLMethod);
        temp.Bind("/p1/hand_l:tx", MoveHandLx);
        temp.Bind("/p1/hand_l:ty", MoveHandLy);
        //p1/hand_r_closed p1/hand_r:tx p1/hand_r:ty
    }

    void TestMethod(OSCMessage message)
    {
        var temp = message.Values[0].FloatValue;
        Debug.Log($"--- blah {temp}");
        if (temp > 0) {
            r_rend.material.color = Color.red;
        } else
        {
            r_rend.material.color = Color.cyan;
        }
        r_cubeTransform.position = new Vector3(rx, ry, -0.5f);
    }

    void MoveHandRx(OSCMessage message)
    {
        rx = message.Values[0].FloatValue * UNITS_RATIO;
    }

    void MoveHandRy(OSCMessage message)
    {
        ry = message.Values[0].FloatValue * UNITS_RATIO + handYOffset;
    }
    
    void TestLMethod(OSCMessage message)
    {
        var temp = message.Values[0].FloatValue;
        Debug.Log($"--- blah {temp}");
        if (temp > 0) {
            l_rend.material.color = Color.red;
        } else
        {
            l_rend.material.color = Color.cyan;
        }
        l_cubeTransform.position = new Vector3(lx, ly, -0.5f);
    }

    void MoveHandLx(OSCMessage message)
    {
        lx = message.Values[0].FloatValue * UNITS_RATIO;
    }

    void MoveHandLy(OSCMessage message)
    {
        ly = message.Values[0].FloatValue * UNITS_RATIO + handYOffset;
    }
}