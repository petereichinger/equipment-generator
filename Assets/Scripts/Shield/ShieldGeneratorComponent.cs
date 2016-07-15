using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace EquipmentGenerator.Shield {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ShieldGeneratorComponent : MonoBehaviour {

		[Range(1, 100)]
		public int res = 3;

		public float radius = 3f;

		public float depth = 0.1f;

		[Range(-0.5f, 0.5f)]
		public float offset = 0f;

		public Vector2 scale = Vector2.one;

		public AnimationCurve curve;
		public AnimationCurve curveBottom;

		public void GenerateShield() {
			var overlay = new PyramidOverlayShape(1f, scale, offset);
			// var overlay = new SphereOverlayShape(radius, scale.x, offset);
			var subMeshes = new List<SubMesh> {
// ShieldMeshGenerator.GenerateCarthesian(x=>Mathf.Sqrt(.25f - x*x),x=>-Mathf.Sqrt(.25f - x*x),overlay,res,depth)
ShieldMeshGenerator.GeneratePolar(x => .1f,x=> 0f,overlay,res,5, depth)
			};
			var mesh = SubMesh.Combine(subMeshes);

			var meshFilter = GetComponent<MeshFilter>();
			DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = mesh;
		}
	}
}
