using System.Collections;
using UnityEngine;
public class DestroyGameObject : MonoBehaviour
{
    public float destroyTime = 2f;
    void Update()
    {
        Destroy(gameObject, destroyTime);
    }
}
public class Bomb : MonoBehaviour
{
    PlayerControll player;
    
    public GameObject smoke;
    public float force { get; set; }
    public Vector3 direction { get; set; }

    private Rigidbody _rigidbody;

    [SerializeField]
    private float smokeDelay;

    [SerializeField]
    private float smokeScale;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControll>();
        StartCoroutine(SpawnSmoke());
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        // Debug.Log($"{direction} {force} {_rigidbody.velocity}");
    }

    private IEnumerator SpawnSmoke()
    {
        yield return new WaitForSeconds(smokeDelay);
        var smk = Instantiate(smoke, transform.position, Quaternion.identity);
        var sphcol = smk.AddComponent<SphereCollider>();
        Destroy(gameObject);
        sphcol.isTrigger = true;
        sphcol.radius = 5F;
        sphcol.tag = "Smoke";
        smk.transform.localScale = new Vector3(1 * smokeScale, 3, 1 * smokeScale) ;
        smk.AddComponent<DestroyGameObject>();
        smk.GetComponent<DestroyGameObject>().destroyTime = 16f;
        player.canThrowBomb = true;
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.AddForce(Physics.gravity * 3F, ForceMode.Acceleration); // custom gravity
    }
}