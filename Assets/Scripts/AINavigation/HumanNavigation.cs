using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class HumanNavigation : MonoBehaviour
{
    public AudioClip goToBoat;
    public GameObject boat;

    /* Three destinations:
     * 0: original destination
     * 1: escaping destination
     * 2: back to bonefire
    */
    public Transform[] destinations;
    public Transform boatTransform;
    public float viewSeeingTime = 3f;
    public float walkSpeed = 1.5f;
    public float runSpeed = 3.0f;
    
    private NavMeshAgent navAgent;
    private Transform[] pointList;
    private Animator animator;

    public bool isEscaping = false;
    public bool startMoving = false;
    public bool reachGoal = false;
    public bool reachBoat = false;
    public bool reachFire = false;
    public bool IsEscaping
    {
        get { return isEscaping; }
    }
    
    void Start()
    {
        animator = GetComponent<Animator>();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;

        animator.SetFloat("Speed", 0f);
    }
    
    void Update() 
    {
        if (startMoving)
        {
            // reach the first goal, either 0 or 1
            if (GetRemainingDistance(navAgent.destination) <= 1f)
            {
                if (!reachGoal)
                {
                    navAgent.speed = 0f;
                    navAgent.enabled = false;
                    StartCoroutine(SeeTheView());
                    reachGoal = true;
                }
                else if (!reachFire)
                {
                    startMoving = false;
                    reachFire = true;
                    navAgent.speed = 0f;
                    navAgent.enabled = false;
                    ToBoat.instance.humanProtected.Add(gameObject);
                }
                else if (!reachBoat)
                {
                    Debug.Log("Reached the boat");
                    reachBoat = true;
                    startMoving = false;
                    navAgent.speed = 0f;
                    navAgent.enabled = false;
                    transform.parent = boat.transform;
                    ToBoat.instance.onBoat.Add(gameObject);
                    ToBoat.instance.humanProtected.Remove(gameObject);
                    this.transform.position = boatTransform.position;
                }
            }
            animator.SetFloat("Speed", navAgent.speed);
        }
    }

    void LateUpdate()
    {
        if (navAgent.speed != 0 && navAgent.enabled && startMoving)
        {
            Quaternion rotation = Quaternion.LookRotation(navAgent.velocity.normalized) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.4f);
        }
    }

    public void StartMoving()
    {
        if (navAgent.enabled)
        {
            navAgent.SetDestination(destinations[0].position);
        }
        navAgent.speed = walkSpeed;
        navAgent.acceleration = 1f;
        startMoving = true;
    }

    IEnumerator SeeTheView()
    {
        Debug.Log("Coroutine started!");
        yield return new WaitForSecondsRealtime(viewSeeingTime);
        navAgent.speed = walkSpeed;
        navAgent.enabled = true;
        if (navAgent.enabled)
        {
            navAgent.SetDestination(destinations[2].position);
        }
        Debug.Log("Coroutine finished!");
        if (goToBoat != null)
        {
            GetComponent<AudioSource>().clip = goToBoat;
            GetComponent<AudioSource>().Play();
        }
    }

    public float GetRemainingDistance(Vector3 dest)
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(dest.x, dest.z));
    }

    public void SetIsEscaping()
    {
        isEscaping = true;
        reachGoal = false;
        if (navAgent.enabled)
        {
            navAgent.SetDestination(destinations[1].position);
            navAgent.speed = runSpeed;
        }       
    }
}
