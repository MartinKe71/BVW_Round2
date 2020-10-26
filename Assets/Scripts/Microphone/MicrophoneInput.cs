using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip _audioClip;
    public AudioMixerGroup _mixerGroupMicrophone, _mixerGroupMaster;

    public bool _useMicrophone;
    public float levelMax;
    public int _sampleWindow;

    public float[] spectrum = new float[512];
    public float[] _freqBand = new float[8];
    public string _selectedDevice;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_useMicrophone && Microphone.devices.Length > 0)
        {
            _selectedDevice = Microphone.devices[0].ToString();
            _audioSource.clip = Microphone.Start(_selectedDevice, true, 10, AudioSettings.outputSampleRate);
            _audioSource.outputAudioMixerGroup = _mixerGroupMicrophone;
            _audioSource.Play();            
        }
        else
        {
            _audioSource.clip = _audioClip;
            _audioSource.outputAudioMixerGroup = _mixerGroupMaster;
            _audioSource.Play();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_audioSource.clip != null)
        {
            GetAudioVolumeData();
            GetSpectrumAudioData();
            MakeFrequencyBands();
        }
    }

    void GetAudioVolumeData()
    {
        levelMax = 0;
        float[] waveData = new float[128];
        if (_useMicrophone)
        {
            int micPosition = Microphone.GetPosition(null) - (128 + 1); // null means the first microphone
            if (micPosition < 0)
            {
                levelMax = 0;
                return;
            }

            _audioSource.clip.GetData(waveData, micPosition);
            // Getting a peak on the last 128 samples
            
        }
        else
        {
            _audioSource.clip.GetData(waveData, _audioSource.timeSamples);
        }
        for (int i = 0; i < 128; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }


        Debug.Log(levelMax);
    }

    void GetSpectrumAudioData()
    {
        _audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        int count = 0;
        for (int i = 0; i < 8; i ++)
        {
            float average = 0f;
            int sampleCount = (int)Mathf.Pow(2, i + 1);
            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrum[count] * (count + 1);
                count++;
            }
            average /= sampleCount;

            _freqBand[i] = average * 10;
        }
    }
}
