using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour
{
    [SerializeField]
    PlayerControll playerControl;
    public static StaminaScript singleton { get; private set; }

    void Awake()
    {
        singleton = this;
    }
    
    public float MaxStamina;

    public Image StaminaBar;
    [SerializeField]
    float StaminaIncreasedPerSecond;

    float UpdateStamina;

    internal bool CanRunning = true;
    bool IsRunning = false;
    bool CanRegenerate = false;
    float start;
    [SerializeField]
    float regTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        MaxStamina = 100f;
        UpdateStamina = MaxStamina;
        start = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (!playerControl._inSmoke && !playerControl.isStan)
            MinusStamina();
        
        if (UpdateStamina <= MaxStamina - 0.01 && UpdateStamina >= 1 && !IsRunning)
        {
            //animator.SetBool("isRunning", false);
            CanRegenerate = true;
            IsRunning = false;
            RegenerateStamina();
        }
        else
        {
            IsRunning = false;
            CanRegenerate = false;
            Stamina();


        }
        if (Time.time - start >= regTime)
        {
            CanRunning = true;
        }
        if (UpdateStamina < 1)
        {
            start = Time.time;
            UpdateStamina = 1;
            CanRunning = false;
        }
        
        //Stamina();




    }

    void MinusStamina() //working
    {


        if (playerControl._playerAnimator.GetBool("isRunning") && !IsRunning)
        {
            //animator.SetBool("isRunning", true);
            UpdateStamina -= StaminaIncreasedPerSecond * Time.deltaTime;
            CheckStamina();
            CanRegenerate = false;
            CanRunning = true;
            IsRunning = true;

        }
        else
        {
            //animator.SetBool("isRunning", false);
            IsRunning = false;
            CanRunning = false;
            CanRegenerate = true;
        }
    }


    void RegenerateStamina()
    {
        if (CanRegenerate && !IsRunning)
        {
            UpdateStamina += StaminaIncreasedPerSecond * Time.deltaTime;
            CheckStamina();
        }
    }

    void Stamina()
    {
        if (UpdateStamina <= 0.3)
        {
            //animator.SetBool("isRunning", false);
            UpdateStamina = 0;
            CheckStamina();
            CanRegenerate = true;
            CanRunning = false;
            RegenerateStamina();
        }
        else
        {
            CanRegenerate = false;
            CanRunning = true;
            
        }
    }

    void CheckStamina()
    {

        StaminaBar.fillAmount = UpdateStamina / MaxStamina;

    }
}

