using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject smoke;
    public float force { get; set; }
    public Vector3 direction { get; set; }

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnSmoke());
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        // Debug.Log($"{direction} {force} {_rigidbody.velocity}");
    }

    private IEnumerator SpawnSmoke()
    {
        yield return new WaitForSeconds(5F);
        var smk = Instantiate(smoke, transform.position, Quaternion.identity);
        var sphcol = smk.AddComponent<SphereCollider>();
        Destroy(gameObject);
        sphcol.isTrigger = true;
        sphcol.radius = 5F;
        sphcol.tag = "Smoke";
        smk.transform.localScale = new Vector3(2, 2, 2);
        smk.AddComponent<DestroyGameObject>();
        smk.GetComponent<DestroyGameObject>().destroyTime = 16f;
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.AddForce(Physics.gravity * 3F, ForceMode.Acceleration); // custom gravity
    }
}