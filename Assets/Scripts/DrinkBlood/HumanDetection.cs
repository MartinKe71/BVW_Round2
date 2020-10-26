using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanDetection : MonoBehaviour
{
    
    private TriggerDrinkBlood triggerDrinkBlood;
    // Start is called before the first frame update
    void Start()
    {
        triggerDrinkBlood = gameObject.GetComponentInParent<TriggerDrinkBlood>();
    }

    void OnTriggerEnter(Collider other) 
    {
        triggerDrinkBlood.TriggerEnter(other);
    }

    void OnTriggerExit(Collider other) 
    {
        triggerDrinkBlood.TriggerExit(other);
    }

  
}
