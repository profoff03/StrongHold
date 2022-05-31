using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeTimer : MonoBehaviour
{
    private float _startTime;
    internal float _smokeTime;

    private void Start()
    {
        _startTime = Time.time;
    }

    private void Update()
    {
        _smokeTime = Time.time - _startTime;  
    }


}
