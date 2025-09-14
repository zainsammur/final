using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRCameraZoom : MonoBehaviour
{
    [Header("XR Origin References")]
    public GameObject xrOrigin;
    public Camera mainCamera;

    [Header("Zoom & Scale Settings")]
    public Transform targetObject;
    public float desiredDistanceFromCamera = 0.5f;
    public Vector3 targetFinalScale = new Vector3(1f, 1f, 1f);
    public float zoomSpeed = 2.0f;

    [Header("Grab Trigger Settings")]
    public XRGrabInteractable grabTriggerObject;
    public float zoomDelayAfterGrab = 4.0f;

    private Coroutine _zoomCoroutine;
    private Coroutine _delayCoroutine;
    private Vector3 _originalTargetScale;
    private bool _isZoomed = false;

    void OnEnable()
    {
        if (grabTriggerObject != null)
        {
            grabTriggerObject.selectEntered.AddListener(OnGrabStarted);
            grabTriggerObject.selectExited.AddListener(OnGrabEnded);
        }
    }

    void OnDisable()
    {
        if (grabTriggerObject != null)
        {
            grabTriggerObject.selectEntered.RemoveListener(OnGrabStarted);
            grabTriggerObject.selectExited.RemoveListener(OnGrabEnded);
        }
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        if (_isZoomed) return;

        if (targetObject != null)
        {
            _originalTargetScale = targetObject.localScale;
        }

        if (_delayCoroutine != null)
            StopCoroutine(_delayCoroutine);

        _delayCoroutine = StartCoroutine(StartZoomAfterDelay(zoomDelayAfterGrab));
    }

    private void OnGrabEnded(SelectExitEventArgs args)
    {
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
            _delayCoroutine = null;
        }

        if (_isZoomed && targetObject != null)
        {
            StartCoroutine(RevertTargetScaleCoroutine());
        }
        _isZoomed = false;
    }

    private IEnumerator StartZoomAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartZoomToTarget();
        _delayCoroutine = null;
    }

    public void StartZoomToTarget()
    {
        if (xrOrigin == null || mainCamera == null || targetObject == null)
        {
            Debug.LogError("Required references are missing.");
            return;
        }

        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ProcessZoomCoroutine());
    }

    private IEnumerator RevertTargetScaleCoroutine()
    {
        if (targetObject == null) yield break;

        Vector3 startScale = targetObject.localScale;
        Vector3 endScale = _originalTargetScale;

        float duration = 1f / zoomSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            targetObject.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        targetObject.localScale = endScale;
    }

    private IEnumerator ProcessZoomCoroutine()
    {
        _isZoomed = true;

        Vector3 desiredTargetWorldPos = mainCamera.transform.position + mainCamera.transform.forward * desiredDistanceFromCamera;
        Vector3 offsetFromTargetToXROrigin = xrOrigin.transform.position - targetObject.position;
        Vector3 targetXrOriginPos = desiredTargetWorldPos + offsetFromTargetToXROrigin;
        Quaternion targetXrOriginRot = Quaternion.LookRotation(targetObject.position - targetXrOriginPos, Vector3.up);

        Vector3 startXrOriginPos = xrOrigin.transform.position;
        Quaternion startXrOriginRot = xrOrigin.transform.rotation;
        Vector3 startTargetScale = targetObject.localScale;

        float duration = 1f / zoomSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            xrOrigin.transform.position = Vector3.Lerp(startXrOriginPos, targetXrOriginPos, t);
            xrOrigin.transform.rotation = Quaternion.Slerp(startXrOriginRot, targetXrOriginRot, t);

            if (targetObject != null)
                targetObject.localScale = Vector3.Lerp(startTargetScale, targetFinalScale, t);

            yield return null;
        }

        xrOrigin.transform.position = targetXrOriginPos;
        xrOrigin.transform.rotation = targetXrOriginRot;
        if (targetObject != null)
            targetObject.localScale = targetFinalScale;

        _zoomCoroutine = null;
    }
}
