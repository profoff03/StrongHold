using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace ENEMY
{
    public enum MovementState
    {
        Linear,
        Circular,
        Homing
    }

    public class Bullet : MonoBehaviour
    {
        public float moveSpeed = 40f;
        public float rotationSpeed = 30f;
        public float lifetime;
        private Coroutine _lifeCoroutine;

        public Texture redTexture;
        public Texture yellowTexture;


        private bool _reflective;

        [UsedImplicitly]
        public bool reflective
        {
            get => _reflective;
            set
            {
                ChangeTextureTo(value ? "red" : "yellow");
                _reflective = value;
            }
        }

        [Header("Homing Target")] public Transform target;
        [Header("Rotate around parent")] public Transform parentTransform;
        [Header("Default State")] public MovementState state = MovementState.Linear;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        // no void start? :(((

        private IEnumerator LifeTime(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            switch (state)
            {
                case MovementState.Linear:
                    transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
                    break;
                case MovementState.Circular:
                    transform.RotateAround(parentTransform.position, Vector3.up, rotationSpeed * Time.deltaTime);
                    break;
                case MovementState.Homing:
                    var tsfm = transform;
                    if (tsfm is null || target is null)
                    {
                        state = MovementState.Linear;
                        break;
                    }
                    var targetDirection = target.position - tsfm.position;
                    var singleStep = Mathf.Deg2Rad * rotationSpeed * Time.deltaTime;
                    var newDirection = Vector3.RotateTowards(tsfm.forward, targetDirection, singleStep, 0.0f);
                    newDirection.y = transform.forward.y;
                    var newRotation = Quaternion.LookRotation(newDirection);
                    newRotation.x = 0;
                    newRotation.z = 0;
                    transform.rotation = newRotation;
                    transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
                    break;
                default:
                    transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
                    break;
            }
        }


        [UsedImplicitly]
        public void MakeCircularMovement(float angularSpeed, Transform parent)
        {
            parentTransform = parent;
            rotationSpeed = angularSpeed;
            state = MovementState.Circular;
        }

        [UsedImplicitly]
        public void MakeLinearMovement(float speed)
        {
            moveSpeed = speed;
            state = MovementState.Linear;
        }

        [UsedImplicitly]
        public void MakeHomingMovement(Transform player, float rotateF, float speed)
        {
            moveSpeed = speed;
            target = player;
            rotationSpeed = rotateF;
            state = MovementState.Homing;
        }

        private void OnEnable()
        {
            GameObject o;
            (o = gameObject).GetComponentInChildren<TrailRenderer>().Clear();
            o.tag = "EnemyHit";
            _lifeCoroutine = StartCoroutine(LifeTime(lifetime));
        }

        private void ChangeTextureTo(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    gameObject.GetComponent<MeshRenderer>().material.SetTexture(MainTex, redTexture);
                    break;
                case "yellow":
                    gameObject.GetComponent<MeshRenderer>().material.SetTexture(MainTex, yellowTexture);
                    break;
            }
        }

        private void Crush() => gameObject.SetActive(false);

        private void OnDisable()
        {
            transform.localScale = Vector3.one * 3;
            reflective = true;
            gameObject.GetComponentInChildren<TrailRenderer>().Clear();
            StopCoroutine(_lifeCoroutine);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !gameObject.CompareTag("Untagged"))
            {
                gameObject.tag = "EnemyHit";
                Crush();
            }

            if (other.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Hit")) Crush();
            if (other.gameObject.CompareTag("Push") && gameObject.CompareTag("EnemyHit") && reflective)
            {
                transform.Rotate(0f, 180f, 0f);
                MakeHomingMovement(parentTransform, 80f, moveSpeed);
                gameObject.tag = "Hit";
                StartCoroutine(Invisible());
            }
        }

        private IEnumerator Invisible()
        {
            GameObject o;
            var tempTag = (string)(o = gameObject).tag.Clone();
            o.tag = "Untagged";
            yield return new WaitForSeconds(.3f);
            if (gameObject.CompareTag("Untagged")) gameObject.tag = tempTag;
        }
    }
}