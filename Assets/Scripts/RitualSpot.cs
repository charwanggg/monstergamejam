using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualSpot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isExhausted;
    [SerializeField] private float cooldown = 60f;
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private Material m1;
    [SerializeField] private Material m2;
    public void ExhaustRitual()
    {
        isExhausted = true;
        StartCoroutine(ResetRitualCoroutine());
    }
    private IEnumerator ResetRitualCoroutine()
    {
        mr.materials = new Material[] {m2, m1}; 
        yield return new WaitForSeconds(cooldown);
        mr.materials = new Material[] {m1}; 
        isExhausted = false;
    }
}
