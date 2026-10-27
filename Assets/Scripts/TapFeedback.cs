using System.Collections;
using UnityEngine;

/// <summary>
/// Petit effet "rebond" quand on touche la forge.
/// À placer sur le bouton Forge, puis appeler Punch() depuis son OnClick.
/// </summary>
public class TapFeedback : MonoBehaviour
{
    [SerializeField] private float punchScale = 0.9f;
    [SerializeField] private float duration = 0.08f;

    private Vector3 baseScale;
    private Coroutine running;

    private void Awake() => baseScale = transform.localScale;

    public void Punch()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        transform.localScale = baseScale * punchScale;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(baseScale * punchScale, baseScale, t / duration);
            yield return null;
        }
        transform.localScale = baseScale;
    }
}
