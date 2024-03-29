using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;

    private Collider2D _coll;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    private bool isEntering = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                // Stop the exit coroutine if it's currently running
                if (CameraManager.Instance.IsPanning)
                {
                    StopCoroutine(CameraManager.Instance._panCameraCoroutine);
                }

                isEntering = true;

                // Calculate the remaining distance to pan based on the current progress
                CinemachineFramingTransposer framingTransposer = CameraManager.Instance.currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                float currentPanProgress = framingTransposer.m_TrackedObjectOffset.y / customInspectorObjects.panDistance;
                float remainingDistance = customInspectorObjects.panDistance - (currentPanProgress * customInspectorObjects.panDistance);

                // Start the enter coroutine from the current progress
                CameraManager.Instance.PanCameraOnContact(remainingDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private bool isSwappingCamera = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

            if (customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                isSwappingCamera = true; // Set the flag to true when swapping cameras
                CameraManager.Instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }
            if (!isSwappingCamera && customInspectorObjects.panCameraOnContact)
            {
                // Stop the enter coroutine if it's currently running
                if (isEntering)
                {
                    StopCoroutine(CameraManager.Instance._panCameraCoroutine);
                }

                isEntering = false;

                // Calculate the current progress of the pan
                CinemachineFramingTransposer framingTransposer = CameraManager.Instance.currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                float currentPanProgress = framingTransposer.m_TrackedObjectOffset.y / customInspectorObjects.panDistance;

                // Calculate the remaining distance to pan based on the current progress
                float remainingDistance = customInspectorObjects.panDistance - (currentPanProgress * customInspectorObjects.panDistance);

                // Start the exit coroutine from the current progress
                CameraManager.Instance.PanCameraOnContact(remainingDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }


}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.3f;


}
public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}
[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;


    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (cameraControlTrigger.customInspectorObjects.swapCameras)
        {
            cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera On Left", cameraControlTrigger.customInspectorObjects.cameraOnLeft,
            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera On Right", cameraControlTrigger.customInspectorObjects.cameraOnRight,
            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                cameraControlTrigger.customInspectorObjects.panDirection);

            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
        
    }
}

