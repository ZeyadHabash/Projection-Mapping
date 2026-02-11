using UnityEngine;
using UnityEngine.InputSystem;
using extOSC;

public class TestTracking : MonoBehaviour
{
    private OSCReceiver temp;
    [SerializeField] private MeshRenderer r_rend;
    [SerializeField] private Transform r_cubeTransform;
    [SerializeField] private MeshRenderer l_rend;
    [SerializeField] private Transform l_cubeTransform;
    [SerializeField] private bool enableKeyboardMovement = true;
    [SerializeField] private float keyboardMoveSpeed = 5f;

    private float rx;
    private float ry;
    private float lx;
    private float ly;

    private float rightClosedValue;
    private float leftClosedValue;

    private const float HandZ = -0.5f;
    private float handYOffset = 4.5f * 2;
    private static float UNITS_RATIO = 10;

    public Transform RightHandTransform => r_cubeTransform;
    public Transform LeftHandTransform => l_cubeTransform;
    public bool RightHandClosed => rightClosedValue > 0.5f;
    public bool LeftHandClosed => leftClosedValue > 0.5f;
    private float zOffset = -12.5f;

    void Awake()
    {
        temp = GetComponent<OSCReceiver>();
    }

    void Start()
    {
        temp.Bind("/p1/hand_r_closed", TestMethod);
        temp.Bind("/p1/hand_r:tx", MoveHandRx);
        temp.Bind("/p1/hand_r:ty", MoveHandRy);
        temp.Bind("/p1/hand_l_closed", TestLMethod);
        temp.Bind("/p1/hand_l:tx", MoveHandLx);
        temp.Bind("/p1/hand_l:ty", MoveHandLy);
        //p1/hand_r_closed p1/hand_r:tx p1/hand_r:ty
    }

    void Update()
    {
        if (!enableKeyboardMovement)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        ApplyKeyboardClosedStates(keyboard);

        var leftInput = new Vector2(
            ReadAxis(keyboard.aKey.isPressed, keyboard.dKey.isPressed),
            ReadAxis(keyboard.sKey.isPressed, keyboard.wKey.isPressed));
        var rightInput = new Vector2(
            ReadAxis(keyboard.leftArrowKey.isPressed, keyboard.rightArrowKey.isPressed),
            ReadAxis(keyboard.downArrowKey.isPressed, keyboard.upArrowKey.isPressed));

        if (leftInput.sqrMagnitude >= Mathf.Epsilon || rightInput.sqrMagnitude >= Mathf.Epsilon)
        {
            var delta = keyboardMoveSpeed * Time.deltaTime;
            ApplyKeyboardOffset(leftInput * delta, rightInput * delta);
        }
    }

    void TestMethod(OSCMessage message)
    {
        var temp = message.Values[0].FloatValue;
        Debug.Log($"--- blah {temp}");
        rightClosedValue = temp;
        SetHandState(r_rend, r_cubeTransform, temp, rx, ry);
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
        leftClosedValue = temp;
        SetHandState(l_rend, l_cubeTransform, temp, lx, ly);
    }

    void MoveHandLx(OSCMessage message)
    {
        lx = message.Values[0].FloatValue * UNITS_RATIO;
    }

    void MoveHandLy(OSCMessage message)
    {
        ly = message.Values[0].FloatValue * UNITS_RATIO + handYOffset;
    }

    private void SetHandState(MeshRenderer renderer, Transform cubeTransform, float closedValue, float x, float y)
    {
        renderer.material.color = closedValue > 0 ? Color.red : Color.cyan;
        cubeTransform.position = new Vector3(x, y, HandZ);
    }

    private void ApplyKeyboardOffset(Vector2 leftDelta, Vector2 rightDelta)
    {
        lx += leftDelta.x;
        ly += leftDelta.y;
        rx += rightDelta.x;
        ry += rightDelta.y;

        if (r_cubeTransform != null)
        {
            r_cubeTransform.position = new Vector3(rx, ry, HandZ);
        }

        if (l_cubeTransform != null)
        {
            l_cubeTransform.position = new Vector3(lx, ly, HandZ);
        }
    }

    private void ApplyKeyboardClosedStates(Keyboard keyboard)
    {
        var rightClosed = keyboard.rightShiftKey.isPressed ? 1f : 0f;
        var leftClosed = keyboard.leftShiftKey.isPressed ? 1f : 0f;

        rightClosedValue = rightClosed;
        leftClosedValue = leftClosed;

        SetHandState(r_rend, r_cubeTransform, rightClosed, rx, ry);
        SetHandState(l_rend, l_cubeTransform, leftClosed, lx, ly);
    }

    private static float ReadAxis(bool negative, bool positive)
    {
        if (negative == positive)
        {
            return 0f;
        }

        return positive ? 1f : -1f;
    }
}
