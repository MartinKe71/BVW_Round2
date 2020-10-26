using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectRing : MonoBehaviour
{
    public AudioClip clip;
    public float hitForce = 200f;

    List<GameObject> humenAlive = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            int vampireLevel = other.gameObject.transform.parent.GetComponent<VampireProgress>().vampireLevel;
            float currentHitForce = Mathf.Max(0, hitForce - vampireLevel * hitForce);
            other.gameObject.GetComponentInParent<Rigidbody>().AddForce(-other.transform.forward * currentHitForce, ForceMode.Impulse);
            Debug.Log("Hit!");
            SoundManager.instance.HitWall(vampireLevel);
        }
    }
}
