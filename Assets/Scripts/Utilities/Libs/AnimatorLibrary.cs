using UnityEngine;
using System.Collections.Generic;

public class AnimatorLibrary : MonoBehaviour
{
    public List<RuntimeAnimatorController> AnimatorList;

    public static AnimatorLibrary AL;

    void Awake()
    {
        //Singleton pattern
        if (AL == null) {
            AL = this;
        }
        else if (AL != this) {
            Destroy(gameObject);
        }     
    }

    public RuntimeAnimatorController GetAnimatorByName(string name)
    {
        return AnimatorList.Find(x => x.name == name);
    }
}