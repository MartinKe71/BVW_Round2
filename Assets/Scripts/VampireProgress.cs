using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireProgress : MonoBehaviour
{
    public int vampireLevel = 0;
    public GameObject batsGroup;
    public Camera cam;

    [Header("AudioDelaySettings")]
    public float delay = 4f;


    private bool isDramaPlayed = false;
    private bool isKillWitch = false;
    private bool isKillPeople = false;
    
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("DramaTrigger") && !isDramaPlayed)
        {
            SoundManager.instance.DramaStart();
            isDramaPlayed = true;
        }

        if (other.gameObject.CompareTag("Witch") && !isKillWitch)
        {
            SoundManager.instance.Witch(vampireLevel);
            if (vampireLevel == 4) { KillWitch(); }
            else { HitByWitch(); }
        }
    }

    private void HitByWitch()
    {
        gameObject.GetComponentInParent<Rigidbody>().AddForce(
            -transform.forward * 50f, ForceMode.Impulse
        );
    }

    private void KillWitch()
    {
        vampireLevel ++;
        isKillWitch = true;
        SceneChangeManager.instance.EndGame();
    }

    public void AddVampireLevel()
    {
        vampireLevel ++;
        SetScene(vampireLevel);
    }

    private void SetScene(int level)
    {
        switch (level)
        {
            case 1:
                StartCoroutine(PlayTransformNarration());
                ChangeRenderView(1);
                break;

            case 4:
                if (!isKillPeople) StartCoroutine(ToFinalBattle());
                break;
            
            default:
                break;

        }
    }

    IEnumerator PlayTransformNarration()
    {
        yield return new WaitForSeconds(delay);
        SoundManager.instance.Narration(2);
    }

    IEnumerator ToFinalBattle()
    {
        isKillPeople = true;
        yield return new WaitForSeconds(delay);
        batsGroup.SetActive(true);
        SoundManager.instance.AfterKillPeople();
    }

    private void ChangeRenderView(int state)
    {
        UnityEngine.Rendering.Universal.UniversalAdditionalCameraData additionalCameraData = cam.transform.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        additionalCameraData.SetRenderer(state);
    }
}
