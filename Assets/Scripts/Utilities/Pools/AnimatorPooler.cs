using UnityEngine;

public class AnimatorPooler : ObjectPooler
{
    public static AnimatorPooler AP;

    override protected void Awake()
    {
        //Singleton pattern
        if (AP == null) {
            if(transform.parent.gameObject) {
                DontDestroyOnLoad(transform.parent.gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
            }

            AP = this;
        }
        else if (AP != this) {
            Destroy(gameObject);
        }
        
        base.Awake();
    }

    public GameObject GetPooledObject(string animCtrlName, bool isEnabled = false, Transform parent = null)
    {
        var animator = base.GetPooledObject("Animator", parent).GetComponent<Animator>();
        animator.runtimeAnimatorController = AnimatorLibrary.AL.GetAnimatorByName(animCtrlName);
        animator.enabled = isEnabled;

        return animator.gameObject;
    }
}