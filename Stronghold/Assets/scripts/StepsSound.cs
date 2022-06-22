using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField]
    AudioClip steps;
    AudioSource audioSorce;
    PlayerControll playerControll;
    float rand = 0;
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
            float prevrand = rand;
            
            do rand = Random.Range(0.8f, 0.9f);
            while (rand == prevrand);

            audioSorce.pitch = (rand);
            audioSorce.PlayOneShot(steps);
        }
        
    }

}
