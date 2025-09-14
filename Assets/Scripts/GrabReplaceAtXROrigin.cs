using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Grabable : MonoBehaviour
{
    [Header("References")]
    public Transform rightHandTransform; // اسحب هنا يد RightHand
    public GameObject replacementObject; // الكائن البديل
    public XRGrabInteractable grabInteractable; // الكائن الأصلي الذي سيتم سحبه
    public float distanceInFront = 0.3f; // المسافة أمام اليد أو الكاميرا عند الاستبدال

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnGrabReleased);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnGrabReleased);
        }
    }

    private void OnGrabReleased(SelectExitEventArgs args)
    {
        // حفظ موقع الأصلي قبل الإخفاء
        originalPosition = grabInteractable.transform.position;
        originalRotation = grabInteractable.transform.rotation;

        // إخفاء الكائن الأصلي بعد الإفلات
        grabInteractable.gameObject.SetActive(false);

        // وضع الكائن البديل أمام اليد مباشرة أو أي مكان تريد
        Vector3 spawnPos = rightHandTransform.position + rightHandTransform.forward * distanceInFront;
        replacementObject.transform.position = spawnPos;
        replacementObject.transform.rotation = rightHandTransform.rotation;
        replacementObject.SetActive(true);
    }
}
