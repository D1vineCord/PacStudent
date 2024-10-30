using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraScript : MonoBehaviour
{
    public static bool isIntroComplete = false;  // Static flag for intro completion
    private AudioSource introAudio;

    void Start()
    {
        introAudio = GetComponent<AudioSource>();
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        introAudio.Play();
        yield return new WaitForSeconds(introAudio.clip.length);
        isIntroComplete = true;  // Set flag to true when intro finishes
    }
}

