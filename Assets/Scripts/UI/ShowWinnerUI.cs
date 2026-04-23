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

        redLayout.flexibleWidth = 5;
        blueLayout.flexibleWidth = 5;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        if (winner == PlayerSide.Red)
        {
            redSideAnimator.PlayAnimation(redWinFrames);
            redLayout.flexibleWidth = winnerRatio; 
            blueLayout.flexibleWidth = loserRatio;
            

            yield return StartCoroutine(PunchEffect(redSideAnimator.targetImage, punchSpeed));
            

            yield return new WaitForSecondsRealtime(delayBetweenPlayers);
            

            blueSideAnimator.PlayAnimation(blueLoseFrames);
            yield return StartCoroutine(PunchEffect(blueSideAnimator.targetImage, punchSpeed));
        }
        else
        {

            blueSideAnimator.PlayAnimation(blueWinFrames);
            blueLayout.flexibleWidth = winnerRatio;
            redLayout.flexibleWidth = loserRatio;


            yield return StartCoroutine(PunchEffect(blueSideAnimator.targetImage, punchSpeed));


            yield return new WaitForSecondsRealtime(delayBetweenPlayers);


            redSideAnimator.PlayAnimation(redLoseFrames);
            yield return StartCoroutine(PunchEffect(redSideAnimator.targetImage, punchSpeed));
        }

        yield return new WaitForSecondsRealtime(3f);
    }

    private IEnumerator PunchEffect(Image target, float speed)
    {
        if (target == null) yield break;
        RectTransform rect = target.rectTransform;
        
        float startScaleMult = 1.5f;
        rect.localScale = Vector3.one * startScaleMult;
        
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * speed;
            rect.localScale = Vector3.Lerp(Vector3.one * startScaleMult, Vector3.one, t);
            yield return null;
        }
        rect.localScale = Vector3.one;
    }
}
