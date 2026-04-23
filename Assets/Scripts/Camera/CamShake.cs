using UnityEngine;
using System.Collections;
using VInspector;

public class CamShake : MonoBehaviour
{
    public float shakeDuration = 0.5f; // ระยะเวลาการสั่น
    public float shakeMagnitude = 0.1f; // ความแรงของการสั่น
    public static CamShake Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }    
    }

    [Button]
    public void TriggerShake()
    {
        StartCoroutine(Shake(shakeDuration, shakeMagnitude));
    }

    public IEnumerator Shake(float duration , float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
