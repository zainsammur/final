using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanFadeController : MonoBehaviour
{
    public Animator animator;           // Animator على جسم الإنسان
    public float fadeDuration = 2f;     // مدة الفيد
    public float targetAlpha = 0.3f;    // الشفافية النهائية

    private bool faded = false;
    private List<Renderer> renderers = new List<Renderer>();
    private List<MaterialPropertyBlock> blocks = new List<MaterialPropertyBlock>();

    void Awake()
    {
        // نجمع كل Renderer تحت Parent (MeshRenderer و SkinnedMeshRenderer)
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            renderers.Add(r);
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);
            blocks.Add(block);
        }
    }

    void Update()
    {
        if (!faded && animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            // أي أنيميشن خلص normalizedTime >= 1
            if (state.normalizedTime >= 1f && state.length > 0.1f)
            {
                faded = true;
                StartCoroutine(FadeOutCoroutine());
            }
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, targetAlpha, elapsed / fadeDuration);

            for (int i = 0; i < renderers.Count; i++)
            {
                MaterialPropertyBlock block = blocks[i];
                block.SetColor("_BaseColor", new Color(1f, 1f, 1f, alpha));
                renderers[i].SetPropertyBlock(block);
            }

            yield return null;
        }
    }
}
