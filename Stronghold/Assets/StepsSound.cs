using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField]
    AudioClip[] steps;
    AudioSource audioSorce;
    PlayerControll playerControll;
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
            int rand = Random.Range(0, steps.Length);
            audioSorce.PlayOneShot(steps[rand]);
        }
        
    }

}
