using UnityEngine;

public class FadingWall : MonoBehaviour
{
    public float fadeDuration = 0.4f;
    private SpriteRenderer[] spriteRenderers;
    private bool isFading = false;
    private LTDescr fadeTween;

    private void Start()
    {
        // Get all SpriteRenderer components in the hierarchy under this GameObject
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            // Fade out
            fadeTween = LeanTween.value(gameObject, 1f, 0f, fadeDuration)
                .setOnUpdate((float alpha) =>
                {
                    foreach (SpriteRenderer renderer in spriteRenderers)
                    {
                        Color color = renderer.color;
                        color.a = alpha;
                        renderer.color = color;
                    }
                })
                .setOnComplete(() =>
                {
                    // Optionally, disable the GameObject or do other actions
                });

            isFading = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isFading && fadeTween != null)
        {
            // Stop the current fade and fade back in
            LeanTween.cancel(fadeTween.id);
            LeanTween.value(gameObject, 0f, 1f, fadeDuration)
                .setOnUpdate((float alpha) =>
                {
                    foreach (SpriteRenderer renderer in spriteRenderers)
                    {
                        Color color = renderer.color;
                        color.a = alpha;
                        renderer.color = color;
                    }
                })
                .setOnComplete(() =>
                {
                    isFading = false;
                });

            isFading = false;
        }
    }
}
