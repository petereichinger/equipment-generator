using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator.Shield {

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class ShieldGeneratorComponent : MonoBehaviour {

		[Range(1, 100)]
		public int res = 3;

		public float radius = 3f;

		[Range(-0.5f, 0.5f)]
		public float offset = 0.5f;

		public Vector2 scale = Vector2.one;
		public bool invert = false;

		public AnimationCurve curve;

		public void GenerateShield() {
			// var newShield = new ShieldGenerator().Generate(System.Environment.TickCount);

			var top = new FunctionMeshGenerator().Generate(new FunctionPointSource(x => curve.Evaluate(x), res, invert), scale, radius, offset);
			var middle = new FunctionMeshGenerator().Generate(new SquarePointSource(res), scale, radius, offset);
			var subMeshes = new List<SubMesh>();
			subMeshes.Add(top);
			subMeshes.Add(middle);
			middle.Modify(v => v += Vector3.down * scale.y);
			var mesh = SubMesh.Combine(subMeshes);

			var meshFilter = GetComponent<MeshFilter>();
			DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = mesh;
		}
	}
}
