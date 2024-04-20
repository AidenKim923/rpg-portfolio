using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Minsung.Cam
{
    public class CameraHandler : MonoBehaviour
    {
        /**************************************************************
         * 
         *                  Field
         * 
         **************************************************************/

        #region Field
        [Header("Camera Setting")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;

        [SerializeField] private float rotationSpeed    = 5.0f;
        [SerializeField] private float minZoomDistance  = 2.0f;
        [SerializeField] private float maxZoomDistance  = 10.0f;

        private CustomAction m_input;

        #endregion

        /**************************************************************
         * 
         *                  Unity Event
         * 
         **************************************************************/

        #region Unity Event

        private void Awake()
        {
            m_input = new CustomAction();
            AssignInputs();
        }

        private void OnEnable()
        {
            m_input.Enable();
        }


        private void Start()
        {
        }

        public void LateUpdate()
        {
            if (target == null)
                return;


            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);
            transform.LookAt(target);
        }

        private void OnDisable()
        {
            m_input.Disable();
        }

        #endregion

        /**************************************************************
         * 
         *                  Private Method
         * 
         **************************************************************/

        #region Private Method
        private void AssignInputs()
        {
            m_input.Main.Display_Fov.performed += ctx=> UpdateFov(ctx);
        }

        private void UpdateFov(InputAction.CallbackContext _ctx)
        {
            Vector2 dir = _ctx.ReadValue<Vector2>().normalized;

            float delta = dir.y;

            Vector3 deltaPos = Vector3.one * delta;

            // 휠을 위로 올릴 때 확대하게 하기 위해 deltaPos에 -를 곱해준다.
            offset = offset + (-deltaPos);

            Vector3 minOffset = Vector3.one * minZoomDistance;
            Vector3 maxOffset = Vector3.one * maxZoomDistance;

            if (offset.magnitude < minOffset.magnitude)
            {
                offset = minOffset;
            }

            if (offset.magnitude > maxOffset.magnitude)
            {
                offset = maxOffset;
            }
        }
        #endregion

    }
}
