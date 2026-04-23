using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFrameAnimator : MonoBehaviour
{
    public Image targetImage;
    [SerializeField] private float frameRate = 0.1f; // ความเร็วต่อเฟรม
    private Sprite[] currentSprites;
    private int currentFrame;
    private Coroutine animationRoutine;

    public void PlayAnimation(Sprite[] sprites)
    {
        if (sprites == null || sprites.Length == 0) return;
        
        currentSprites = sprites;
        currentFrame = 0;

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        while (true)
        {
            targetImage.sprite = currentSprites[currentFrame];
            currentFrame = (currentFrame + 1) % currentSprites.Length;
            yield return new WaitForSecondsRealtime(frameRate);
        }
    }
}