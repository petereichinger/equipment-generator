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
		// public bool invert = false;

		public AnimationCurve curve;
		public AnimationCurve curveBottom;

		public void GenerateShield() {
			// var newShield = new ShieldGenerator().Generate(System.Environment.TickCount);

			var top = CylindricalMeshGenerator.Generate(new FunctionPointSource(x => curve.Evaluate(x), res), scale, radius, offset);
			var middle = CylindricalMeshGenerator.Generate(new SquarePointSource(res), scale, radius, offset);
			var bottom = CylindricalMeshGenerator.Generate(new FunctionPointSource(x => curveBottom.Evaluate(x), res, true), scale,
				radius, offset);
			var subMeshes = new List<SubMesh>();
			subMeshes.Add(top);
			subMeshes.Add(middle);
			subMeshes.Add(bottom);
			middle.Modify(v => v += Vector3.down * scale.y);
			bottom.Modify(v => v += Vector3.down * scale.y * 2f);
			var mesh = SubMesh.Combine(subMeshes);

			var meshFilter = GetComponent<MeshFilter>();
			DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = mesh;
		}
	}
}
