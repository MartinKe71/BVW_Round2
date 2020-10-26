using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BatRandomFly : MonoBehaviour
{
    public float velocity = 1f;

    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        float timePeriod = Random.Range(5f, 15f);
        InvokeRepeating("RandomRotation", 1f, timePeriod);
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Ray forwardRay = new Ray(transform.position, -transform.right);
        if (Physics.Raycast(forwardRay, out hit) && hit.distance < 5f)
        {
            ReverseRotation();
        }
    
    }

    private void RandomRotation()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            gameObject.transform.DORotateQuaternion(Random.rotation, 1f).SetEase(Ease.Linear).OnComplete(() => {
                AddVelocity();
            });
        }
    }

    private void ReverseRotation()
    {
          if (rb != null)
        {
            rb.velocity = Vector3.zero;
            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            gameObject.transform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.Linear).OnComplete(() => {
                AddVelocity();
            });
        }

    }

    private void AddVelocity()
    {
        if (rb != null)
        {
            rb.velocity = velocity * transform.right * -1f;
        }
    }
}
