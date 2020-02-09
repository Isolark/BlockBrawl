﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuItem : MonoBehaviour
{
    public TMP_Text ItemText;
    public string ItemDescription;
    public string ActionName;
    public Vector2 ListLoc;
    public bool IsSelectable;
    public bool UseSingleCursor;
    public Slider LinkedSlider;

    public Vector3 InitialPos;
}