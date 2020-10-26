using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VolumeBar : MonoBehaviour
{
    public MicrophoneInput microphoneInput;
    public float _maxScale;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(5, microphoneInput.levelMax * _maxScale + 2, 5);
    }
}
