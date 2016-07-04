using System.Collections;
using UnityEditor;
using UnityEngine;

namespace EquipmentGenerator.Shield.Editor {

	[CustomEditor(typeof(ShieldGeneratorComponent))]
	public class ShieldGeneratorComponentEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Generate")) {
				((ShieldGeneratorComponent)target).GenerateShield();
			}
		}
	}
}
