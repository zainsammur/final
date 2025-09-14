using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabReplaceAtXROrigin : MonoBehaviour
{
    public XRGrabInteractable grabInteractable; // الكائن الأصلي
    public GameObject replacementObject;        // الكائن البديل
    public Camera mainCamera;                   // المرجع للكاميرا الرئيسية

    void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabStarted);

        if (replacementObject != null)
            replacementObject.SetActive(false);
    }

    void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        if (grabInteractable == null || replacementObject == null || mainCamera == null) return;

        // حفظ موقع Main Camera الحالي
        Vector3 camPos = mainCamera.transform.position;
        Quaternion camRot = mainCamera.transform.rotation;

        // إخفاء الكائن الأصلي
        grabInteractable.gameObject.SetActive(false);

        // تفعيل الكائن البديل ووضعه في موقع Main Camera
        replacementObject.transform.position = camPos;
        replacementObject.transform.rotation = camRot;
        replacementObject.SetActive(true);
    }
}
