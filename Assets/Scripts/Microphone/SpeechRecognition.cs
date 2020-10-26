using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class SpeechRecognition : MonoBehaviour
{
    public Transformation transformation;
    public string[] keywords;
    public ConfidenceLevel confidence = ConfidenceLevel.Low;

    protected PhraseRecognizer recognizer;
    protected string currentWord = "";


    // Start is called before the first frame update
    void Start()
    {
        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
            Debug.Log( recognizer.IsRunning );
        }

        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
        
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        currentWord = args.text;
        Debug.Log(currentWord);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (string word in keywords)
        {
            if (currentWord != word) continue;
            transformation.TriggerTransformation();
            currentWord = "";
            break;
        }
    }


}
