using UnityEngine;
using UnityEngine.InputSystem; // نظام الإدخال الجديد
using UnityEngine.Animations;

public class XROriginZoomAfterAnimation : MonoBehaviour
{
    [Header("Animation Check")]
    public Animator targetAnimator;         // الأنيميشن اللي نريد ننتظر إنه يخلص
    public string animationName;            // اسم الأنيميشن المحدد
    private bool animationFinished = false;

    [Header("XR Origin Settings")]
    public Transform xrOrigin;              // الـ XR Origin أو Rig
    public Transform zoomTarget;            // الكائن اللي نريد نعمل له زوم
    public float zoomDuration = 1f;         // مدة التحريك والزوم
    public Vector3 finalOffset = new Vector3(0, 0.2f, -0.5f); // إزاحة نهائية بالنسبة للهدف

    [Header("Input Settings")]
    public InputActionReference xrButtonAction; // زر الـ XR Controller
    public Key keyboardKey = Key.A;              // زر الكيبورد

    private bool zooming = false;

    void Start()
    {
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
            StartCoroutine(ZoomXROriginCoroutine());
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

    private System.Collections.IEnumerator ZoomXROriginCoroutine()
    {
        zooming = true;

        Vector3 startPos = xrOrigin.position;
        Vector3 targetPos = zoomTarget.position + finalOffset;
        Quaternion startRot = xrOrigin.rotation;
        Quaternion targetRot = Quaternion.LookRotation(zoomTarget.position - xrOrigin.position);

        float elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;

            xrOrigin.position = Vector3.Lerp(startPos, targetPos, t);
            xrOrigin.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        xrOrigin.position = targetPos;
        xrOrigin.rotation = targetRot;
        zooming = false;
    }
}
