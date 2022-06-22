using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class Enemy : MonoBehaviour
{
    public GameObject blood;

    public float maxHealth = 100F;
    public float health = -1F; // если значение -1, то автоматически выставляется максимальное значение

    private Canvas canvas;
    private Slider healthSlider;
    private Vector2 _force;

    void Start()
    {
        if (Abs(health - -1F) < Epsilon) health = maxHealth;
        if (health > maxHealth) health = maxHealth;
        canvas = transform.Find("HealthBar").gameObject.GetComponent<Canvas>();
        healthSlider = transform.Find("HealthBar/Panel/Slider").gameObject.GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        canvas.transform.rotation = canvas.worldCamera.transform.rotation;

        

    }

    void Update()
    {
        canvas.transform.LookAt(canvas.worldCamera.transform);
        transform.position -= new Vector3(_force.x, _force.y, 0);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }

    private void DoDamage(float? dmg)
    {

        Debug.Log($"Получен удар на {dmg}");
        dmg ??= 0;
        health -= (float)dmg;
        if (health <= 0.001) health = 0f;

        if (health == 0) Kill();
        healthSlider.value = health;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hit"))
        {
            DoDamage(other.GetComponent<DamageProperty>()?.Damage);
            Instantiate(blood, transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
        if (other.gameObject.CompareTag("Push"))
        {
            Debug.Log("push");
            if (other.transform.parent.gameObject.GetComponent<PlayerControll>())
            {
                var controll = other.transform.parent.gameObject.GetComponent<PlayerControll>();
                Debug.Log("player");
                var direction = transform.position - controll.transform.position;
                Debug.Log(direction);
                StartCoroutine(Push(direction.normalized * controll._puchForce));
            }
        }
    }

    private IEnumerator Push(Vector2 force)
    {
        _force = force;
        yield return new WaitForSeconds(1f);
        _force.x = 0;
        _force.y = 0;
    }
}