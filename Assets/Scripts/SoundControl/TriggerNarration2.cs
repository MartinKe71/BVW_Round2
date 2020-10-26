using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNarration2 : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.instance.Narration(1);
        }
    }
}
