using System;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    public float destroyTime = 2f;
    void Update() => Destroy(gameObject, destroyTime);
}