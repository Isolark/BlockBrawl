// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace ScreenTransitionsPro
{
	[ExecuteInEditMode]
	[AddComponentMenu("Screen Transitions Pro/Multi Focus")]
	public class ScreenTransitionMultiFocus : MonoBehaviour, IScreenTransition
	{
		#region Variables

		/// <summary>
		/// Material that will be applied to rendered image during transition.
		/// </summary>
		[Tooltip("Material that will be applied to rendered image during transition.")]
		public Material transitionMaterial;

		/// <summary>
		/// Background color that will be used during transition.
		/// </summary>
		[Tooltip("Background color that will be used during transition.")]
		public Color backgroundColor = Color.black;

		/// <summary>
		/// Texture that will be used as background during transition.
		/// Render Texture also allowed.
		/// </summary>
		[Tooltip("Texture that will be used as background during transition. Render Texture also allowed.")]
		public Texture backgroundTexture;

		public enum BackgroundType
		{
			COLOR,
			TEXTURE
		}
		/// <summary>
		/// Defines what type background will be used during transition.
		/// </summary>
		[Tooltip("Defines what type background will be used during transition.")]
		public BackgroundType backgroundType;

		/// <summary>
		/// Represents current progress of the transition.
		/// 0 - no transition
		/// 1.5 - full transition to background color.
		/// </summary>
		[Range(0f, 1.5f), Tooltip("Represents current progress of the transition.")]
		public float cutoff = 0f;

		/// <summary>
		/// Flag that tells Unity to process transition. 
		/// Set this flag at the beginning of the transition and unset at the end 
		/// to avoid unnecessary calculations and save some performance.
		/// </summary>
		[Tooltip("Flag that tells Unity to process transition. Set this flag at the beginning of the transition and unset it at the end to avoid unnecessary calculations and to save some performance.")]
		public bool transitioning;

		/// <summary>
		/// Positions of the Transforms from this list will be used as the centers of the transition's circles.
		/// Only first 5 entries will be taken into account.
		/// Empty entries will be ignored.
		/// When all Focus objects are Null or placed behind the camera, center of the screen will be used as the target instead.
		/// </summary>
		[Tooltip("Positions of the Transforms from this list will be used as the centers of the transition's circles. Only first 5 entries will be taken into account. Empty entries will be ignored. When all Focus objects are Null or placed behind the camera, center of the screen will be used as the target instead.")]
		public List<Transform> focus;

		/// <summary>
		/// Reference to the camera component.
		/// </summary>
		private Camera _cam;

		#endregion

		#region Unity Callbacks

		private void Start()
		{
			if (transitionMaterial)
			{
				switch (backgroundType)
				{
					case BackgroundType.COLOR:
						transitionMaterial.DisableKeyword("USE_TEXTURE");
						break;
					case BackgroundType.TEXTURE:
						transitionMaterial.EnableKeyword("USE_TEXTURE");
						break;
				}
			}

			_cam = GetComponent<Camera>();
		}

		private void LateUpdate()
		{
			if (_cam == null)
			{
				Debug.LogError("There is no Camera component at " + gameObject.name);
				return;
			}

			if (transitioning && transitionMaterial)
			{
				// Set circle's center to the center of the screen
				transitionMaterial.SetFloat("_FocusX1", 0.5f);
				transitionMaterial.SetFloat("_FocusY1", 0.5f);
				transitionMaterial.SetInt("_Focus1", 1);
				transitionMaterial.SetInt("_Focus2", 0);
				transitionMaterial.SetInt("_Focus3", 0);
				transitionMaterial.SetInt("_Focus4", 0);
				transitionMaterial.SetInt("_Focus5", 0);
				transitionMaterial.SetFloat("_Cutoff", cutoff);

				switch (backgroundType)
				{
					case BackgroundType.COLOR:
						transitionMaterial.SetColor("_Color", backgroundColor);
						break;
					case BackgroundType.TEXTURE:
						transitionMaterial.SetTexture("_Texture", backgroundTexture);
						break;
				}

				if (focus.Count > 0)
				{
					for (int i = 0; i < focus.Count; i++)
					{
						if (i > 4)
						{
							break; // Take into account only first 5 entries
						}

						if (focus[i] == null)
						{
							continue; // Ignore empty entries
						}

						// Check if current focus Transform is behind the camera
						Vector3 dir = (focus[i].position - transform.position).normalized;
						float dot = Vector3.Dot(transform.forward, dir);

						if (dot > 0)
						{
							Vector2 screenPos = _cam.WorldToViewportPoint(focus[i].position);
							transitionMaterial.SetFloat("_FocusX" + (i + 1), screenPos.x);
							transitionMaterial.SetFloat("_FocusY" + (i + 1), screenPos.y);
							transitionMaterial.SetInt("_Focus" + (i + 1), 1);
						}
					}
				}
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (transitioning && transitionMaterial && _cam)
			{
				Graphics.Blit(source, destination, transitionMaterial);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}

		#endregion

		#region Interface Implementation

		public void SetTransitioning(bool t)
		{
			transitioning = t;
		}

		public void SetMaterial(Material m)
		{
			transitionMaterial = m;
		}

		public void SetCutoff(float c)
		{
			cutoff = Mathf.Clamp(c, 0f, 1.5f);
		}

		public void SetFalloff(float f)
		{
			Debug.LogWarning("Current screen transition doesn't support falloff. Value will be ignored.");
		}

		public void SetBackgroundColor(Color bc)
		{
			backgroundColor = bc;
		}

		public void SetBackgroundTexture(Texture tex)
		{
			backgroundTexture = tex;
		}

		public void SetFitToScreen(bool fts)
		{
			Debug.LogWarning("Current screen transition doesn't support fit to screen. Value will be ignored.");
		}

		public void SetHorizontalFlip(bool hf)
		{
			Debug.LogWarning("Current screen transition doesn't support horizontal flip. Value will be ignored.");
		}

		public void SetVerticalFlip(bool vf)
		{
			Debug.LogWarning("Current screen transition doesn't support vertical flip. Value will be ignored.");
		}

		public void SetInvert(bool i)
		{
			Debug.LogWarning("Current screen transition doesn't support invert. Value will be ignored.");
		}

		public void AddFocus(Transform f)
		{
			if (!focus.Contains(f))
			{
				focus.Add(f);
			}
			else
			{
				Debug.LogWarning("Provided Transform is already transition focus. Value will be ignored.");
			}
		}

		public void RemoveFocus(Transform f)
		{
			if (focus.Contains(f))
			{
				focus.Remove(f);
			}
			else
			{
				Debug.LogWarning("Provided Transform is not current transition focus. Value will be ignored.");
			}
		}

		public void SetNoiseScale(float s)
		{
			Debug.LogWarning("Current screen transition doesn't support noise scale. Value will be ignored.");
		}

		public void SetNoiseVelocity(Vector2 v)
		{
			Debug.LogWarning("Current screen transition doesn't support noise velocity. Value will be ignored.");
		}

		#endregion
	}
}
