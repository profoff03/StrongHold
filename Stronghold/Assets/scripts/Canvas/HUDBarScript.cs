using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class HUDBarScript : MonoBehaviour
{
    private bool canPlayHitSound = true;

    #region Save
    private float HPAmount;
    private float UltAmount;
    #endregion
    [SerializeField]
    PlayerControll playerControll;
    public GameObject blood;
    [Header("PushShield")]
    private bool CanPushAway = false;


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

    [Header("Dash")]
    internal bool CanDash = false;
    internal float DefaultDashCoolDown;
    public float MaxDashCoolDown;
    public Image DashBar;

    [Header("Heal")]
    private bool canHeal = true;
    internal bool isHeal = true;
    internal bool CanUseHeal = false;
    internal float DefaultHealCoolDown;
    public float MaxHealCoolDown;
    public float HealStrength;
    public Image HealBar;


    [Header("Ultimate")]
    public Image Ultimate;

    [Header("HP")]
    public Image HPBar;
    [SerializeField]
    float maxHP;
    public float HP;

    internal bool canUseSmoke = false;

    [Header("Effects")]
    [SerializeField]
    GameObject healEffect;
    ParticleSystem.MainModule healParticleSystem;

    void Start()
    {
        healParticleSystem = healEffect.GetComponentsInChildren<ParticleSystem>()[2].main;

        DefaultDashCoolDown = MaxDashCoolDown;
        DefaultShieldCoolDown = MaxShieldCoolDown;
        DefaultSmokeCoolDown = 0;
        HP = maxHP/100;
        Load(); //TODO
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) { Save(); }//TODO
        if (playerControll.canDoSmth)
        {
            CheckDashCoolDown();

            CheckShieldCoolDown();

            if (!isHeal)
                CheckHealCoolDown();

            if (canUseSmoke)
                CheckSmokeCoolDown();

            if (playerControll.isUlting) // ulta
                Ultimate.fillAmount -= Time.deltaTime / playerControll._ultTime;
            if (Ultimate.fillAmount >= 1 && !playerControll.isUlting)
            {
                playerControll.ultRegenerate = false;
                Ultimate.fillAmount = 1;
            }else playerControll.ultRegenerate = true;
        }
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
            if (playerControll.IsAnimationPlaying("ULTIMATE", 0))
            {
                CanDash = false;
            }
            else
            {
                CanDash = true;
            }

            if (CanDash && Input.GetKeyDown(KeyCode.Space) && !playerControll.isStan)
            {
                playerControll._audioSource[1].PlayOneShot(playerControll._atkVoiceSound[Random.Range(0, playerControll._atkVoiceSound.Length)]);
                playerControll._audioSource[0].PlayOneShot(playerControll._dashSound[Random.Range(0, playerControll._dashSound.Length)]);
                playerControll._playerAnimator.SetBool("isDash", true);
                UseDash();
            }
            

        }

        float newScale = DefaultDashCoolDown / MaxDashCoolDown;
        DashBar.fillAmount = newScale;


    }
    void CheckHealCoolDown()
    {

        if (DefaultHealCoolDown < MaxHealCoolDown)
        {
            DefaultHealCoolDown += Time.deltaTime / 2;
            CanUseHeal = false;
        }
        else
        {
            if (playerControll.IsAnimationPlaying("ULTIMATE", 0))
            {
                CanUseHeal = false;
            }
            else
            {
                CanUseHeal = true;
            }

            if (CanUseHeal && Input.GetKeyDown(KeyCode.F) && !playerControll.isStan && HP != maxHP)
            {
                canHeal = true;
                StartCoroutine(HealAbbillity());
                DefaultHealCoolDown = 0;
            }


        }

        float newScale = DefaultHealCoolDown / MaxHealCoolDown;
        HealBar.fillAmount = newScale;


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
            if (playerControll.isAtack || playerControll.IsAnimationPlaying("Death", 0) || playerControll.IsAnimationPlaying("ULTIMATE", 0) || playerControll.isStan)
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
        canHeal = false;
        
        //HP -=(float)(dmg / 100);
        if (!playerControll.isUlting)
        {
            Ultimate.fillAmount += (float)(dmg / 50);
            if (Ultimate.fillAmount >= 0.7)
            {
                playerControll.deBuff(1 - Ultimate.fillAmount/3);
            }
        }

        if (canPlayHitSound)
        {
            playerControll._audioSource[1].PlayOneShot(playerControll._hitVoiceSound[Random.Range(0, playerControll._hitVoiceSound.Length)]);
            canPlayHitSound = false;
            StartCoroutine(hitSoundDelay());
        }
        if (HP <= 0.001) HP = 0f;
        
        if (HP == 0)
        {
            playerControll._playerAnimator.SetTrigger("isDie");
        }

        HPBar.fillAmount = HP;
    }
    internal void StartHeal() => StartCoroutine(Heal());
    private IEnumerator Heal()
    {
        healEffect.SetActive(true);
        healParticleSystem.loop = true;
        while (HP <= maxHP / 100)
        {
            HP += 0.01f;
            HPBar.fillAmount = HP;
            isHeal = true;
            yield return new WaitForSeconds(0.1f);
        }
        if (HP > 1) HP = 1;
        isHeal =false;
        healParticleSystem.loop = false;
        isHeal = false;
        yield return new WaitForSeconds(2f);
        healEffect.SetActive(false);
    }
    private IEnumerator HealAbbillity()
    {
        float h = HP;
        healEffect.SetActive(true);
        healParticleSystem.loop = true;
        while (HP <= h + HealStrength / 100 && canHeal && HP < 1)
        {
            HP += 0.01f;
            HPBar.fillAmount = HP;
            isHeal = true;
            yield return new WaitForSeconds(0.2f);

        }
        healParticleSystem.loop = false;
        if (HP > 1) HP = 1;
        isHeal = false;
        healParticleSystem.loop = false;
        isHeal = false;
        yield return new WaitForSeconds(2f);
        healEffect.SetActive(false);

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
            DefaultShieldCoolDown = 0;
        }
        
    }
    void UseSmoke()
    {
        if (DefaultSmokeCoolDown < MaxSmokeCoolDown) return;

        if (playerControll._playerAnimator.GetBool("isSmoke") == true)
        {
            DefaultSmokeCoolDown = 0;
        }

    }
    private IEnumerator hitSoundDelay()
    {
        yield return new WaitForSeconds(5);
        canPlayHitSound = true;
    }



    private void Save()//TODO
    {
        PlayerPrefs.SetFloat("HPAmount", HP);
        PlayerPrefs.SetFloat("UltimateAmount", Ultimate.fillAmount);

        if (canHeal)
        {
            PlayerPrefs.SetInt("CanHealth", 1);
        }
        else { PlayerPrefs.SetInt("CanHealth", 0); }

        if (CanSmoke)
        {
            PlayerPrefs.SetInt("CanUseSmoke", 1);
        }
        else { PlayerPrefs.SetInt("CanUseSmoke", 0); }

        if (CanTakeShields)
        {
            PlayerPrefs.SetInt("CanUseShield", 1);
        }
        else { PlayerPrefs.SetInt("CanUseShield", 0); }
        
        if(CanDash)
        {
            PlayerPrefs.SetInt("CanDash", 1);
        }
        else { PlayerPrefs.SetInt("CanDash", 0); }

        if(CanPushAway)
        {
            PlayerPrefs.SetInt("CanPushAway", 1);
        } 
        else
        {
            PlayerPrefs.SetInt("CanPushAway", 0);
        }
    }

    private void Load()//TODO
    {
        if (PlayerPrefs.HasKey("CanHealth"))
        {
            if(PlayerPrefs.GetInt("CanHealth", 1))
            {
                canHeal = true;
            } else { canHeal = false; }
        }

        if (PlayerPrefs.HasKey("CanUseSmoke"))
        {
            if (PlayerPrefs.GetInt("CanUseSmoke", 1))
            {
                CanSmoke = true;
            }
            else { CanSmoke = false; }
        }

        if (PlayerPrefs.HasKey("CanUseShield"))
        {
            if (PlayerPrefs.GetInt("CanUseShield", 1))
            {
                CanTakeShields = true;
            }
            else { CanTakeShields = false; }
        }

        if (PlayerPrefs.HasKey("CanDash"))
        {
            if (PlayerPrefs.GetInt("CanDash", 1))
            {
                CanDash = true;
            }
            else { CanDash = false; }
        }
        if (PlayerPrefs.HasKey("CanPushAway"))
        {
            if (PlayerPrefs.GetInt("CanPushAway", 1))
            {
                CanPushAway = true;
            }
            else { CanPushAway = false; }
        }


        if (PlayerPrefs.HasKey("HPAmount"))
        {
            HPAmount = PlayerPrefs.GetFloat("HPAmount");
        }
        if (PlayerPrefs.HasKey("UltimateAmount"))
        {
            UltAmount = PlayerPrefs.GetFloat("UltimateAmount");
        }

        Ultimate.fillAmount = UltAmount;
        HP = HPAmount;

        if (Ultimate.fillAmount < 0.99)
        {
            playerControll.ultRegenerate = true;
        }
    }
}
