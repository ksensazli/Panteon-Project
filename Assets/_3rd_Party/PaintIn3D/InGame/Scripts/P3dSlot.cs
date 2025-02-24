﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PaintIn3D
{
	/// <summary>This struct stores a reference to a texture on a GameObject.</summary>
	[System.Serializable]
	public struct P3dSlot
	{
		/// <summary>The material index in the attached renderer.</summary>
		public int Index;

		/// <summary>The name of the texture in the specified material.</summary>
		public string Name;

		public P3dSlot(int newIndex, string newName)
		{
			Index = newIndex;
			Name  = newName;
		}

		public Material FindMaterial(GameObject gameObject)
		{
			return P3dHelper.GetMaterial(gameObject, Index);
		}

		public Texture FindTexture(GameObject gameObject)
		{
			var material = P3dHelper.GetMaterial(gameObject, Index);

			if (material != null)
			{
				return material.GetTexture(Name);
			}

			return null;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Index.GetHashCode() ^ Name.GetHashCode();
		}

		public static bool operator == (P3dSlot a, P3dSlot b)
		{
			return a.Index == b.Index && a.Name == b.Name;
		}

		public static bool operator != (P3dSlot a, P3dSlot b)
		{
			return a.Index != b.Index || a.Name != b.Name;
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	[CustomPropertyDrawer(typeof(P3dSlot))]
	public class P3dSlot_Drawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var sObj      = property.serializedObject;
			var sIdx      = property.FindPropertyRelative("Index");
			var sNam      = property.FindPropertyRelative("Name");
			var rectA     = position; rectA.width = EditorGUIUtility.labelWidth;
			var rectB     = position; rectB.xMin += EditorGUIUtility.labelWidth; rectB.width = 20;
			var rectC     = position; rectC.xMin += EditorGUIUtility.labelWidth; rectC.xMin += 22; rectC.width -= 20;
			var rectD     = position; rectD.xMin = rectD.xMax - 18;
			var component = property.serializedObject.targetObject as Component;
			var exists    = false;

			// Invalid slot?
			if (component != null)
			{
				var material = P3dHelper.GetMaterial(component.gameObject, sIdx.intValue);

				if (material != null && P3dHelper.TexEnvNameExists(material.shader, sNam.stringValue) == false)
				{
					exists = true;
				}
			}

			P3dHelper.BeginColor(exists);
			{
				EditorGUI.LabelField(rectA, label);

				sIdx.intValue    = Mathf.Clamp(EditorGUI.IntField(rectB, sIdx.intValue), 0, 99);
				sNam.stringValue = EditorGUI.TextField(rectC, sNam.stringValue);

				sObj.ApplyModifiedProperties();

				// Draw menu
				if (GUI.Button(rectD, "", EditorStyles.popup) == true)
				{
					if (component != null)
					{
						var menu     = new GenericMenu();
						var renderer = component.GetComponent<Renderer>();

						if (renderer != null)
						{
							var materials = renderer.sharedMaterials;

							if (materials.Length > 0)
							{
								for (var i = 0; i < materials.Length; i++)
								{
									var material     = materials[i];
									var materialName = i.ToString();
									var matIndex     = i;

									if (material != null)
									{
										materialName += " (" + material.name + ")";

										var texEnvs = P3dHelper.GetTexEnvs(material.shader);

										if (texEnvs != null && texEnvs.Count > 0)
										{
											for (var j = 0; j < texEnvs.Count; j++)
											{
												var texName  = texEnvs[j].Name;
												var texTitle = texName;
												var tex      = material.GetTexture(texName);

												if (tex != null)
												{
													texTitle += " (" + tex.name + ")";
												}
												else
												{
													texTitle += " (empty)";
												}

												menu.AddItem(new GUIContent(materialName + "/" + texTitle), sIdx.intValue == matIndex && sNam.stringValue == texName, () => { sIdx.intValue = matIndex; sNam.stringValue = texName; sObj.ApplyModifiedProperties(); });
											}
										}
										else
										{
											menu.AddDisabledItem(new GUIContent(materialName + "/This Material's shader has no textures!"));
										}
									}
									else
									{
										menu.AddDisabledItem(new GUIContent(materialName + "/This Material is null!"));
									}
								}
							}
							else
							{
								menu.AddDisabledItem(new GUIContent("This GameObject has no materials!"));
							}
						}
						else
						{
							menu.AddDisabledItem(new GUIContent("This GameObject has no renderer!"));
						}

						menu.DropDown(rectD);
					}
				}
			}
			P3dHelper.EndColor();
		}
	}
}
#endif