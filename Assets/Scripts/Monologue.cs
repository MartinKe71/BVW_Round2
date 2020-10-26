using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monologue : MonoBehaviour
{
    public static Monologue instance;

    public AudioClip[] monologues;
    public GameObject Rose;
    public GameObject Rowan;
    public GameObject Jon;

    void Awake()
    {
        instance = this;
    }

    public void AudioCountDown()
    {
        StartCoroutine(PlayAudios());
    }

    IEnumerator PlayAudios()
    {
        // Play Rose 
        yield return new WaitForSecondsRealtime(120f);
        if (!ToBoat.instance.dead.Find((x) => (x==Rose)))
        {
            Rose.GetComponent<AudioSource>().clip = monologues[0];
            Rose.GetComponent<AudioSource>().Play();
        }
        

        // Play Rowan
        yield return new WaitForSecondsRealtime(60f);
        if (!ToBoat.instance.dead.Find((x) => (x == Rowan)))
        {
            Rowan.GetComponent<AudioSource>().clip = monologues[1];
            Rowan.GetComponent<AudioSource>().Play();
        }

        // Play Jon
        yield return new WaitForSecondsRealtime(60f);
        if (!ToBoat.instance.dead.Find((x) => (x == Jon)))
        {
            Jon.GetComponent<AudioSource>().clip = monologues[2];
            Jon.GetComponent<AudioSource>().Play();
        }
    }
}
