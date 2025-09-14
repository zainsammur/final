using UnityEngine;
using UnityEngine.InputSystem; // نظام الإدخال الجديد
using UnityEngine.Animations;

public class CameraAnimationAfterTarget : MonoBehaviour
{
    [Header("Animation Check")]
    public Animator targetAnimator;         // الأنيميشن اللي نريد ننتظر إنه يخلص
    public string animationName;            // اسم الأنيميشن المحدد
    private bool animationFinished = false;

    [Header("Camera Settings")]
    public Camera mainCamera;               // الكاميرا الرئيسية
    public Transform zoomTarget;            // الكائن اللي نريد نعمل له زوم
    public float zoomDuration = 1f;         // مدة الزوم
    public float targetFOV = 30f;           // قيمة الزوم (FOV)

    [Header("Input Settings")]
    public InputActionReference xrButtonAction; // زر الـ XR Controller
    public Key keyboardKey = Key.A;              // زر الكيبورد

    private float originalFOV;
    private bool zooming = false;

    void Start()
    {
        if (mainCamera != null)
            originalFOV = mainCamera.fieldOfView;

        // فعل الـ XR Action
        if (xrButtonAction != null)
            xrButtonAction.action.Enable();
    }

    void Update()
    {
        // تحقق هل انتهى الأنيميشن المطلوب
        animationFinished = CheckAnimationFinished();

        // تحقق من ضغط زر الكيبورد أو زر XR
        bool inputPressed = Keyboard.current[keyboardKey].wasPressedThisFrame ||
                            (xrButtonAction != null && xrButtonAction.action.WasPressedThisFrame());

        if (animationFinished && inputPressed && !zooming)
        {
            StartCoroutine(ZoomCameraCoroutine());
        }
    }

    private bool CheckAnimationFinished()
    {
        if (targetAnimator == null || string.IsNullOrEmpty(animationName))
            return false;

        AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);

        // يتحقق إذا الأنيميشن المحدد انتهى (100% played)
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }

    private System.Collections.IEnumerator ZoomCameraCoroutine()
    {
        zooming = true;

        float elapsed = 0f;
        float startFOV = mainCamera.fieldOfView;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / zoomDuration);

            // يمكن أيضًا تغيير position للكاميرا إذا تحب تحركها نحو zoomTarget
            if (zoomTarget != null)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, zoomTarget.position, elapsed / zoomDuration);
            }

            yield return null;
        }

        mainCamera.fieldOfView = targetFOV;
        zooming = false;
    }
}
