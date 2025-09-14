using UnityEngine;

public class ActivateAfterAnimation : MonoBehaviour
{
    [Header("References")]
    public Animator parentAnimator;       // الأنيميتور تبع البارنت
    public string animationName;          // اسم الأنيميشن اللي بدك تراقبه
    public GameObject childToActivate;    // التشايلد اللي بدك يطلع

    private bool hasActivated = false;

    void Update()
    {
        if (!hasActivated && parentAnimator != null && childToActivate != null)
        {
            AnimatorStateInfo stateInfo = parentAnimator.GetCurrentAnimatorStateInfo(0);

            // إذا الأنيميشن المطلوب شغال وخلص
            if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f)
            {
                childToActivate.SetActive(true);
                hasActivated = true; // عشان ما يعيدها
            }
        }
    }
}
