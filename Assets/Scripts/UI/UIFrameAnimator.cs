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
        if (sprites == null || sprites.Length == 0) 
        {
            Debug.LogError($"{gameObject.name}: No image");
            return;
        }
        
        currentSprites = sprites;
        currentFrame = 0;

        targetImage.sprite = currentSprites[0];

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        Debug.Log($"{gameObject.name} Animation Started!");
        while (true)
        {
            targetImage.overrideSprite = currentSprites[currentFrame];
            currentFrame = (currentFrame + 1) % currentSprites.Length;
            yield return new WaitForSecondsRealtime(frameRate);
        }
    }
}