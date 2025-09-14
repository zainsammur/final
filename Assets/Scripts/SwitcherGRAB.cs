using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SwitchGRAB : MonoBehaviour
{
    [Header("References")]
    public XRGrabInteractable grabObject;      // الكائن اللي بعمل له Grab
    public GameObject replacementObject;       // الكائن البديل بعد الإفلات
    public Transform mainCamera;               // كاميرا اللاعب

    [Header("Placement Settings")]
    public float yOffsetBelowCamera = -0.2f;  // مقدار النزول تحت الكاميرا

    private bool isGrabbed = false;
    private Vector3 lastPosition;              // آخر موضع قبل الإفلات

    void OnEnable()
    {
        grabObject.selectEntered.AddListener(OnGrabStarted);
        grabObject.selectExited.AddListener(OnGrabEnded);
    }

    void OnDisable()
    {
        grabObject.selectEntered.RemoveListener(OnGrabStarted);
        grabObject.selectExited.RemoveListener(OnGrabEnded);
    }

    void OnGrabStarted(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        // خزن آخر موضع قبل الإفلات
        lastPosition = grabObject.transform.position;

        // الكائن الأصلي يظل ظاهر أثناء الجراب
        grabObject.gameObject.SetActive(true);
        if (replacementObject != null)
            replacementObject.SetActive(false);
    }

    void OnGrabEnded(SelectExitEventArgs args)
    {
        isGrabbed = false;

        if (replacementObject != null)
        {
            // استخدم آخر X وZ للكائن، وY أقل قليلاً من الكاميرا
            Vector3 targetPos = new Vector3(
                lastPosition.x,
                mainCamera.position.y + yOffsetBelowCamera,  // النزول تحت الكاميرا
                lastPosition.z
            );

            replacementObject.transform.position = targetPos;
            replacementObject.transform.rotation = grabObject.transform.rotation;
            replacementObject.SetActive(true);
        }

        // أخفي الكائن الأصلي بعد الإفلات
        grabObject.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isGrabbed)
        {
            // أثناء الجراب، خزن آخر موضع (X وZ وY)
            lastPosition = grabObject.transform.position;
        }
    }
}
