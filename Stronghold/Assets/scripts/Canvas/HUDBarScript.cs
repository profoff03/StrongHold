using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class HUDBarScript : MonoBehaviour
{
    [SerializeField] PlayerControll playerControll;
    public GameObject blood;

    [Header("Shield")] private bool CanTakeShields = false;
    private float DefaultShieldCoolDown;
    public float MaxShieldCoolDown;
    public Image Shield;


    [Header("Smoke")] private bool CanSmoke = false;
    private float DefaultSmokeCoolDown;
    public float MaxSmokeCoolDown;
    public Image SmokeBar;

    [Header("Dash")] internal bool CanDash = false;
    internal float DefaultDashCoolDown;
    public float MaxDashCoolDown;
    public Image DashBar;


    [Header("Ultimate")] public Image Ultimate;

    [Header("HP")] public Image HPBar;
    [SerializeField] float maxHP;
    internal float HP;
    public float MaxShieldHp = 15;
    private float? _shieldHp;


    void Start()
    {
        DefaultDashCoolDown = MaxDashCoolDown;
        DefaultShieldCoolDown = MaxShieldCoolDown;
        DefaultSmokeCoolDown = MaxSmokeCoolDown;
        HP = maxHP / 100;
        _shieldHp = MaxShieldHp;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDashCoolDown();

        CheckShieldCoolDown();

        CheckSmokeCoolDown();

        if (playerControll.isUlting) // ulta
            Ultimate.fillAmount -= Time.deltaTime / playerControll._ultTime;
        else
            Ultimate.fillAmount += Time.deltaTime / playerControll._ultRegenerateTime;
    }

    void CheckDashCoolDown()
    {
        if (DefaultDashCoolDown < MaxDashCoolDown && !playerControll._playerAnimator.GetBool("isDash"))
        {
            DefaultDashCoolDown += Time.deltaTime / 2;
            CanDash = false;
        }
        else
        {
            CanDash = !playerControll.IsAnimationPlaying("ULTIMATE", 0);

            if (CanDash && Input.GetKeyDown(KeyCode.Space) && !playerControll.isStan)
            {
                playerControll._playerAnimator.SetBool("isDash", true);
                UseDash();
            }
        }

        float newScale = DefaultDashCoolDown / MaxDashCoolDown;
        DashBar.fillAmount = newScale;
    }

    void CheckShieldCoolDown()
    {
        if (_shieldHp <= 0.001f)
        {
            _shieldHp = MaxShieldHp;
            CanTakeShields = false;
            playerControll._playerAnimator.SetBool("isShield", false);
        }

        if (DefaultShieldCoolDown < MaxShieldCoolDown && !playerControll._playerAnimator.GetBool("isShield"))
        {
            CanTakeShields = false;
            DefaultShieldCoolDown += Time.deltaTime / 2;
        }
        else
        {
            if (playerControll.isAtack || playerControll.IsAnimationPlaying("Death", 0) ||
                playerControll.IsAnimationPlaying("ULTIMATE", 0) || playerControll.isStan)
            {
                CanTakeShields = false;
            }
            else
            {
                CanTakeShields = true;
            }

            if (CanTakeShields && Input.GetKey(KeyCode.Mouse1) && !playerControll.isStan)
            {
                
                playerControll._playerAnimator.SetBool("isShield", true);
                UseShield();
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

            if (CanSmoke && Input.GetKeyDown(KeyCode.G) && !playerControll.isStan)
            {
                playerControll._playerAnimator.SetBool("isSmoke", true);
                UseSmoke();
                playerControll.SpawnBomb();
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
        //Debug.Log($"Получен удар на {dmg}");

        dmg ??= 0;

        if (playerControll._playerAnimator.GetBool("isShield")) _shieldHp -= dmg;
        
        dmg = playerControll._playerAnimator.GetBool("isShield") ? 0 : dmg;


        HP -= (float)dmg / 300;

        if (HP <= 0.001) HP = 0f;

        if (HP == 0) playerControll._playerAnimator.SetTrigger("isDie");

        HPBar.fillAmount = HP;
    }

    void UseDash()
    {
        if (DefaultDashCoolDown < MaxDashCoolDown) return;

        if (playerControll._playerAnimator.GetBool("isDash") == true)
        {
            DefaultDashCoolDown = 0;
        }
    }

    void UseShield()
    {
        if (DefaultShieldCoolDown < MaxShieldCoolDown) return;

        if (playerControll._playerAnimator.GetBool("isShield") == true)
        {
            _shieldHp = MaxShieldHp;
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