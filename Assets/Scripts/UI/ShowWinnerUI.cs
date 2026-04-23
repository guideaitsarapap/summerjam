using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowWinnerUI : UIComponent
{
    [Header("Slots & Layouts")]
    [SerializeField] private LayoutElement redLayout;
    [SerializeField] private LayoutElement blueLayout;
    [Header("Fixed Slots")]
    [SerializeField] private UIFrameAnimator redSideAnimator;
    [SerializeField] private UIFrameAnimator blueSideAnimator;

    [Header("Red Character Sprites")]
    public Sprite[] redWinFrames;
    public Sprite[] redLoseFrames;

    [Header("Blue Character Sprites")]
    public Sprite[] blueWinFrames;
    public Sprite[] blueLoseFrames;

    [Header("Settings")]
    public float punchSpeed = 5f;
    public float delayBetweenPlayers = 0.5f;

    [Header("Area Settings")]
    [Range(1f, 10f)] public float winnerRatio = 7f;
    [Range(1f, 10f)] public float loserRatio = 3f;


    public IEnumerator ShowMatchWinnerRoutine(PlayerSide winner)
    {
        this.gameObject.SetActive(true);

        SetAlpha(redSideAnimator.targetImage, 0);
        SetAlpha(blueSideAnimator.targetImage, 0);

        redLayout.flexibleWidth = 5;
        blueLayout.flexibleWidth = 5;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        if (winner == PlayerSide.Red)
        {
            redSideAnimator.PlayAnimation(redWinFrames);
            SetAlpha(redSideAnimator.targetImage, 1);
            redLayout.flexibleWidth = winnerRatio; 
            blueLayout.flexibleWidth = loserRatio;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

            yield return StartCoroutine(PunchEffect(redSideAnimator.targetImage, punchSpeed));
            
            yield return new WaitForSecondsRealtime(delayBetweenPlayers);
            
            blueSideAnimator.PlayAnimation(blueLoseFrames);
            SetAlpha(blueSideAnimator.targetImage, 1);
            yield return StartCoroutine(PunchEffect(blueSideAnimator.targetImage, punchSpeed));
        }
        else
        {
            blueSideAnimator.PlayAnimation(blueWinFrames);
            SetAlpha(blueSideAnimator.targetImage, 1);
            blueLayout.flexibleWidth = winnerRatio;
            redLayout.flexibleWidth = loserRatio;
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

            yield return StartCoroutine(PunchEffect(blueSideAnimator.targetImage, punchSpeed));

            yield return new WaitForSecondsRealtime(delayBetweenPlayers);

            redSideAnimator.PlayAnimation(redLoseFrames);
            SetAlpha(redSideAnimator.targetImage, 1); // เพิ่งจะโชว์ตัวตอนนี้!
            yield return StartCoroutine(PunchEffect(redSideAnimator.targetImage, punchSpeed));
        }

        yield return new WaitForSecondsRealtime(3f);
    }


    private void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private IEnumerator PunchEffect(Image target, float speed)
    {
        if (target == null) yield break;
        RectTransform rect = target.rectTransform;
        
        float startScale = 1.8f; 
        float endScale = 1.2f;
        
        rect.localScale = Vector3.one * startScale;
        
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * speed;
            float easedT = 1f - Mathf.Pow(1f - t, 4); 
            rect.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, easedT);
            yield return null;
        }
        rect.localScale = Vector3.one * endScale;
    }
}
