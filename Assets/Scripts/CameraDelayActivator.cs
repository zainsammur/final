using UnityEngine;

public class CameraDelayActivator : MonoBehaviour
{
    public GameObject cameraObject;  // الكاميرا اللي بدها تتفعل
    public float delayTime = 4f;     // وقت الانتظار

    // هاي بتناديها لما يصير جراب
    public void StartCameraSequence()
    {
        if (cameraObject != null)
        {
            cameraObject.SetActive(false); // نتأكد إنها مطفية
            Invoke(nameof(ActivateCamera), delayTime);
        }
    }

    void ActivateCamera()
    {
        cameraObject.SetActive(true);
    }
}
