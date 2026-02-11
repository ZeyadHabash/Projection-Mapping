using _Sandbox.Scripts.Hand;
using extOSC;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    public class TestRagdollOSC : MonoBehaviour
    {
        private OSCReceiver oscReceiver;
        [SerializeField] private Transform _rHand;
        [SerializeField] private Transform _lHand;
        [SerializeField] private Transform _hip;
        [SerializeField] private Transform _head;

        private float rx;
        private float ry;
        private float lx;
        private float ly;
        private float hy;
        private float hx;
        private float hdx;

        private float rightClosedValue;
        private float leftClosedValue;

        private float handYOffset = 4.5f * 2;
        private static float UNITS_RATIO = 10;

        public bool RightHandClosed => rightClosedValue > 0.5f;
        public bool LeftHandClosed => leftClosedValue > 0.5f;

        void Awake() {
            oscReceiver = GetComponent<OSCReceiver>();
        }

        private void Update() {
            _rHand.position = new Vector3(rx, ry, 0);
            _lHand.position = new Vector3(lx, ly, 0);
            _hip.position = new Vector3(hx, hy, 0);
        }

        void Start() {
            oscReceiver.Bind("/p1/hand_r_closed", CloseRHand);
            oscReceiver.Bind("/p1/hand_r:tx", MoveHandRx);
            oscReceiver.Bind("/p1/hand_r:ty", MoveHandRy);
            oscReceiver.Bind("/p1/hand_l_closed", CloseLHand);
            oscReceiver.Bind("/p1/hand_l:tx", MoveHandLx);
            oscReceiver.Bind("/p1/hand_l:ty", MoveHandLy);
            oscReceiver.Bind("/p1/hip:ty", MoveHipY);
            oscReceiver.Bind("/p1/hip:tx", MoveHipX);
            oscReceiver.Bind("/p1/head:tx", MoveHeadX);
            oscReceiver.Bind("/p1/face:rx", RotateHeadX);
            oscReceiver.Bind("/p1/face:ry", RotateHeadY);
            oscReceiver.Bind("/p1/face:rz", RotateHeadZ);
            
        }

        private void RotateHeadZ(OSCMessage message) {
            // throw new System.NotImplementedException();
        }

        private void RotateHeadY(OSCMessage message) {
            // throw new System.NotImplementedException();
        }

        private void RotateHeadX(OSCMessage message) {
            // throw new System.NotImplementedException();
        }

        private void MoveHeadX(OSCMessage message) {
            hdx = message.Values[0].FloatValue * UNITS_RATIO;
        }

        private void MoveHipY(OSCMessage message) {
            hy = message.Values[0].FloatValue * UNITS_RATIO;
        }
        
        private void MoveHipX(OSCMessage message) {
            hx = message.Values[0].FloatValue * UNITS_RATIO;
        }

        void CloseRHand(OSCMessage message) {
            // _rHand.SetState(message.Values[0].FloatValue); //should emit event... probably
        }

        void CloseLHand(OSCMessage message) {
            // _lHand.SetState(message.Values[0].FloatValue);
        }

        void MoveHandRx(OSCMessage message) {
            rx = message.Values[0].FloatValue * UNITS_RATIO;
        }

        void MoveHandRy(OSCMessage message) {
            ry = message.Values[0].FloatValue * UNITS_RATIO + handYOffset;
        }

        void MoveHandLx(OSCMessage message) {
            lx = message.Values[0].FloatValue * UNITS_RATIO;
        }

        void MoveHandLy(OSCMessage message) {
            ly = message.Values[0].FloatValue * UNITS_RATIO + handYOffset;
        }
    }
}