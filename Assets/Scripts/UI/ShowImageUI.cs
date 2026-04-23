using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowImageUI : UIComponent
{
    [SerializeField] private Image countDownImage;
    [SerializeField] private Image overImageUI;
    [SerializeField] private Sprite[] countDownSprites;
    [SerializeField] private Sprite overSprite;

    [Header("Punch Animation Settings")]
    [SerializeField] private float punchSpeed = 5f;
    
    [Tooltip("ขนาดเริ่มต้นตอนเด้งออกมา (1.5 = ใหญ่กว่าปกติ 50%)")]
    [SerializeField] private float startScaleMultiplier = 1.5f;

    private Coroutine punchRoutine;

    public void UpdateCountdownDisplay(int secondsLeft)
    {
        int index = (secondsLeft <= 0) ? 3 : 3 - secondsLeft;

        if (index >= 0 && index < countDownSprites.Length)
        {
            countDownImage.gameObject.SetActive(true);
            overImageUI.gameObject.SetActive(false);

            countDownImage.sprite = countDownSprites[index];

            RestartPunch(countDownImage);
        }
    }

    public void ShowImageDisplay()
    {
        countDownImage.gameObject.SetActive(false);
        overImageUI.gameObject.SetActive(true);

        overImageUI.sprite = overSprite;


        RestartPunch(overImageUI);
    }

    private void RestartPunch(Image targetImage)
    {
        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(PunchEffect(targetImage, punchSpeed));
    }

    private IEnumerator PunchEffect(Image target, float speed)
    {
        if (target == null) yield break;

        RectTransform rect = target.rectTransform;
        
        Vector3 startScale = Vector3.one * startScaleMultiplier;
        rect.localScale = startScale;
        
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * speed; 
            
            rect.localScale = Vector3.Lerp(startScale, Vector3.one, t);
            yield return null;
        }
        
        rect.localScale = Vector3.one;
    }
}