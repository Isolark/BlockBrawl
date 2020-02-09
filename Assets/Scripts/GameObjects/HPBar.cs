// using System;
// using UnityEngine;

// public class HPBar : MonoBehaviour
// {
//     public GameObject SprHolder;
//     public SpriteMask BarMask;
//     private SpriteRenderer BarSpr;
//     private SpriteRenderer FuseSpr;
//     private Animator FuseAnim;

//     public int MaxHP;
//     public int HP;

//     public bool IsFuseOn;
//     private Vector2 BarMaskInitialPos;

//     public int RegenAmount;
//     public float RegenRate;
//     public float RegenDelay;
//     private TimedAction RegenTimer;
//     public bool IsRegenActive => RegenTimer != null;

//     public int DamageAmount;
//     public float DamageRate; //Dependent on SpeedLv
//     private TimedAction DamageTimer;
//     public bool IsTakingDamage => DamageTimer != null;

//     private Action DeathCallback;

//     public void Initialize(int maxHP, int regenAmount, float regenRate, float regenDelay, int dmgAmount, float dmgRate, Action deathCallback)
//     {
//         HP = MaxHP = maxHP;
//         RegenAmount = regenAmount;
//         RegenRate = regenRate;
//         RegenDelay = regenDelay;

//         DamageAmount = dmgAmount;
//         RegenRate = dmgRate;

//         DeathCallback = deathCallback;

//         BarSpr = SprHolder.transform.Find("BarSpr").GetComponent<SpriteRenderer>();

//         var fuseGO = SprHolder.transform.Find("FuseSpr");
//         FuseSpr = fuseGO.GetComponent<SpriteRenderer>();
//         FuseAnim = fuseGO.GetComponent<Animator>();
//         FuseAnim.enabled = false;
//         FuseAnim.gameObject.SetActive(false);

//         BarMaskInitialPos = BarMask.transform.localPosition;
//     }

//     private void Deinitialize()
//     {
//         if(RegenTimer != null) { MainController.MC.RemoveTimedAction(RegenTimer); }
//         if(DamageTimer != null) { MainController.MC.RemoveTimedAction(DamageTimer); }

//         FuseAnim.gameObject.SetActive(false);
//     }

//     void Destroy()
//     {
//         Deinitialize();
//     }


//     public void StartRegen()
//     {
//         if(!IsTakingDamage && !IsRegenActive) 
//         { 
//             RegenTimer = MainController.MC.AddTimedAction(Regen, RegenRate, true);

//             if(!IsFuseOn)
//             {
//                 FuseAnim.gameObject.SetActive(true);
//                 FuseSpr.color = Color.blue;
//                 FuseAnim.enabled = true;
//                 FuseAnim.SetTrigger("Play");

//                 IsFuseOn = true;
//             }
//         }
//         else { RegenTimer = null; }
//     }

//     public void Regen()
//     {
//         HP += RegenAmount;

//         if(HP >= MaxHP)
//         {
//             HP = MaxHP;
//             MainController.MC.RemoveTimedAction(RegenTimer);

//             if(IsFuseOn) { ToggleFuse(false); }

//             return;
//         }

//         UpdateFuse();
//     }

//     private void ToggleFuse(bool isHidden)
//     {
//         var moveDist = Mathf.Ceil(BarSpr.bounds.size.x * 1.5f);
//         var finalPos = new Vector2(SprHolder.transform.localPosition.x, SprHolder.transform.localPosition.y + (isHidden ? moveDist : -moveDist));
//         MainController.MC.TransformManager.Add_LinearTimePos_Transform(SprHolder, finalPos, DamageRate * 1.5f, StopFuse);
//     }

//     public void StartFuse()
//     {
//         if(!IsFuseOn && (IsTakingDamage || IsRegenActive))
//         {
//             //TODO: SFX?
//             FuseAnim.gameObject.SetActive(true);
//             FuseSpr.color = IsTakingDamage ? Color.red : Color.blue;
//             FuseAnim.SetTrigger("Play");
//             IsFuseOn = true;

//             ToggleFuse(true);
//         }
//     }

//     public void StopFuse()
//     {
//         if(IsFuseOn)
//         {
//             FuseSpr.transform.localPosition = new Vector2(BarSpr.transform.localPosition.x + BarSpr.bounds.size.y/2, FuseSpr.transform.localPosition.y);
//             FuseAnim.SetTrigger("Stop");
//             IsFuseOn = false;

//             ToggleFuse(false);
//         }
//     }

//     private void UpdateFuse()
//     {
//         var nextPos = BarSpr.bounds.size.y * (HP / MaxHP) * (IsTakingDamage ? -1 : 1);
//         FuseSpr.transform.localPosition = 

//     }

//     public void StartDamage()
//     {
//         if(IsRegenActive) { 
//             MainController.MC.RemoveTimedAction(RegenTimer); 
//         }
//         if(!IsTakingDamage) 
//         { 
//             DamageTimer = MainController.MC.AddTimedAction(TakeDamage, DamageRate, true);
            
//             if(!IsFuseOn)
//             {
//                 StartFuse();
//             }
//         }
//     }

//     public void StopDamage()
//     {
//         if(IsTakingDamage) { 
//             MainController.MC.RemoveTimedAction(DamageTimer);
//             DamageTimer = null;
//         }
//         if(HP < MaxHP && !IsRegenActive) {
//             MainController.MC.AddTimedAction(StartRegen, RegenDelay);
//         }
//     }

//     private void TakeDamage()
//     {
//         HP -= DamageAmount;

//         if(HP <= 0)
//         {
//             HP = 0;
            
//             StartDeath();
//             return;
//         }

//         UpdateFuse();
//     }

//     private void StartDeath()
//     {
//         FuseAnim.SetTrigger("Stop");
//         FuseAnim.enabled = false;

//         Deinitialize();
//         DeathCallback();
//     }
// }