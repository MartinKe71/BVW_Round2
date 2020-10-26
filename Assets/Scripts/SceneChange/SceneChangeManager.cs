using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance;

    public Animator fadeAnimator;
    public VampireProgress vampireProgress;

    private void Awake()
    {
        instance = this;
    }
    
    public void EndGame()
    {
        StartCoroutine(ToEndScene());
    }

    IEnumerator ToEndScene()
    {
        PlayerPrefs.SetInt("score", vampireProgress.vampireLevel);
        fadeAnimator.SetTrigger("FadeOutToEnd");
        yield return new WaitForSeconds(10f);        
        SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
    }
}
