using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;

    Animator animator;
    void Start()=> animator = GetComponent<Animator>();

    void Update() => transform.position = playerTransform.position + offset;

    internal void Shake() => animator.SetTrigger("shake");

}
