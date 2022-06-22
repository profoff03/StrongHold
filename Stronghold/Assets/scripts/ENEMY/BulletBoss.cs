using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ENEMY
{
    public enum Phases
    {
        Idle = 0,
        First = 1,
        Second = 2
    }

    public class BulletBoss : MonoBehaviour
    {
        // external objects
        public GameObject bullet; // prefab
        public Transform totalPosition;
        public PlayerControll player;

        // positions
        private List<Transform> _randPositions;
        private List<Transform> _centerPositions;
        private float _constantHeightPosition;
        private float _targetScale;

        // hp
        [SerializeField]
        private float _health;

        private float health
        {
            get => _health;
            set
            {
                _health = value;
                if (_health <= 0.001) _health = 0f;
                _healthSlider.value = _health;
            }
        }

        [SerializeField] private float maxHealth = 500;
        private Canvas _canvas;
        private Slider _healthSlider;

        // phases
        public Phases phase = Phases.Idle;
        public int timeStep = 2;
        private int _wait;
        private GameObject _clone;


        private void Start()
        {
            #region Exceptions

            if (bullet == null)
            {
                Debug.Log("add bullet object to boss. Bullet is in Prefab folder");
                throw new UnassignedReferenceException();
            }

            if (player == null)
            {
                Debug.Log("Игрок не найден");
                throw new UnassignedReferenceException();
            }

            if (totalPosition == null)
            {
                Debug.Log(
                    "Вы не настроили позиции для босса." +
                    " Нужно перетащить на 'positions' обьект содержащий несколько позиций"
                );
                throw new UnassignedReferenceException();
            }

            #endregion

            _constantHeightPosition = transform.position.y;
            player = FindObjectOfType<PlayerControll>();

            var inChildren = totalPosition.GetComponentsInChildren<Transform>();
            _centerPositions = inChildren
                .Where(i => i.gameObject.name.StartsWith("center"))
                .ToList();
            _randPositions = inChildren
                .Where(i => i.gameObject.name.StartsWith("pos") && !i.gameObject.name.Equals(totalPosition.name))
                .ToList();

            #region Health

            _canvas = transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
            _healthSlider = transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();
            _healthSlider.maxValue = maxHealth;
            health = maxHealth;
            _canvas.worldCamera = Camera.main;
            if (_canvas.worldCamera != null) _canvas.transform.rotation = _canvas.worldCamera.transform.rotation;

            #endregion
        }

        private GameObject SpawnBullet(Vector3 position, Vector3 direction, float lifetime = 8f)
        {
            var tr = transform;
            var bull = ObjectPool.SharedInstance.GetPooledObject();
            if (bull == null) bull = Instantiate(bullet, tr.position, tr.rotation);
            bull.transform.position = position + tr.position;
            bull.transform.localRotation = Quaternion.Euler(direction);
            bull.GetComponent<Bullet>().MakeLinearMovement(20f);
            bull.GetComponent<Bullet>().parentTransform = gameObject.transform;
            bull.GetComponent<Bullet>().lifetime = lifetime;
            bull.GetComponent<DamageProperty>().Damage = 10f;
            bull.SetActive(true);

            return bull;
        }


        private Vector3 GetRandomPosition() => _randPositions[Random.Range(0, _randPositions.Count)].transform.position;

        private Vector3 GetCenterPosition() =>
            _centerPositions[Random.Range(0, _centerPositions.Count)].transform.position;

        private void Update()
        {
            if ((player.transform.position - transform.position).magnitude <= 75f && phase == Phases.Idle)
            {
                phase = Phases.First;
                StartAttack();
            }
            else if ((phase == Phases.First || phase == Phases.Idle) && health <= maxHealth / 2)
            {
                phase = Phases.Second;
                gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                _clone = Instantiate(gameObject, gameObject.transform.parent, true);
                _clone.GetComponent<BulletBoss>().maxHealth = maxHealth / 2f;
                _clone.GetComponent<BulletBoss>()._clone = gameObject;
                _clone.GetComponent<BulletBoss>().TeleportToRandPosition();
                _clone.GetComponent<BulletBoss>().StartAttack();
            }

            _canvas.transform.LookAt(_canvas.worldCamera.transform);
            // if (Input.GetKeyDown(KeyCode.G)) StartCoroutine(LineAttack());
            // if (Input.GetKeyDown(KeyCode.H)) TeleportToRandPosition();
            // if (Input.GetKeyDown(KeyCode.J)) TeleportToCenterPosition();
            // if (Input.GetKeyDown(KeyCode.K)) StartCoroutine(ChessCircleAttack());
            // if (Input.GetKeyDown(KeyCode.L)) StartCoroutine(RotationAttack());
            // if (Input.GetKeyDown(KeyCode.Y)) StartCoroutine(HomingAttack());
        }

        private void StartAttack()
        {
            InvokeRepeating(nameof(PhaseAttacks), 1, timeStep);
        }

        private void PhaseAttacks()
        {
            if (phase != Phases.First && phase != Phases.Second || !gameObject.activeSelf) return;
            if (_wait > 0)
            {
                _wait -= timeStep;
                return;
            }

            var attackNumber = phase switch
            {
                Phases.Second => Random.Range(0, 11),
                Phases.First => Random.Range(0, 7),
                _ => Random.Range(0, 6)
            };

            if (phase == Phases.Second && attackNumber == 1) attackNumber = 9;
            
            switch (attackNumber)
            {
                case 0:
                    TeleportToRandPosition();
                    StartCoroutine(CircleAttack());
                    break;
                case 1:
                    TeleportToCenterPosition();
                    StartCoroutine(RotationAttack(step: 360f / Random.Range(4, 7), rotationSpeed: Random.Range(20, 40)));
                    _wait = 8;
                    break;
                case 2:
                    TeleportToRandPosition();
                    StartCoroutine(LineAttack());
                    break;
                case 3:
                    TeleportToRandPosition();
                    StartCoroutine(HomingAttack());
                    break;
                case 4:
                    TeleportToRandPosition();
                    StartCoroutine(CircleHomingAttack());
                    break;
                case 5:
                    TeleportToRandPosition();
                    StartCoroutine(ChessCircleAttack(rows: 6));
                    _wait = 6;
                    break;
                case 6:
                    TeleportToRandPosition();
                    StartCoroutine(MultiHomingAttack(amount: Random.Range(5, 10), totalTime: 6f));
                    _wait = 6;
                    break;
                case 7:
                    TeleportToRandPosition();
                    break;
                case 8:
                    TeleportToRandPosition();
                    StartCoroutine(MultiHomingAttack(amount: Random.Range(10, 15), totalTime: 6f));
                    _wait = 6;
                    break;
                case 9:
                    TeleportToRandPosition();
                    StartCoroutine(MultiHomingAttack(amount: Random.Range(10, 20), totalTime: 8f));
                    _wait = 8;
                    break;
                case 10:
                    TeleportToRandPosition();
                    StartCoroutine(MultiHomingAttack(amount: Random.Range(8, 10), totalTime: 6f));
                    _wait = 6;
                    break;
                default:
                    TeleportToRandPosition();
                    break;
            }
        }

        private IEnumerator CircleAttack(float radius = 10, float step = 30, float offset = 0)
        {
            for (var i = 0 + offset; i < 360 + offset; i += step)
                SpawnBullet(
                    new Vector3(Mathf.Cos(Mathf.Deg2Rad * i) * radius, -3, Mathf.Sin(Mathf.Deg2Rad * i) * radius),
                    new Vector3(0, -i + 90, 0));
            yield break;
        }

        private IEnumerator LineAttack(float radius = 5, float step = 72, float offset = 0)
        {
            var bull = SpawnBullet(new Vector3(0, -3, 0), Vector3.forward);
            bull.transform.LookAt(player.transform.position);
            var direction = new Vector3(0, bull.transform.rotation.eulerAngles.y, 0);
            bull.transform.localRotation = Quaternion.Euler(direction);
            for (var i = 0 + offset; i < 360 + offset; i += step)
                SpawnBullet(
                    new Vector3(Mathf.Cos(Mathf.Deg2Rad * i) * radius, -3, Mathf.Sin(Mathf.Deg2Rad * i) * radius),
                    direction);
            yield break;
        }

        private IEnumerator CircleHomingAttack(float radius = 10,
            float step = 45,
            float offset = 0,
            float rotationSpeed = 50f,
            float moveSpeed = 30f)
        {
            for (var i = 0 + offset; i < 360 + offset; i += step)
            {
                var bull = SpawnBullet(
                    new Vector3(Mathf.Cos(Mathf.Deg2Rad * i) * radius, -3, Mathf.Sin(Mathf.Deg2Rad * i) * radius),
                    new Vector3(0, -i + 90, 0));
                bull.GetComponent<Bullet>().MakeHomingMovement(player.transform, rotationSpeed, moveSpeed);
            }

            yield break;
        }

        private IEnumerator RotationAttack(float radius = 50,
            float step = 90,
            float stepRadius = 10,
            float offset = 0,
            float offsetRadius = 8,
            float rotationSpeed = 20,
            float lifetime = 8f)
        {
            for (var i = 0f + offset; i < 360 + offset; i += step)
            for (var j = 0f + offsetRadius; j < radius + offsetRadius; j += stepRadius)
            {
                var bull = SpawnBullet(
                    new Vector3(Mathf.Cos(Mathf.Deg2Rad * i) * j, -3, Mathf.Sin(Mathf.Deg2Rad * i) * j),
                    new Vector3(0, -i - 180, 0), lifetime);
                bull.GetComponent<Bullet>().MakeCircularMovement(rotationSpeed, transform);
                bull.GetComponent<Bullet>().reflective = false;
                bull.GetComponent<DamageProperty>().Damage = 20f;
                bull.transform.localScale = Vector3.one * 4;
                bull.transform.LookAt(player.transform.position);
            }

            yield break;
        }

        private IEnumerator HomingAttack(float rotationSpeed = 60, float moveSpeed = 30f, float lifetime = 8f)
        {
            var bull = SpawnBullet(new Vector3(0, -3, 0), Vector3.forward, lifetime);
            bull.transform.LookAt(player.transform.position);
            bull.GetComponent<Bullet>().MakeHomingMovement(player.transform, rotationSpeed, moveSpeed);
            yield break;
        }

        private IEnumerator MultiHomingAttack(float rotationSpeed = 60, float moveSpeed = 30f, float lifetime = 8f,
            float amount = 6f, float totalTime = 5f)
        {
            for (var i = 0; i < amount; i++)
            {
                StartCoroutine(HomingAttack(rotationSpeed, moveSpeed, lifetime));
                yield return new WaitForSeconds(totalTime / amount);
            }
        }

        private IEnumerator ChessCircleAttack(float radius = 10,
            float step = 30,
            float offset = 0,
            int rows = 5,
            float offsetTime = 1f)
        {
            for (var i = 0; i < rows; i++)
            {
                StartCoroutine(CircleAttack(radius, step, offset + step * i / 2));
                yield return new WaitForSeconds(offsetTime);
            }
        }

        private void TeleportToPosition(Vector3 position, bool doRotate = true) // teleport and rotate to player
        {
            position = new Vector3(position.x, _constantHeightPosition, position.z);
            transform.position = position;
            if (doRotate) RotateToTarget();
        }

        private void TeleportToCenterPosition()
        {
            var pos = GetCenterPosition();
            // проверка на телепорт в ту же позицию
            for (var i = 0; i <= 1000; i++)
                if ((new Vector2(pos.x, pos.z) - new Vector2(transform.position.x, transform.position.z)).magnitude <=
                    20)
                    pos = GetCenterPosition();
                else
                    break;

            TeleportToPosition(pos);
            var rotation = transform.rotation;
            rotation = Quaternion.Euler(rotation.x, 0, rotation.z);
            transform.rotation = rotation;
        }

        private void TeleportToRandPosition()
        {
            var pos = GetRandomPosition();
            // проверка на телепорт в ту же позицию или в позицию клона
            for (var i = 0; i <= 1000; i++)
                if ((new Vector2(pos.x, pos.z) - new Vector2(transform.position.x, transform.position.z)).magnitude <=
                    20)
                    pos = GetRandomPosition();
                else if (_clone != null &&
                         (new Vector2(pos.x, pos.z) -
                          new Vector2(_clone.transform.position.x, _clone.transform.position.z)).magnitude <= 20)
                    pos = GetRandomPosition();
                else
                    break;

            TeleportToPosition(pos);
        }

        private void RotateToTarget()
        {
            var lookVector = player.transform.position - transform.position;
            lookVector.y = 0;
            if (lookVector == Vector3.zero) return;
            transform.rotation = Quaternion.RotateTowards
            (
                transform.rotation,
                Quaternion.LookRotation(lookVector, Vector3.up),
                float.MaxValue
            );
        }

        private void TakeDamage(float? dmg)
        {
            dmg ??= 0;
            health -= (float)dmg;
            if (health == 0) Kill();
            
        }

        private void Kill()
        {
            
            gameObject.SetActive(false);
            Destroy(gameObject, 8f);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Hit") && other.gameObject.GetComponent<Bullet>() != null)
            {
                var dmg = other.gameObject.GetComponent<DamageProperty>().Damage;
                TakeDamage(dmg);
            }
        }
    }
}