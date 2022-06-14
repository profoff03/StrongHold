using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptForLet : MonoBehaviour
{
    [SerializeField] GameObject thingSpawners;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Equals("Player"))
        {
            thingSpawners.SetActive(true);
            Destroy(gameObject);
        }
    }
}
