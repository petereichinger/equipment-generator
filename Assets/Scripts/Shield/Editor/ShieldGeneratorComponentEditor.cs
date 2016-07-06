using System.Collections;
using UnityEditor;
using UnityEngine;

namespace EquipmentGenerator.Shield.Editor {

	[CustomEditor(typeof(ShieldGeneratorComponent))]
	public class ShieldGeneratorComponentEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();

			if (EditorGUI.EndChangeCheck()) {
				((ShieldGeneratorComponent)target).GenerateShield();
			}
		}
	}
}
