using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EndScene : MonoBehaviour
{
    public Text scoreText;
    public Text titleText;
    public GameObject texts;
    public GameObject button;
    public Animator vampirAnimator;
    public Animator coffinAnimator;
    public Animator maskAnimator;
    public AudioSource coffinAudio;
    public string[] titlesList = {
       "Count Von Count",
       "Edward",
       "Lestat",
       "Drusilla",
       "Nosferatu",
       "Dracula",
    };
    
    private int score = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        score = PlayerPrefs.GetInt ("score");
        scoreText.text = score.ToString() + " / 5";
        titleText.text = titlesList[score];
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void ClickPlayAgain()
    {
        Debug.Log("click");
        Text[] allTexts = texts.GetComponentsInChildren<Text>();
        foreach (Text t in allTexts) t.enabled = false;
        button.GetComponentInChildren<Text>().enabled = false;

        // Animation
        coffinAudio.Play();
        vampirAnimator.enabled = true;
        coffinAnimator.enabled = true;

        // Wait to reload scene
        StartCoroutine(ReloadScene());
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(3f);
        maskAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Master", LoadSceneMode.Single);
    }


}
