using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TriggerDrinkBlood : MonoBehaviour
{
    
    public PostProcessController postProcessController;
    public GameObject vampire;
    public bool isAttacking = false;

    [Header("Animation Setting")]
    public Animator cameraAnimator;
    public Animator canvasAnimator;
    public Animator vampireAnimator;
    public float drinkBloodDelay = 5f;
    public float goToHumanBackDealy = 1f;
    public float distanceWithHuman = 5f;
    public float biteDelay = 3f;
    public float humanDeathDelay = 9f;
    public float screamPlayDelay = 5f;


    private Transformation transformation;
    private CharacterMovement_position characterMovement;
    private VampireProgress vampireProgress;
    private Rigidbody rb;
    private bool isTriggerHaman = false;
    private bool isDelay = false;
    private GameObject human;
    private float defaultOffsetY;
    
    // Start is called before the first frame update
    void Start()
    {
        transformation = gameObject.GetComponent<Transformation>();
        characterMovement = gameObject.GetComponent<CharacterMovement_position>();
        vampireProgress = gameObject.GetComponent<VampireProgress>();
        rb = gameObject.GetComponent<Rigidbody>();
        defaultOffsetY = gameObject.transform.position.y - vampire.transform.position.y;  
    }

    void FixedUpdate() 
    {
        if (isTriggerHaman && characterMovement.isAttack && !isAttacking)
        {
            // Fix the attacked human and vampire
            SoundManager.instance.StopFootstep();
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            if (human != null)
            {
                // human.GetComponent<BoxCollider>().enabled = false;
                BoxCollider[] boxColliders = human.GetComponentsInChildren<BoxCollider>();
                foreach (BoxCollider collider in boxColliders)
                {
                    if (collider.isTrigger) collider.enabled = false;
                }
                CapsuleCollider[] capsuleColliders = human.GetComponentsInChildren<CapsuleCollider>();
                foreach (CapsuleCollider capsule in capsuleColliders)
                {
                    if (capsule.isTrigger) capsule.enabled = false;
                }

                NavMeshAgent navMeshAgent = human.GetComponent<NavMeshAgent>();
                if (navMeshAgent != null)
                {
                    navMeshAgent.speed = 0;
                    navMeshAgent.enabled = false;
                }

                human.GetComponent<Animator>().SetFloat("Speed", 0f);

                //Animator humanAnimator = human.GetComponent<Animator>();
                //if (humanAnimator != null) {
                //    humanAnimator.SetFloat("Speed", 0f);
                //    humanAnimator.SetBool("Idle", true);
                //    humanAnimator.SetBool("Move", false);
                //    humanAnimator.speed = 0;
                //}
            }

            // Set up state
            isAttacking = true;
            isDelay = true;
            isTriggerHaman = false;
            transformation.IsAllowToTransform = false;
            StartCoroutine(DrinkingDelay());            

            // Animation start
            StartCoroutine(GoToHumanBackDelay());
            StartCoroutine(BiteDelay());
            canvasAnimator.SetTrigger("FadeOutToDrinkBlood");
            cameraAnimator.applyRootMotion = false;
            cameraAnimator.SetTrigger("MoveToDrinkBloodView");
            postProcessController.SetDrinkTrigger(false);

            // Kill human
            StartCoroutine(HumanOnDeathDelay());

            // Audios
            StartCoroutine(ScreamDelayPlay());
        }

        if (isAttacking && !characterMovement.isAttack && !isDelay)
        {
            isAttacking = false;
            isTriggerHaman = false;
            transformation.IsAllowToTransform = true;
            postProcessController.SetDrinkTrigger(false);
        }
    }


    private void SetVampireTransform()
    {
        Gender gender = human.GetComponent<HumanMarker>()._gender;
        Vector3 targetPosition = human.transform.position + human.transform.right.normalized * distanceWithHuman;
        targetPosition += new Vector3(0, defaultOffsetY, 0);
        if (gender == Gender.Male) {
            targetPosition += new Vector3(0, 0.65f, 0);
            targetPosition += human.transform.right.normalized * 0.1f;
        }
        gameObject.transform.position = targetPosition;

        Quaternion targetRotation = human.transform.rotation;
        targetRotation *= Quaternion.Euler(0f, -90f, 0f);
        gameObject.transform.rotation = targetRotation;
    }

    IEnumerator DrinkingDelay()
    {
        yield return new WaitForSeconds(drinkBloodDelay);
        isDelay = false;
    }

    IEnumerator BiteDelay()
    {
        yield return new WaitForSeconds(biteDelay);
        vampireAnimator.SetTrigger("Bite");
    }

    IEnumerator GoToHumanBackDelay()
    {
        yield return new WaitForSeconds(goToHumanBackDealy);
        SetVampireTransform();
    }

    IEnumerator HumanOnDeathDelay()
    {
        yield return new WaitForSeconds(humanDeathDelay);
        human.GetComponent<HumanMarker>().OnDeath();
        vampireProgress.AddVampireLevel();
        rb.isKinematic = false;
        transformation.IsAllowToTransform = true;
        cameraAnimator.applyRootMotion = true;
    }

    IEnumerator ScreamDelayPlay()
    {
        yield return new WaitForSeconds(screamPlayDelay);
        SoundManager.instance.Scream(human.GetComponent<HumanMarker>()._gender);
    }


    public void TriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Human") && IsVampier())
        {
            postProcessController.SetDrinkTrigger(true);
            isTriggerHaman = true;
            human = other.gameObject.transform.parent.gameObject;
        }

    }
    
    public void TriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Human"))
        {
            postProcessController.SetDrinkTrigger(false);
            postProcessController.SetDrinking(false);
            isTriggerHaman = false;
        }
    }

    private bool IsVampier()
    {
        return  transformation.currentGuestState == GuestState.Vampier;
    }

}
