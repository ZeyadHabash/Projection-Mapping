using _Sandbox.Scripts.Hand;
using extOSC;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    public class OSCManager : MonoBehaviour
    {
        private OSCReceiver oscReceiver;
        [SerializeField] private HandBehaviour _rHand;
        [SerializeField] private HandBehaviour _lHand;

        private float rx;
        private float ry;
        private float lx;
        private float ly;

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
            // _rHand.SetPosition(rx, ry); //hands should reference manager instead... maybe
            // _lHand.SetPosition(lx, ly);
        }

        void Start() {
            oscReceiver.Bind("/p1/hand_r_closed", CloseRHand);
            oscReceiver.Bind("/p1/hand_r:tx", MoveHandRx);
            oscReceiver.Bind("/p1/hand_r:ty", MoveHandRy);
            oscReceiver.Bind("/p1/hand_l_closed", CloseLHand);
            oscReceiver.Bind("/p1/hand_l:tx", MoveHandLx);
            oscReceiver.Bind("/p1/hand_l:ty", MoveHandLy);
        }


        void CloseRHand(OSCMessage message) {
            _rHand.SetState(message.Values[0].FloatValue); //should emit event... probably
        }

        void CloseLHand(OSCMessage message) {
            _lHand.SetState(message.Values[0].FloatValue);
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