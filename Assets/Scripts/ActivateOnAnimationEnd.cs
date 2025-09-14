using UnityEngine;

public class ActivateOnAnimationEnd : MonoBehaviour
{
    public Animator animator;          // Animator اللي فيه الأنيميشن
    public string animationName;       // اسم الأنيميشن كليب
    public GameObject objectToActivate; // الشي اللي بدك يصير Active

    private bool activated = false;

    void Update()
    {
        if (activated) return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // نتحقق إن الأنيميشن اللي شغال هو المطلوب
        if (state.IsName(animationName))
        {
            // normalizedTime >= 1 يعني خلص الكليب
            if (state.normalizedTime >= 1f && state.length > 0.1f)
            {
                objectToActivate.SetActive(true);
                activated = true; // عشان ما ينفذ مرة ثانية
            }
        }
    }
}
