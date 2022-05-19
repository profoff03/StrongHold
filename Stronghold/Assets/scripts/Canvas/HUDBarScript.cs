using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class HUDBarScript : MonoBehaviour
{
    

    [SerializeField]
    PlayerControll playerControll;
    public GameObject blood;

    [Header("Shield")]
    private bool CanTakeShields = false;
    private float DefaultShieldCoolDown;
    public float MaxShieldCoolDown;   
    public Image Shield;


    [Header("Smoke")]
    private bool CanSmoke = false;
    private float DefaultSmokeCoolDown;
    public float MaxSmokeCoolDown;
    public Image SmokeBar;


    [Header("Ultimate")]
    public Image Ultimate;

    [Header("HP")]
    public Image HPBar;
    [SerializeField]
    float maxHP;
    internal float HP;


    


    void Start()
    {
        DefaultShieldCoolDown = MaxShieldCoolDown;
        DefaultSmokeCoolDown = MaxSmokeCoolDown;
        HP = maxHP/100;
    }

    // Update is called once per frame
    void Update()
    {
        CheckShieldCoolDown();
        if (Input.GetAxis("Shield") == 1)
        {
            UseShield();

        }

        CheckSmokeCoolDown();
        if (Input.GetKey(KeyCode.G))
        {
            UseSmoke();
        }

       

        if (playerControll.isUlting) // ulta
            Ultimate.fillAmount -= Time.deltaTime / playerControll._ultTime;
        else
            Ultimate.fillAmount += Time.deltaTime / playerControll._ultRegenerateTime;



    }

    void CheckShieldCoolDown()
    {
        
        if (DefaultShieldCoolDown < MaxShieldCoolDown && !playerControll._playerAnimator.GetBool("isShield"))
        {
            CanTakeShields = false;
            DefaultShieldCoolDown += Time.deltaTime / 2;

        }
        else
        {
            if (playerControll.isAtack || playerControll.IsAnimationPlaying("ULTIMATE", 0))
            {
                CanTakeShields = false;
            }
            else
            {
                CanTakeShields = true;
            }
            if (CanTakeShields && Input.GetKey(KeyCode.Mouse1))
            {
                playerControll._playerAnimator.SetBool("isShield", true);
            }
            else
            {
                playerControll._playerAnimator.SetBool("isShield", false);
            }
        }

        float newScale = DefaultShieldCoolDown / MaxShieldCoolDown;
        Shield.fillAmount = newScale;


    }

    void CheckSmokeCoolDown()
    {

        if (DefaultSmokeCoolDown < MaxSmokeCoolDown && !playerControll._playerAnimator.GetBool("isSmoke"))
        {
            CanSmoke = false;
            DefaultSmokeCoolDown += Time.deltaTime / 2;

        }
        else
        {
            if (playerControll.isAtack || playerControll.IsAnimationPlaying("ULTIMATE", 0))
            {
                CanSmoke = false;
            }
            else
            {
                CanSmoke = true;
            }
            if (CanSmoke && Input.GetKey(KeyCode.G))
            {
                playerControll._playerAnimator.SetBool("isSmoke", true);
                playerControll.SpawnBomb();
                playerControll._playerAnimator.SetInteger("isAttackPhase", 0);
               
            }
            else
            {
                playerControll._playerAnimator.SetBool("isSmoke", false);
            }
        }

        float newScale = DefaultSmokeCoolDown / MaxSmokeCoolDown;
        SmokeBar.fillAmount = newScale;


    }
    internal void TakeDamage(float? dmg)
    {

        Debug.Log($"Получен удар на {dmg}");
        
        HP -= (float)dmg/100;
        

        if (HP <= 0.001) HP = 0f;
        
        if (HP == 0)
        {
            playerControll._playerAnimator.SetTrigger("isDie");
        }

        HPBar.fillAmount = HP;
    }

    void UseShield()
    {
        if (DefaultShieldCoolDown < MaxShieldCoolDown) return;

        if (playerControll._playerAnimator.GetBool("isShield") == true)
        {
            DefaultShieldCoolDown = 0;
        }
        
    }
    void UseSmoke()
    {
        if (DefaultShieldCoolDown < MaxShieldCoolDown) return;

        if (playerControll._playerAnimator.GetBool("isSmoke") == true)
        {
            DefaultSmokeCoolDown = 0;
        }

    }
}
