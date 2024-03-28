using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static LeanTween;
using UnityEditor;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] allVirtualCameras;

    public CinemachineVirtualCamera currentCamera { get; private set; }
    private CinemachineFramingTransposer framingTransposer;

    [Header("Y Damping Settings for Player Jump/Fall:")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    [SerializeField] private float cameraLerpTime = 0.2f;
    [SerializeField] public float xOffset = 1.0f;
    public float playerFallSpeedTheshold = -30
;
    public bool isLerpingYDamping;
    public bool hasLerpedYDamping;

    private float normalYDamp;
    public Coroutine _panCameraCoroutine;
    private Vector2 _startingTrackedObjectOffset;

    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (var camera in allVirtualCameras)
        {
            if (camera.Follow == null)
            {
                Debug.LogWarning("Virtual camera '" + camera.name + "' does not have a follow target.");
            }
        }

        // Set the initial camera to the first camera in the list
        currentCamera = allVirtualCameras[0];
        framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        normalYDamp = framingTransposer.m_YDamping;
        _startingTrackedObjectOffset = framingTransposer.m_TrackedObjectOffset;
    }

    private void Start()
    {
        PlayerController.OnPlayerFlipped += HandlePlayerFlipped;

        Transform playerTransform = null;

        if (PlayerController.Instance.gameObject.activeSelf)
        {
            playerTransform = PlayerController.Instance.transform;
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        if (playerTransform != null)
        {
            for (int i = 0; i < allVirtualCameras.Length; i++)
            {
                allVirtualCameras[i].Follow = playerTransform;
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        PlayerController.OnPlayerFlipped -= HandlePlayerFlipped;
    }

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if (currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true;

            cameraFromLeft.enabled = false;

            currentCamera = cameraFromRight;

            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        else if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            cameraFromLeft.enabled = true;

            cameraFromRight.enabled = false;

            currentCamera = cameraFromLeft;

            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();  
        }
    }



    public IEnumerator LerpYDamping(bool _isPlayerFalling)
    {
        if (isLerpingYDamping)
        {
            yield break; // Don't start another tween if one is already running
        }

        isLerpingYDamping = true;
        float startYDamp = framingTransposer.m_YDamping;
        float endYDamp = _isPlayerFalling ? panAmount + startYDamp : normalYDamp;

        LeanTween.value(gameObject, startYDamp, endYDamp, panTime)
            .setOnUpdate((float val) =>
            {
                framingTransposer.m_YDamping = val;
            })
            .setOnComplete(() =>
            {
                isLerpingYDamping = false;
                hasLerpedYDamping = _isPlayerFalling;
            });

        // Wait for the LeanTween to complete
        while (isLerpingYDamping)
        {
            yield return null;
        }
    }


    void HandlePlayerFlipped(Vector3 targetOffset)
    {
        StartCoroutine(LerpCameraOffset(targetOffset));
    }

    private int lerpTween; // Store the integer identifier for the LeanTween animation

   public IEnumerator LerpCameraOffset(Vector3 targetOffset)
    {
        if (lerpTween != 0)
        {
            LeanTween.cancel(lerpTween); // Cancel the previous tween if it's still running
        }

        Vector3 currentOffset = framingTransposer.m_TrackedObjectOffset; // Get the current offset
        Vector3 startOffset = new Vector3(-currentOffset.x, currentOffset.y, currentOffset.z); // Start from the negative of the current x-axis offset

        // Use LeanTween to lerp the camera offset from the start offset to the target offset
        lerpTween = LeanTween.value(gameObject, startOffset, targetOffset, cameraLerpTime)
            .setEase(LeanTweenType.easeOutSine) // Use easeOutSine for a smooth deceleration
            .setOnUpdate((Vector3 val) =>
            {
                framingTransposer.m_TrackedObjectOffset = val;
            })
            .id;

        // Wait for the lerp to complete
        yield return new WaitForSeconds(cameraLerpTime);

        // Ensure the final offset is set after the lerp completes
        framingTransposer.m_TrackedObjectOffset = targetOffset;
    }



    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
            _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    [SerializeField] private bool isPanning = false;

    public bool IsPanning
    {
        get { return isPanning; }
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
{
    isPanning = true;
    Vector2 endPos = Vector2.zero;
    Vector2 startingPos = Vector2.zero;
    Vector2 currentPos = framingTransposer.m_TrackedObjectOffset; // Added to track current position

    if (!panToStartingPos)
    {
        switch (panDirection)
        {
            case PanDirection.Up:
                endPos = Vector2.up * panDistance;
                break;
            case PanDirection.Down:
                endPos = Vector2.down * panDistance;
                break;
            case PanDirection.Left:
                endPos = Vector2.left * panDistance;
                break;
            case PanDirection.Right:
                endPos = Vector2.right * panDistance;
                break;
            default:
                break;
        }

        startingPos = currentPos; // Changed to start from the current position
        endPos += startingPos;
    }
    else
    {
        startingPos = currentPos; // Changed to start from the current position
        endPos = new Vector2(8, 0); // Corrected end position calculation
    }

    float elapsedTime = 0f;
    while (elapsedTime < panTime)
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / panTime);
        Vector3 panLerp = Vector3.Lerp(startingPos, endPos, t);
        framingTransposer.m_TrackedObjectOffset = panLerp;

        yield return null;
    }

    // Ensure the final offset is set after the lerp completes
    framingTransposer.m_TrackedObjectOffset = endPos;

    if (panToStartingPos)
    {
        isPanning = true;
        // Wait until the camera pans back to its starting position
        while (Vector3.Distance(framingTransposer.m_TrackedObjectOffset, currentPos) > 8.56f)
        {
            yield return null;
        }
        isPanning = false;
    }
}



}


