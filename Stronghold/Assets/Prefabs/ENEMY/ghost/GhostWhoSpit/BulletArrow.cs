using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

public class BulletArrow : MonoBehaviour
{
    public float Speed;
    //Vector3 lastPos;
    public GameObject Spawn;
    private bool Spawned;
    //Vector3 Minus;
    public GameObject Point;
    internal bool _isTouched;
    private TrailEffect Effect;
    private GameObject DOT;
    private Rigidbody _rb;
    [SerializeField]
    private float _damage;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        //lastPos = transform.position;
        Spawn = GameObject.FindGameObjectWithTag("Spawner");
        Effect = GetComponent<TrailEffect>();
        GetComponent<DamageProperty>().Damage = _damage;
    }

    private void FixedUpdate()
    {
        _rb.AddForce(transform.forward * Speed * Time.deltaTime * 500);

        Destroy(gameObject, 15.0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            _isTouched = true;
            Destroy(gameObject);
            Destroy(DOT);
        }
        if (other.CompareTag("Untagged"))
        {
            Destroy(DOT);
            Destroy(gameObject);
        }

        
    }


}
