using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class HUDBarScript : MonoBehaviour
{
    [SerializeField] 
    public float maxHP = 100f;
    [SerializeField] 
    internal float HP = 100f;

    [SerializeField]
    PlayerControll playerControll;

    public GameObject blood;
    public float MaxCoolDown;
    private float DefaultCoolDown;

    public Image Ultimate;
    public Image Skills;
    public Image HPBar;

    private bool CanTakeShields = false;


    void Start()
    {
        DefaultCoolDown = MaxCoolDown;
        HP = maxHP/100;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCoolDown();
        if (Input.GetKey(KeyCode.Mouse1))//Input.GetKey(KeyCode.E)
        {
            UseSkill();
            playerControll.noOfClick = 0;
            playerControll.canClick = true;
        }


        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        if (playerControll.isUlting) // ulta
            Ultimate.fillAmount -= Time.deltaTime / playerControll._ultTime;
        else
            Ultimate.fillAmount += Time.deltaTime / playerControll._ultRegenerateTime;

    }

    void CheckCoolDown()
    {
        
        if (DefaultCoolDown < MaxCoolDown && !playerControll._playerAnimator.GetBool("isAbil"))
        {
            CanTakeShields = false;
            DefaultCoolDown += Time.deltaTime / 2;

        }
        else
        {
            if (playerControll._playerAnimator.GetInteger("isAttackPhase") !=0 || playerControll.IsAnimationPlaying("ULTIMATE", 0))
            {
                CanTakeShields = false;
            }
            else
            {
                CanTakeShields = true;
            }
            if (CanTakeShields && Input.GetKey(KeyCode.Mouse1))
            {
                playerControll._playerAnimator.SetBool("isAbil", true);
                playerControll._playerAnimator.SetInteger("isAttackPhase", 0);
            }
            else
            {
                playerControll._playerAnimator.SetBool("isAbil", false);
            }
        }

        float newScale = DefaultCoolDown / MaxCoolDown;
        Skills.fillAmount = newScale;


    }

    internal void TakeDamage(float? dmg)
    {

        //Debug.Log($"Получен удар на {dmg}
        dmg ??= 0;
        HP -= (float)dmg/100;
        

        if (HP <= 0.001) HP = 0f;
        
        if (HP == 0)
        {
            playerControll._playerAnimator.SetTrigger("isDie");
        }

        HPBar.fillAmount = HP;
    }

    void UseSkill()
    {
        if (DefaultCoolDown < MaxCoolDown) return;

        if (playerControll._playerAnimator.GetBool("isAbil") == true)
        {
            DefaultCoolDown = 0;
        }

        //HP -= 10.0f;


        
    }
}
