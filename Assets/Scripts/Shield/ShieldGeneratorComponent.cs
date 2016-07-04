using System.Collections;
using UnityEngine;

namespace EquipmentGenerator.Shield {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ShieldGeneratorComponent : MonoBehaviour {

		public void GenerateShield() {
			var newShield = new ShieldGenerator().Generate(System.Environment.TickCount);
			var meshFilter = GetComponent<MeshFilter>();
			DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = newShield;
		}
	}
}
