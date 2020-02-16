using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public SpriteRenderer TimeGaugeSpr;
    public Image FirstTimeBarImg;
    public Image SecondTimeBarImg;
    private Vector2 InitialGaugePos;
    private Vector2 InitialBarPos;

    private TimedAction BarAdjustTimer;

    public float BarTimeHeight;
    public float BarDeltaRate;
    public float TargetTime;
    public float CurrentTime;
    public float DisplaySpeed;

    public bool IsDisplayed;

    void Start()
    {
        // FirstTimeBarImg = transform.Find("FirstTimeBar").GetComponent<Image>();
        // SecondarTimeBarImg = transform.Find("SecondTimeBar").GetComponent<Image>();

        // var fuseGO = SprHolder.transform.Find("Fuse");
        // FuseSpr = fuseGO.GetComponent<SpriteRenderer>();
        // FuseAnim = fuseGO.GetComponent<Animator>();
        // FuseAnim.enabled = false;
        // FuseAnim.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        InitialGaugePos = TimeGaugeSpr.transform.localPosition;
        InitialBarPos = FirstTimeBarImg.transform.localPosition;
        BarAdjustTimer = null;
    }

    private void Deinitialize()
    {
        //FuseAnim.gameObject.SetActive(false);

        if(BarAdjustTimer != null) { 
            MainController.MC.RemoveTimedAction(BarAdjustTimer); 
            BarAdjustTimer = null;
        }
    }

    void Destroy()
    {
        Deinitialize();
    }

    public void SetTargetTime(float time)
    {
        TargetTime = time;

        if(BarAdjustTimer == null) {
            BarAdjustTimer = MainController.MC.AddTimedAction(AdjustBar, 0.0001f, true);
        }
    }

    public void ResetTime()
    {
        CurrentTime = TargetTime = 0;
        FirstTimeBarImg.fillAmount = SecondTimeBarImg.fillAmount = 0;

        AdjustBar();
    }

    public void AdjustBar()
    {
        var deltaTime = Time.deltaTime;

        if(CurrentTime < TargetTime)
        {
            CurrentTime += deltaTime * BarDeltaRate;
            if(CurrentTime > TargetTime) { CurrentTime = TargetTime; }
        }
        else
        {
            CurrentTime -= deltaTime * BarDeltaRate;
            if(CurrentTime < TargetTime) { CurrentTime = TargetTime; }
        }

        if(CurrentTime > BarTimeHeight)
        {
            FirstTimeBarImg.fillAmount = 1;
            SecondTimeBarImg.fillAmount = (CurrentTime - BarTimeHeight) / BarTimeHeight;
        }
        else
        {
            FirstTimeBarImg.fillAmount = CurrentTime / BarTimeHeight;
            SecondTimeBarImg.fillAmount = 0;
        }

        if(CurrentTime == TargetTime && BarAdjustTimer != null) { 
            BarAdjustTimer.IsContinuous = false;
            BarAdjustTimer = null;
            return; 
        }
    }

    public void MoveBar(bool isShow)
    {
        var nextGaugePos = InitialGaugePos;
        var nextBarPos = InitialBarPos;

        if(isShow)
        {
            var deltaX = -TimeGaugeSpr.bounds.size.x;
            var barDeltaX = (deltaX / FirstTimeBarImg.transform.lossyScale.x) * FirstTimeBarImg.transform.localScale.x;
            var barDeltaVector = new Vector2(barDeltaX, 0);

            nextGaugePos += new Vector2(deltaX, 0);
            nextBarPos += barDeltaVector;
        }

        MainController.MC.TransformManager.Add_LinearTimePos_Transform(TimeGaugeSpr.gameObject, nextGaugePos, DisplaySpeed);
        MainController.MC.TransformManager.Add_LinearTimePos_Transform(FirstTimeBarImg.gameObject, nextBarPos, DisplaySpeed);
        MainController.MC.TransformManager.Add_LinearTimePos_Transform(SecondTimeBarImg.gameObject, nextBarPos, DisplaySpeed);

        IsDisplayed = isShow;
    }

    private void ToggleFuse()
    {
        // var moveDist = Mathf.Ceil(FirstBarSpr.bounds.size.x * 1.5f);
        // var finalPos = new Vector2(SprHolder.transform.localPosition.x, SprHolder.transform.localPosition.y + (IsDisplayed ? -moveDist : moveDist));
        // MainController.MC.TransformManager.Add_LinearTimePos_Transform(SprHolder, finalPos, 1, StopFuse);
    }
}