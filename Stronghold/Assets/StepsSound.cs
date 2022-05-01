using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField]
    AudioClip steps;
    AudioSource audioSorce;
    PlayerControll playerControll;
    int rand = 0;
    // Start is called before the first frame update
    void Start()
    {
        audioSorce = GetComponent<AudioSource>();
        playerControll = GetComponent<PlayerControll>();
    }

    void FootSteps()
    {
        if (playerControll.isMove)
        {
            int prevrand = rand;
            audioSorce.pitch = (Random.Range(0.7f, 0.9f));
            audioSorce.PlayOneShot(steps);
        }
        
    }

}
