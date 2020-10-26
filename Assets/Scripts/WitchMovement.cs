using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchMovement : MonoBehaviour
{
    public Transform destinations;

    float relativePosition;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 distance = destinations.position - transform.position;
        //relativePosition = new Vector2(distance.x, distance.z);
        relativePosition = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(destinations.position.x, destinations.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moving = new Vector3(relativePosition * Mathf.Sin(Time.time * 0.2f) * Time.deltaTime, 0, -relativePosition * Mathf.Cos(Time.time) * Time.deltaTime);
        transform.position += moving * 0.5f;
        transform.rotation = Quaternion.LookRotation(moving) * Quaternion.Euler(new Vector3(0, 90,0));
        //transform.LookAt(moving);
    }
}
