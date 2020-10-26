using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCameraFroDrinkView : MonoBehaviour
{
    public Animator cameraAnimator;
        
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetToDrinkView()
    {
        cameraAnimator.SetTrigger("ToDrinkView");
    }

      public void SetToFirstPersonView()
    {
        cameraAnimator.SetTrigger("ToFirstPersonView");
    }
}
