using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandedScale : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    private Vector3 initialScale; // الحجم الأولي للكائن عند الإمساك به
    private float initialDistance; // المسافة الأولية بين اليدين عند الإمساك

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // إذا تم الإمساك بيد واحدة فقط، لا تفعل شيئًا خاصًا بالتكبير/التصغير
        if (interactorsSelecting.Count == 1)
        {
            initialScale = transform.localScale;
        }
        // إذا تم الإمساك بيدين، احسب المسافة الأولية
        else if (interactorsSelecting.Count == 2)
        {
            initialScale = transform.localScale;
            initialDistance = Vector3.Distance(interactorsSelecting[0].transform.position, interactorsSelecting[1].transform.position);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // إعادة تعيين المتغيرات عند ترك الكائن
        initialDistance = 0f;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && interactorsSelecting.Count == 2)
        {
            // احسب المسافة الحالية بين اليدين
            float currentDistance = Vector3.Distance(interactorsSelecting[0].transform.position, interactorsSelecting[1].transform.position);

            // تجنب القسمة على صفر
            if (initialDistance == 0) initialDistance = currentDistance;

            // احسب عامل التكبير/التصغير بناءً على التغير في المسافة
            float scaleFactor = currentDistance / initialDistance;

            // طبق عامل التكبير/التصغير على الحجم الأولي
            transform.localScale = initialScale * scaleFactor;
        }
    }
}
