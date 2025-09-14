using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabReplacePermanent : MonoBehaviour
{
    [Header("XR Grab Settings")]
    public XRGrabInteractable grabInteractable; // الكائن الذي يتم سحبه
    public GameObject replacementObject;        // الكائن البديل الذي سيظهر بعد الجراب

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabStarted);

        if (replacementObject != null)
            replacementObject.SetActive(false); // البداية: الكائن البديل مخفي
    }

    void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        // حفظ الموقع والدوار الأصلي للكائن
        originalPosition = grabInteractable.transform.position;
        originalRotation = grabInteractable.transform.rotation;

        // إخفاء الكائن الأصلي
        grabInteractable.gameObject.SetActive(false);

        // تفعيل الكائن البديل في نفس الموقع والدوران
        if (replacementObject != null)
        {
            replacementObject.transform.position = originalPosition;
            replacementObject.transform.rotation = originalRotation;
            replacementObject.SetActive(true);
        }

        // لا نعيد الكائن الأصلي عند Release
    }
}
