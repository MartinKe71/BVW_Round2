using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GuestState { Vampier, Bat };


public class Transformation : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bat;
    public GameObject vampire;
    public GameObject playerCamera;
    public GameObject smokeParticle;
    public bool isAllowToTransform = false;


    private GuestState guestState = GuestState.Vampier;
    public GuestState currentGuestState 
    {
        get { return guestState; }
    }

    public float floatDistance;
    public float zoomOutDistance = 10f;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private bool isFalling = false;
    private Vector3 defaultCameraPosition;
    private Vector3 defaultBoxColliderSize;
    private Vector3 defaultBoxColliderCenter;

    public bool IsAllowToTransform
    {
        get { return isAllowToTransform; }
        set { isAllowToTransform = value; }
    }

    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        rb = gameObject.GetComponent<Rigidbody>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        defaultCameraPosition = playerCamera.transform.localPosition;
        defaultBoxColliderSize = boxCollider.size;
        defaultBoxColliderCenter = boxCollider.center;
    }

    // Update is called once per frame
    public void TriggerTransformation()
    {
        if (isAllowToTransform)
        {
            
            switch (guestState)
            {
                case GuestState.Vampier:
                    TransfromToBat();
                    break;
                
                case GuestState.Bat:
                    isFalling = true;
                    TransfromToVampier();
                    break;
            }
            SoundManager.instance.Transformation();
        }
    }

    void OnCollisionEnter(Collision other) 
    {
        if (isFalling)
        {
            isFalling = false;
            guestState = GuestState.Vampier;
        }
    }

    private void TransfromToBat()
    {
        InstaniateSmoke();
        
        //ChangeRenderView(1);
        
        SetTransformToBat();
        guestState = GuestState.Bat;

        gameObject.transform.DOMoveY(
            transform.position.y + floatDistance, 2f).SetEase(Ease.Linear);

        Vector3 targetCameraTransform = playerCamera.transform.localPosition - Vector3.forward * zoomOutDistance;
        playerCamera.transform.DOLocalMove(
            targetCameraTransform, 1f).SetEase(Ease.OutExpo);
    }

    private void TransfromToVampier()
    {
        InstaniateSmoke();

        //ChangeRenderView(0);
        
        SetTransformToVampire();

        playerCamera.transform.DOLocalMove(
            defaultCameraPosition, 1f).SetEase(Ease.OutExpo);
    } 

    private void SetTransformToBat()
    {
        rb.useGravity = false;
        boxCollider.center = new Vector3(1f, 2f, 2.4f);
        boxCollider.size = new Vector3(4.2f, 2.2f, 2.3f);
        bat.SetActive(true);
        vampire.SetActive(false);
    }

    private void SetTransformToVampire()
    {
        rb.useGravity = true;
        boxCollider.size = defaultBoxColliderSize;
        boxCollider.center = defaultBoxColliderCenter;
        bat.SetActive(false);
        vampire.SetActive(true);
    }

    private void InstaniateSmoke()
    {
        var particle = Instantiate(smokeParticle, gameObject.transform);
        Destroy(particle, 3); 
    }

    private void ChangeRenderView(int state)
    {
        UnityEngine.Rendering.Universal.UniversalAdditionalCameraData additionalCameraData = playerCamera.transform.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        additionalCameraData.SetRenderer(state);
    }

    
}
