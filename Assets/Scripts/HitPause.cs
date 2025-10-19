using System.Collections;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static HitPause Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    public void HitStop(float duration)
    {
        StartCoroutine(HitPauseCoroutine(duration));
    }
    private IEnumerator HitPauseCoroutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
