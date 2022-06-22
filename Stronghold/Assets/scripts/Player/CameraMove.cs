using UnityEngine;


public class CameraMove : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;
    private Animator _animator;
    private void Start() => _animator = GetComponent<Animator>();
    private void Update() => transform.position = playerTransform.position + offset;
    internal void Shake() => _animator.SetTrigger("shake");
}