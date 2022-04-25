using System;
using Unity.Mathematics;
using UnityEngine;
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
    }
}