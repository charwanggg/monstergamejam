using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualSpot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isExhausted;
    [SerializeField] private float cooldown = 60f;
    public void ExhaustRitual()
    {
        isExhausted = true;
        StartCoroutine(ResetRitualCoroutine());
    }
    private IEnumerator ResetRitualCoroutine()
    {
        yield return new WaitForSeconds(cooldown);
        isExhausted = false;
    }
}
