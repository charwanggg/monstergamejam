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
            Debug.Log("Ritual Detected");
            ritualsInRange.Add(other.gameObject);
            sinner.canChannel = true;
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
