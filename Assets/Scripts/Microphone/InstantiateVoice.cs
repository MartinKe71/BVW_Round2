using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateVoice : MonoBehaviour
{
    public GameObject _voiceBarPrefab;
    GameObject[] _instantiatedPrefabs = new GameObject[512];
    public MicrophoneInput microphoneInput;
    public float _maxScale;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 512; i++)
        {
            GameObject _instancedCube = (GameObject)Instantiate(_voiceBarPrefab);
            _instancedCube.transform.position = transform.position;
            _instancedCube.transform.parent = transform;
            _instancedCube.transform.name = "Voice Bar " + i;
            transform.eulerAngles = new Vector3(0, 0.703125f * i, 0);
            _instancedCube.transform.position = Vector3.forward * 100;
            _instantiatedPrefabs[i] = _instancedCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i <512; i++)
        {
            //_instantiatedPrefabs[i].transform.localScale = new Vector3(1,1, _maxScale * microphoneInput.spectrum[i] + 2);
            _instantiatedPrefabs[i].transform.localScale = new Vector3(1, _maxScale * microphoneInput.spectrum[i] + 2,1);
        }
    }
}
