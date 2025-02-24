﻿using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This component implements the replace channels paint mode, which will replace all pixels in the specified textures and channel weights.</summary>
	[HelpURL(P3dHelper.HelpUrlPrefix + "P3dPaintReplaceChannels")]
	[AddComponentMenu(P3dHelper.ComponentMenuPrefix + "Paint/Paint Replace Channels")]
	public class P3dPaintReplaceChannels : MonoBehaviour, IHit, IHitRaycast
	{
		/// <summary>Only the <b>P3dPaintableTexture</b> components with a matching group will be painted by this component.</summary>
		public P3dGroup Group { set { group = value; } get { return group; } } [SerializeField] private P3dGroup group;

		public Texture TextureR { set { textureR = value; } get { return textureR; } } [SerializeField] private Texture textureR;
		public Texture TextureG { set { textureG = value; } get { return textureG; } } [SerializeField] private Texture textureG;
		public Texture TextureB { set { textureB = value; } get { return textureB; } } [SerializeField] private Texture textureB;
		public Texture TextureA { set { textureA = value; } get { return textureA; } } [SerializeField] private Texture textureA;

		public Vector4 ChannelR { set { channelR = value; } get { return channelR; } } [SerializeField] private Vector4 channelR = new Vector4(1, 0, 0, 0);
		public Vector4 ChannelG { set { channelR = value; } get { return channelG; } } [SerializeField] private Vector4 channelG = new Vector4(1, 0, 0, 0);
		public Vector4 ChannelB { set { channelR = value; } get { return channelB; } } [SerializeField] private Vector4 channelB = new Vector4(1, 0, 0, 0);
		public Vector4 ChannelA { set { channelR = value; } get { return channelA; } } [SerializeField] private Vector4 channelA = new Vector4(1, 0, 0, 0);

		public void HandleHitRaycast(bool preview, int priority, float pressure, int seed, RaycastHit hit, Quaternion rotation)
		{
			var model = hit.collider.GetComponentInParent<P3dModel>();

			if (model != null)
			{
				var paintableTextures = P3dPaintableTexture.Filter(model, group);

				if (paintableTextures.Count > 0)
				{
					P3dCommandReplaceChannels.Instance.SetState(preview, priority);
					P3dCommandReplaceChannels.Instance.SetMaterial(textureR, textureG, textureB, textureA, channelR, channelG, channelB, channelA);

					for (var i = paintableTextures.Count - 1; i >= 0; i--)
					{
						var paintableTexture = paintableTextures[i];

						P3dPaintableManager.Submit(P3dCommandReplaceChannels.Instance, model, paintableTexture);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(P3dPaintReplaceChannels))]
	public class P3dPaintReplaceChannels_Editor : P3dEditor<P3dPaintReplaceChannels>
	{
		protected override void OnInspector()
		{
			Draw("group", "Only the P3dPaintableTexture components with a matching group will be painted by this component.");

			Separator();

			Draw("textureR", "");
			Draw("textureG", "");
			Draw("textureB", "");
			Draw("textureA", "");
			Draw("channelR", "");
			Draw("channelG", "");
			Draw("channelB", "");
			Draw("channelA", "");
		}
	}
}
#endif