using System;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Image HPBarImage;

    public int MaxHP;
    public int HP;
    public int RegenAmount;
    public float RegenRate;
    public float RegenDelay;
    private TimedAction RegenTimer;
    public bool IsRegenActive => RegenTimer != null;

    public int DamageAmount;
    public float DamageRate; //Dependent on SpeedLv
    private TimedAction DamageTimer;
    public bool IsTakingDamage => DamageTimer != null;

    private Action DeathCallback;

    public void Initialize(int maxHP, int regenAmount, float regenRate, float regenDelay, int dmgAmount, float dmgRate, Action deathCallback)
    {
        HP = MaxHP = maxHP;
        RegenAmount = regenAmount;
        RegenRate = regenRate;
        RegenDelay = regenDelay;

        DamageAmount = dmgAmount;
        RegenRate = dmgRate;

        DeathCallback = deathCallback;
    }

    private void Deinitialize()
    {
        if(RegenTimer != null) { MainController.MC.RemoveTimedAction(RegenTimer); }
        if(DamageTimer != null) { MainController.MC.RemoveTimedAction(DamageTimer); }
    }

    void Destroy()
    {
        Deinitialize();
    }

    public void StartRegen()
    {
        if(!IsTakingDamage && !IsRegenActive) { RegenTimer = MainController.MC.AddTimedAction(Regen, RegenRate, true); }
    }

    public void Regen()
    {
        HP += MaxHP;

        if(HP >= MaxHP)
        {
            HP = MaxHP;
            MainController.MC.RemoveTimedAction(RegenTimer);
        }

        UpdateDisplay();
    }

    public void StartDamage()
    {
        if(IsRegenActive) { 
            MainController.MC.RemoveTimedAction(RegenTimer); 
        }
        if(IsTakingDamage) { 
            DamageTimer = MainController.MC.AddTimedAction(TakeDamage, DamageRate, true); 
        }
    }

    public void StopDamage()
    {
        if(IsTakingDamage) { 
            MainController.MC.RemoveTimedAction(DamageTimer); 
        }
        if(HP < MaxHP && !IsRegenActive) {
            //MainController.MC.AddTimedAction()
        }
    }

    private void TakeDamage()
    {
        HP -= DamageAmount;

        if(HP <= 0)
        {
            HP = 0;
            Deinitialize();
            DeathCallback();
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        HPBarImage.fillAmount = HP / MaxHP;
    }
}