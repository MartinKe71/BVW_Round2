using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreqBand : MonoBehaviour
{
    public GameObject _cubePrefab;
    public float _cubeInterval;
    public float _maxScale;
    public MicrophoneInput microphoneInput;
    public GameObject[] _cubes = new GameObject[8]; 

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i<8; i++)
        {
            GameObject _instancedCube = (GameObject)Instantiate(_cubePrefab);
            _instancedCube.transform.position = transform.position + new Vector3((i - 3.5f) * _cubeInterval, 0, 0);
            _instancedCube.transform.parent = transform;
            _instancedCube.transform.name = "Freq Band " + i;
            _cubes[i] = _instancedCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            _cubes[i].transform.localScale = new Vector3(1, _maxScale * microphoneInput._freqBand[i] + 1, 1);
        }
    }
}
