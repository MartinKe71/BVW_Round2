using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartToMaster : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.LoadScene("Master", LoadSceneMode.Single);
    }
}
