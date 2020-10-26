using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTest : MonoBehaviour
{
    Rigidbody[] rbs;
    public Transform joint;

    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(true);
    }

    private void Ragdoll(bool state)
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = !state;
        }
        joint.GetComponent<Rigidbody>().AddForce(new Vector3(0, -5, 5), ForceMode.Impulse);
    }
}
