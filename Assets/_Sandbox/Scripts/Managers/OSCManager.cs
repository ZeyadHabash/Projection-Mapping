using _Sandbox.Scripts.Hand;
using _Sandbox.Scripts.IdentityCore;
using _Sandbox.Scripts.Utilities.Bases;
using extOSC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Sandbox.Scripts.Managers
{
    public class OSCManager : Singleton<OSCManager>
    {
        private OSCReceiver receiver;

        [Header("Body")]
        [SerializeField] private HandController rightHand;
        [SerializeField] private HandController leftHand;
        [SerializeField] private CoreMovement coreMovement;

        [Header("Keyboard Debug")]
        [SerializeField] private bool enableKeyboardMovement = true;
        [SerializeField] private float keyboardMoveSpeed = 5f;

        public HandController RightHand => rightHand;
        public HandController LeftHand => leftHand;
        public Transform RightHandTransform => rightHand.HandTransform;
        public Transform LeftHandTransform => leftHand.HandTransform;
        public bool RightHandClosed => rightHand.IsClosed;
        public bool LeftHandClosed => leftHand.IsClosed;

        protected override void Awake() {
            base.Awake();
            receiver = GetComponent<OSCReceiver>();
        }

        void Start()
        {
            receiver.Bind("/p1/hand_r_closed", OnRightClosed);
            receiver.Bind("/p1/hand_r:tx", OnRightX);
            receiver.Bind("/p1/hand_r:ty", OnRightY);

            receiver.Bind("/p1/hand_l_closed", OnLeftClosed);
            receiver.Bind("/p1/hand_l:tx", OnLeftX);
            receiver.Bind("/p1/hand_l:ty", OnLeftY);
            
            receiver.Bind("/p1/head:tx", OnHeadX);
            receiver.Bind("/p1/head:ty", OnHeadY);
        }
        
        void Update()
        {
            if (!enableKeyboardMovement)
                return;

            var keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            ApplyKeyboardClosedStates(keyboard);

            var leftInput = new Vector2(
                ReadAxis(keyboard.aKey.isPressed, keyboard.dKey.isPressed),
                ReadAxis(keyboard.sKey.isPressed, keyboard.wKey.isPressed));

            var rightInput = new Vector2(
                ReadAxis(keyboard.leftArrowKey.isPressed, keyboard.rightArrowKey.isPressed),
                ReadAxis(keyboard.downArrowKey.isPressed, keyboard.upArrowKey.isPressed));

            if (leftInput.sqrMagnitude >= Mathf.Epsilon ||
                rightInput.sqrMagnitude >= Mathf.Epsilon)
            {
                float delta = keyboardMoveSpeed * Time.deltaTime;

                leftHand.ApplyKeyboardDelta(leftInput * delta);
                rightHand.ApplyKeyboardDelta(rightInput * delta);
            }
        }

        #region OSC Forwarding

        void OnRightClosed(OSCMessage message)
        {
            float value = message.Values[0].FloatValue;
            rightHand.SetClosed(value);
        }
        
        private void OnHeadX(OSCMessage message) {
            if (coreMovement != null)
                coreMovement.SetRawX(message.Values[0].FloatValue);
        }

        private void OnHeadY(OSCMessage message) {
            if (coreMovement != null)
                coreMovement.SetRawY(message.Values[0].FloatValue);
        }

        void OnRightX(OSCMessage message)
        {
            rightHand.SetRawX(message.Values[0].FloatValue);
        }

        void OnRightY(OSCMessage message)
        {
            rightHand.SetRawY(message.Values[0].FloatValue);
        }

        void OnLeftClosed(OSCMessage message)
        {
            float value = message.Values[0].FloatValue;
            leftHand.SetClosed(value);
        }

        void OnLeftX(OSCMessage message)
        {
            leftHand.SetRawX(message.Values[0].FloatValue);
        }

        void OnLeftY(OSCMessage message)
        {
            leftHand.SetRawY(message.Values[0].FloatValue);
        }

        #endregion

        #region Keyboard

        public void ShowHands() {
            
        }

        private void ApplyKeyboardClosedStates(Keyboard keyboard)
        {
            bool rightClosed = keyboard.rightShiftKey.isPressed;
            bool leftClosed = keyboard.leftShiftKey.isPressed;

            rightHand.SetKeyboardClosed(rightClosed);
            leftHand.SetKeyboardClosed(leftClosed);
        }

        private static float ReadAxis(bool negative, bool positive)
        {
            if (negative == positive)
                return 0f;

            return positive ? 1f : -1f;
        }

        #endregion
    }
}
