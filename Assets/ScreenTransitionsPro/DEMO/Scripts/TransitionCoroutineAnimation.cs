// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

using System.Collections;
using UnityEngine;
using ScreenTransitionsPro;

public class TransitionCoroutineAnimation : MonoBehaviour
{
	public float transitionDuration = 1f;
	public bool invertFadeIn = true;

	private ScreenTransitionSimple _transition;
	private bool _fadeOut = true;
	
	void Start()
    {
		_transition = GetComponent<ScreenTransitionSimple>();
		_transition.transitioning = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
			StopAllCoroutines();

			if (_fadeOut)
			{
				StartCoroutine(FadeOut());
				_fadeOut = false;
				if (invertFadeIn)
				{
					_transition.invert = false;
				}
			}
			else
			{
				StartCoroutine(FadeIn());
				_fadeOut = true;
				if (invertFadeIn)
				{
					_transition.invert = true;
				}
			}
		}
    }

	IEnumerator FadeOut()
	{
		while (_transition.cutoff < 1f)
		{
			_transition.cutoff += Time.deltaTime / transitionDuration;
			yield return 0;
		}
	}

	IEnumerator FadeIn()
	{
		while (_transition.cutoff > 0f)
		{
			_transition.cutoff -= Time.deltaTime / transitionDuration;
			yield return 0;
		}
	}

}
