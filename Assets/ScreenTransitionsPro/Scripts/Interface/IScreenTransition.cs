// (c) Andrey Torchinskiy, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScreenTransitionsPro
{
	public interface IScreenTransition
	{
		void SetTransitioning(bool t);

		void SetMaterial(Material m);

		void SetCutoff(float c);

		void SetFalloff(float f);

		void SetBackgroundColor(Color bc);

		void SetBackgroundTexture(Texture tex);

		void SetFitToScreen(bool fts);

		void SetHorizontalFlip(bool hf);

		void SetVerticalFlip(bool vf);

		void SetInvert(bool i);

		void AddFocus(Transform f);

		void RemoveFocus(Transform f);

		void SetNoiseScale(float s);

		void SetNoiseVelocity(Vector2 v);
	}
}
