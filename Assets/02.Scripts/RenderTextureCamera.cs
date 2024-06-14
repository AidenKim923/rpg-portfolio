using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCamera : MonoBehaviour
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
    [SerializeField] private float rotationSpeed = 5.0f;

    public Camera camera;

    #endregion

    /**************************************************************
    * 
    *                  Unity Event
    * 
    **************************************************************/

    #region Unity Event

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        if (camera != null)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black; // 원하는 색상으로 변경
        }
        else
        {
            Debug.LogError("카메라가 할당되지 않았습니다.");
        }
    }


    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);
        transform.LookAt(target);
    }
    #endregion
}
