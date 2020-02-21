// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

using UnityEngine;
using ScreenTransitionsPro;

public class TransitionUpdateAnimation : MonoBehaviour
{
	public float transitionSpeed = 1f;

	private ScreenTransitionDisplacementSoft _transition;

	void Start()
    {
		_transition = GetComponent<ScreenTransitionDisplacementSoft>();
		_transition.transitioning = true;
    }

    void Update()
    {
		_transition.cutoff = Mathf.Abs(Mathf.Sin(Time.time * transitionSpeed) * (1 + _transition.falloff)) - _transition.falloff;
	}
}
