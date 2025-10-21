using System.Collections.Generic;
using UnityEngine;

public class RitualDetector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Sinner sinner;
    private List<GameObject> ritualsInRange = new List<GameObject>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ritual")
        {
            RitualSpot ritual = other.GetComponentInParent<RitualSpot>();
            if (!ritual.isExhausted)
            {
                ritualsInRange.Add(other.gameObject);
                sinner.canChannel = true;
            }
        }

    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ritual")
        {
            RitualSpot ritual = other.GetComponentInParent<RitualSpot>();
            if (!ritual.isExhausted)
            {
                if (!ritualsInRange.Contains(other.gameObject))
                {
                    ritualsInRange.Add(other.gameObject);
                }
                sinner.canChannel = true;
            }
            else
            {
                ritualsInRange.Remove(other.gameObject);
                if (ritualsInRange.Count == 0)
                {
                    sinner.canChannel = false;
                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ritual")
        {
            ritualsInRange.Remove(other.gameObject);
        }
        if (ritualsInRange.Count == 0)
        {
            sinner.canChannel = false;
        }
    }
}
