using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShowImageUI : UIComponent
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] countDownSprites;
    [SerializeField] private Sprite overSprite;

    public void UpdateCountdownDisplay(int secondsLeft)
    {
        int index = (secondsLeft == 0) ? 3 : 3 - secondsLeft;

        if (index >= 0 && index < countDownSprites.Length)
        {
            image.sprite = countDownSprites[index];
            

            StopAllCoroutines();
            StartCoroutine(PunchEffect());
        }
    }

    public void ShowImageDisplay()
    {
        image.sprite = overSprite;
        StopAllCoroutines();
        StartCoroutine(PunchEffect());
    }

    private IEnumerator PunchEffect()
    {
        image.rectTransform.localScale = Vector3.one * 1.5f;
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 10f;
            image.rectTransform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);
            yield return null;
        }
        image.rectTransform.localScale = Vector3.one;
    }
}
