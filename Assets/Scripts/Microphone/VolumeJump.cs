using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeJump : MonoBehaviour
{
    public MicrophoneInput microphoneInput;
    public float _maxScale;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, _maxScale * microphoneInput.levelMax, 0);
    }
}
