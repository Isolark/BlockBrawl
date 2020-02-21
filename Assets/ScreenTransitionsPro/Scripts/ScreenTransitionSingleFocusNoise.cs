// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.

using UnityEngine;

namespace ScreenTransitionsPro
{
	[ExecuteInEditMode]
	[AddComponentMenu("Screen Transitions Pro/Single Focus Noise")]
	public class ScreenTransitionSingleFocusNoise : MonoBehaviour, IScreenTransition
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
		/// from 1.5 to 2.5 - full transition to background color (depends on the falloff).
		/// </summary>
		[Range(0f, 2.5f), Tooltip("Represents current progress of the transition.")]
		public float cutoff = 0f;

		/// <summary>
		/// Noisy blend between rendered texture and background color.
		/// 0 - no blend (sharp border)
		/// 1 - max blend. 
		/// </summary>
		[Range(0f, 1f), Tooltip("Noisy blend between rendered texture and background color.")]
		public float falloff = 0f;

		/// <summary>
		/// Size of the noise.
		/// </summary>
		[Tooltip("Size of the noise.")]
		public float noiseScale = 100f;

		/// <summary>
		/// Velocoty of the noise
		/// X - horizontal speed
		/// Y - vertical speed
		/// </summary>
		[Tooltip("Velocity of the noise. X - horizontal speed; Y - vertical speed.")]
		public Vector2 noiseVelocity = Vector2.zero;

		/// <summary>
		/// Flag that tells Unity to process transition. 
		/// Set this flag at the beginning of the transition and unset at the end 
		/// to avoid unnecessary calculations and save some performance.
		/// </summary>
		[Tooltip("Flag that tells Unity to process transition. Set this flag at the beginning of the transition and unset it at the end to avoid unnecessary calculations and to save some performance.")]
		public bool transitioning;

		/// <summary>
		/// Position of this Transform will be the center of the transition's circle.
		/// When Transform is Null or placed behind the camera, center of the screen will be used as the target instead.
		/// </summary>
		[Tooltip("Position of this Transform will be the center of the transition's circle. When Transform is Null or placed behind the camera, center of the screen will be used as the target instead.")]
		public Transform focus;

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
				// in case the focus is null or behind the camera.
				transitionMaterial.SetFloat("_FocusX", 0.5f);
				transitionMaterial.SetFloat("_FocusY", 0.5f);
				transitionMaterial.SetFloat("_Cutoff", cutoff);
				transitionMaterial.SetFloat("_Falloff", falloff);
				transitionMaterial.SetFloat("_NoiseScale", noiseScale);
				transitionMaterial.SetFloat("_NoiseSpeedX", noiseVelocity.x);
				transitionMaterial.SetFloat("_NoiseSpeedY", noiseVelocity.y);

				switch (backgroundType)
				{
					case BackgroundType.COLOR:
						transitionMaterial.SetColor("_Color", backgroundColor);
						break;
					case BackgroundType.TEXTURE:
						transitionMaterial.SetTexture("_Texture", backgroundTexture);
						break;
				}

				if (focus != null)
				{
					// Check if the focus Transform is behind the camera
					Vector3 dir = (focus.position - transform.position).normalized;
					float dot = Vector3.Dot(transform.forward, dir);

					if (dot > 0)
					{
						Vector2 screenPos = _cam.WorldToViewportPoint(focus.position);
						transitionMaterial.SetFloat("_FocusX", screenPos.x);
						transitionMaterial.SetFloat("_FocusY", screenPos.y);
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
			cutoff = Mathf.Clamp(c, 0f, 2.5f);
		}

		public void SetFalloff(float f)
		{
			falloff = Mathf.Clamp01(f);
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
			focus = f;
		}

		public void RemoveFocus(Transform f)
		{
			if (focus == f)
			{
				focus = null;
			}
			else
			{
				Debug.LogWarning("Provided Transform is not current transition focus. Value will be ignored.");
			}
		}

		public void SetNoiseScale(float s)
		{
			noiseScale = s;
		}

		public void SetNoiseVelocity(Vector2 v)
		{
			noiseVelocity = v;
		}

		#endregion
	}
}
