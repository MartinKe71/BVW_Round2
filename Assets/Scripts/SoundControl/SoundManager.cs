using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Timeline")]
    public PlayableDirector witchTimeline;

    [Header("AudioSource")]
    public AudioSource bgmSource;
    public AudioSource footstepSource; 
    public AudioSource screamSource;
    public AudioSource transformSource;
    public AudioSource narrationSource;
    public AudioSource[] dramaSource;
    public AudioSource hitWallSource;
    public AudioSource witchSource;


    [Header("AudioClips")]
    public List<AudioClip> footstepOnGrass;
    public List<AudioClip> footstepOnRoad;
    public List<AudioClip> scream;
    public AudioClip transformation;
    public List<AudioClip> narration;
    public AudioClip hitWall;
    public AudioClip firstTimeNarration;
    public List<AudioClip> witchClips;

    [Header("Settings")]
    public bool isMoving = false;
    public bool isRunning = false;
    public float volumeDecrease = 0.25f;
    public float volumeDecreaseDelay = 9f;
    public float narration2Delay = 130f;
    public bool hittedByWall = false;

    private void Awake()
    {
        instance = this;
    }

    public void StopFootstep()
    {
        footstepSource.Pause();
    }

    public void FootstepOnGrass()
    {
        if (isMoving)
        {
            AudioClip clipToPlay = isRunning? footstepOnGrass[1] : footstepOnGrass[0];
            if (footstepSource.clip != clipToPlay || !footstepSource.isPlaying)
            {
                footstepSource.clip = clipToPlay;
                footstepSource.loop = true;
                footstepSource.Play();
            }
        }
        
        if (footstepSource.isPlaying && !isMoving)
        {
            footstepSource.Pause();
        }
    }

    public void FootstepOnRoad()
    {
        if (isMoving)
        {
            AudioClip clipToPlay = isRunning ? footstepOnRoad[1] : footstepOnRoad[0];
            if (footstepSource.clip != clipToPlay || !footstepSource.isPlaying)
            {
                Debug.Log("On Road!!");
                footstepSource.clip = clipToPlay;
                footstepSource.loop = true;
                footstepSource.Play();
            }
        }
        
        if (footstepSource.isPlaying && !isMoving)
        {
            Debug.Log("Stop!");
            footstepSource.Pause();
        }
    }

    public void Scream(Gender gender)
    {
        footstepSource.Pause();

        int sexIndex = (gender == Gender.Male) ? 0 : 1;

        screamSource.clip = scream[sexIndex];
        screamSource.loop = false;
        screamSource.Play();
    }

    public void Transformation()
    {
        transformSource.clip = transformation;
        transformSource.loop = false;
        transformSource.Play();
    }

    public void DramaStart()
    {
        StartCoroutine(VolumeDecrease());
        PlayNarration2();
        StartCoroutine(NoticeHumanToMove());
        foreach (AudioSource source in dramaSource)
        {
            source.Play();
        }
    }

    IEnumerator VolumeDecrease()
    {
        yield return new WaitForSeconds(volumeDecreaseDelay);
        footstepSource.volume *= volumeDecrease;
        bgmSource.volume *= volumeDecrease;
    }

    void PlayNarration2()
    {
        //yield return new WaitForSeconds(narration2Delay);
        
        Narration(1);
    }

    IEnumerator NoticeHumanToMove()
    {
        Monologue.instance.AudioCountDown();

        // Mary
        yield return new WaitForSeconds(36f);
        dramaSource[2].gameObject.GetComponent<HumanNavigation>().StartMoving();

        // Rose
        yield return new WaitForSeconds(15f);
        dramaSource[0].gameObject.GetComponent<HumanNavigation>().StartMoving();

        // Rowan
        yield return new WaitForSeconds(15f);
        dramaSource[1].gameObject.GetComponent<HumanNavigation>().StartMoving();

        footstepSource.volume /= volumeDecrease;
        bgmSource.volume /= volumeDecrease;
    }

    public void Narration(int clip)
    {
        narrationSource.clip = narration[clip];
        narrationSource.loop = false;
        narrationSource.Play();
    }

    public void HitWall(int level)
    {
        hitWallSource.clip = hitWall;
        hitWallSource.pitch = 1 + level / 10;
        hitWallSource.Play();
        if (!hittedByWall && level < 1) hitWallSource.PlayOneShot(firstTimeNarration, 0.2f);
        hittedByWall = true;
    }

    public void AfterKillPeople()
    {
        StartCoroutine(AfterKillPeopleDelay());
    }
    
    private bool isPlayWitchHit = false;
    public void Witch(int level)
    {
        if (level == 4)
        {
            witchSource.clip = witchClips[4];
            witchSource.Play();

        } else
        {
            if (!isPlayWitchHit)
            {
                isPlayWitchHit = true;
                StartCoroutine(HitWitchFirstTime());
            }
        }
    }

    IEnumerator HitWitchFirstTime()
    {
        witchSource.clip = witchClips[0];
        witchSource.Play();
        yield return new WaitForSeconds(10f);
        witchSource.clip = witchClips[1];
        witchSource.Play();
    }

    IEnumerator AfterKillPeopleDelay()
    {
        witchSource.clip = witchClips[2];
        witchSource.Play();
        yield return new WaitForSeconds(7f);
        witchTimeline.Play();
    }


}
