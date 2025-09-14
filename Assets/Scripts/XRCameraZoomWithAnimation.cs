using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRCameraZoomWithAnimation : MonoBehaviour
{
    [Header("XR References")]
    public GameObject xrOrigin;          // الـXR Origin في المشهد
    public Camera mainCamera;            // الكاميرا الرئيسية (عادة Child للXR Origin)

    [Header("Zoom & Scale Settings")]
    public Transform targetObject;       // الكائن المستهدف للزوم والتكبير
    public float desiredDistanceFromCamera = 0.5f;   // المسافة النهائية من الكاميرا
    public Vector3 targetFinalScale = new Vector3(1f, 1f, 1f); // المقياس النهائي للكائن
    public float zoomSpeed = 2.0f;       // سرعة الزوم والتحريك
    public float rotationSpeed = 2.0f;   // سرعة دوران XR Origin

    [Header("Animation Settings")]
    public Animator targetAnimator;      // الـAnimator للكائن المستهدف
    public string animationTriggerName;  // اسم Trigger أو Animation ليشتغل عند الجِراب

    [Header("Grab Settings")]
    public XRGrabInteractable grabTriggerObject; // الكائن اللي عمل جراب لتفعيل الزوم
    public float zoomDelayAfterGrab = 0f; // تأخير قبل الزوم إذا بدك

    private Coroutine _zoomCoroutine;
    private Coroutine _delayCoroutine;
    private Vector3 _originalTargetScale;
    private bool _isZoomed = false;

    void OnEnable()
    {
        if (grabTriggerObject != null)
        {
            grabTriggerObject.selectEntered.AddListener(OnGrabStarted);
        }
    }

    void OnDisable()
    {
        if (grabTriggerObject != null)
        {
            grabTriggerObject.selectEntered.RemoveListener(OnGrabStarted);
        }
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        if (_isZoomed) return;

        // حفظ المقياس الأصلي للكائن
        if (targetObject != null)
            _originalTargetScale = targetObject.localScale;

        // تشغيل الأنيميشن فوراً عند الجراب
        if (targetAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            targetAnimator.SetTrigger(animationTriggerName);
        }

        // بدء الزوم بعد التأخير (يمكن 0 ثانية)
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        _delayCoroutine = StartCoroutine(StartZoomAfterDelay(zoomDelayAfterGrab));
    }

    private IEnumerator StartZoomAfterDelay(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        StartZoomToTarget();
        _delayCoroutine = null;
    }

    public void StartZoomToTarget()
    {
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
        _zoomCoroutine = StartCoroutine(ProcessZoomCoroutine());
    }

    private IEnumerator ProcessZoomCoroutine()
    {
        _isZoomed = true;

        Vector3 startXrOriginPos = xrOrigin.transform.position;
        Quaternion startXrOriginRot = xrOrigin.transform.rotation;
        Vector3 startTargetScale = targetObject.localScale;

        // حساب الموضع النهائي لـ XR Origin بحيث يكون targetObject أمام الكاميرا بالمسافة المطلوبة
        Vector3 desiredTargetPos = mainCamera.transform.position + mainCamera.transform.forward * desiredDistanceFromCamera;
        Vector3 offset = xrOrigin.transform.position - targetObject.position;
        Vector3 endXrOriginPos = desiredTargetPos + offset;

        // الدوران النهائي للـXR Origin لينظر للهدف
        Quaternion endXrOriginRot = Quaternion.LookRotation(targetObject.position - endXrOriginPos, Vector3.up);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;

            xrOrigin.transform.position = Vector3.Lerp(startXrOriginPos, endXrOriginPos, t);
            xrOrigin.transform.rotation = Quaternion.Slerp(startXrOriginRot, endXrOriginRot, t);
            targetObject.localScale = Vector3.Lerp(startTargetScale, targetFinalScale, t);

            yield return null;
        }

        // التأكد من تعيين القيم النهائية
        xrOrigin.transform.position = endXrOriginPos;
        xrOrigin.transform.rotation = endXrOriginRot;
        targetObject.localScale = targetFinalScale;

        _zoomCoroutine = null;
    }
}
