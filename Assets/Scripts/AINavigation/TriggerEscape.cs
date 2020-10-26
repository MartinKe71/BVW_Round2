using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEscape : MonoBehaviour
{
    private HumanNavigation humanNavigation;
    
    void Start() 
    {
        humanNavigation = gameObject.GetComponentInParent<HumanNavigation>();
    }
    
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player") && !humanNavigation.IsEscaping)
        {
            humanNavigation.SetIsEscaping();            
        }  
    }
}
