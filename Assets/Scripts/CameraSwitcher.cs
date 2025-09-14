using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera;
    public Camera secondCamera;

    [Header("Transition Settings")]
    public float transitionDuration = 1.0f;
    public float holdTime = 2.0f;
    public float zoomAmount = 5.0f; // مقدار الزوم أثناء التحول

    [Header("Controller Input (Optional)")]
    public InputActionProperty controllerSwitchAction;

    private bool isSwitching = false;

    private Vector3 mainOriginalPos;
    private Quaternion mainOriginalRot;

    void Start()
    {
        mainCamera.gameObject.SetActive(true);
        secondCamera.gameObject.SetActive(false);

        mainOriginalPos = mainCamera.transform.position;
        mainOriginalRot = mainCamera.transform.rotation;

        if (controllerSwitchAction != null && controllerSwitchAction.action != null)
            controllerSwitchAction.action.Enable();
    }

    void Update()
    {
        if (isSwitching) return;

        bool keyboardPressed = Input.GetKeyDown(KeyCode.I);
        bool controllerPressed = controllerSwitchAction != null && controllerSwitchAction.action.WasPerformedThisFrame();

        if (keyboardPressed || controllerPressed)
        {
            StartCoroutine(CinematicSwitchRoutine());
        }
    }

    IEnumerator CinematicSwitchRoutine()
    {
        isSwitching = true;

        Vector3 startPos = mainOriginalPos;
        Quaternion startRot = mainOriginalRot;

        Vector3 zoomPos = startPos + mainCamera.transform.forward * zoomAmount;
        Quaternion zoomRot = startRot;

        float t = 0f;

        // الانتقال مع الزوم إلى الكاميرا الثانية
        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;
            mainCamera.transform.position = Vector3.Lerp(startPos, zoomPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, zoomRot, t);
            yield return null;
        }

        mainCamera.gameObject.SetActive(false);
        secondCamera.gameObject.SetActive(true);

        // الانتظار على الكاميرا الثانية
        yield return new WaitForSeconds(holdTime);

        // العودة بسلاسة إلى الكاميرا الرئيسية
        secondCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;
            mainCamera.transform.position = Vector3.Lerp(zoomPos, startPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(zoomRot, startRot, t);
            yield return null;
        }

        isSwitching = false;
    }
}
