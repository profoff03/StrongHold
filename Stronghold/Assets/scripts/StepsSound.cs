using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsSound : MonoBehaviour
{
    [SerializeField]
    AudioClip[] dirtSteps;
    [SerializeField]
    AudioClip[] woodSteps;
    [SerializeField]
    AudioClip[] grassSteps;

    AudioClip[] steps;
    AudioSource audioSorce;
    PlayerControll playerControll;
    Collider _collider;
    float rand = 0;
    // Start is called before the first frame update
    void Start()
    {
        steps = dirtSteps;
        audioSorce = GetComponent<AudioSource>();
        playerControll = GetComponent<PlayerControll>();
        _collider = GetComponent<Collider>();

    }

    void FootSteps()
    {
        if (playerControll.isMove)
        {
            float prevrand = rand;
            
            do rand = Random.Range(0.8f, 0.9f);
            while (rand == prevrand);

            audioSorce.pitch = (rand);
            audioSorce.PlayOneShot(steps[Random.Range(0, steps.Length)]);
        }
        
    }

    
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("wood"))
        {
            steps = woodSteps;
        }
        if (other.gameObject.CompareTag("grass"))
        {
            steps = grassSteps;
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("grass"))
        {
            steps = dirtSteps;
        }
    }


}
