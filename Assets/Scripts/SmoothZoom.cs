using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothZoom : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public float zoomSpeed = 5f;          // سرعة التدرج

    [Header("Zoom Steps")]
    public float zoomStep = 1f;           // مقدار تغيير FOV لكل كبسة للأمام
    public float zoomStepBackward = 1f;   // مقدار تغيير FOV لكل كبسة للخلف

    [Header("Input Settings")]
    public Key zoomForwardKey = Key.K;    // زر الكيبورد للأمام
    public Key zoomBackwardKey = Key.L;   // زر الكيبورد للخلف
    public InputActionReference zoomForwardXR;   // زر XR للأمام
    public InputActionReference zoomBackwardXR;  // زر XR للخلف

    private float targetFOV;

    void Start()
    {
        if (mainCamera != null)
            targetFOV = mainCamera.fieldOfView; // قيمة البداية

        if (zoomForwardXR != null) zoomForwardXR.action.Enable();
        if (zoomBackwardXR != null) zoomBackwardXR.action.Enable();
    }

    void Update()
    {
        // كبسة للأمام
        if ((Keyboard.current != null && Keyboard.current[zoomForwardKey].wasPressedThisFrame) ||
            (zoomForwardXR != null && zoomForwardXR.action.WasPressedThisFrame()))
        {
            targetFOV -= zoomStep; // تقلل FOV
        }

        // كبسة للخلف
        if ((Keyboard.current != null && Keyboard.current[zoomBackwardKey].wasPressedThisFrame) ||
            (zoomBackwardXR != null && zoomBackwardXR.action.WasPressedThisFrame()))
        {
            targetFOV += zoomStepBackward; // تزيد FOV
        }

        // اجعل الزوم تدريجي نحو القيمة المخزنة في targetFOV
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }
}
