using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class ToBoat : MonoBehaviour
{
    public PlayableDirector runToBoat;
    public PlayableDirector leaveWithBoat;

    public static ToBoat instance;

    public List<GameObject> humanProtected = new List<GameObject>();
    public List<GameObject> dead = new List<GameObject>();
    public List<GameObject> onBoat = new List<GameObject>();

    public bool countDownStart = false;
    public float curTime = 0f;

    bool moveToBoat = false;
    float aliveCount = 0f;

    private void Awake()
    {
        instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if (humanProtected.Count + dead.Count == 4 && !moveToBoat && dead.Count != 4)
        {
            if (humanProtected.Count>0)
            {
                moveToBoat = true;
                StartCoroutine(AliveToBoat());
            }
        }
        if (onBoat.Count + dead.Count == 4 && dead.Count != 4)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Animator>().SetTrigger("Leave");
            leaveWithBoat.Play();
        }
    }

    IEnumerator AliveToBoat()
    {
        yield return new WaitForSeconds(10f);
        foreach (GameObject human in humanProtected)
        {
            Debug.Log(human.transform.name);            
            NavMeshAgent navAgent = human.GetComponent<NavMeshAgent>();
            navAgent.enabled = true;
            navAgent.SetDestination(human.GetComponent<HumanNavigation>().boatTransform.position);
            navAgent.speed = 3f;
            human.GetComponent<Animator>().SetFloat("Speed", 3f);
            human.GetComponent<HumanNavigation>().startMoving = true;
            runToBoat.Play();
        }
    }

    public void ChangeToEndScene()
    {
        SceneChangeManager.instance.EndGame();
    }
}
