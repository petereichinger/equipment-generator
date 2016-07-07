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

		public float depth = 0.1f;

		[Range(-0.5f, 0.5f)]
		public float offset = 0.5f;

		public Vector2 scale = Vector2.one;

		public AnimationCurve curve;
		public AnimationCurve curveBottom;

		public void GenerateShield() {
			var subMeshes = new List<SubMesh> {
				CylindricalMeshGenerator.Generate(new FunctionSource(x => curve.Evaluate(x), res), scale, radius, offset),
				CylindricalMeshGenerator.Generate(new FunctionSource(x => curve.Evaluate(x), res), scale, radius-depth, offset, true).Modify(v=>v + Vector3.forward * depth),
				CylindricalMeshGenerator.Generate(new SquareSource(res), scale, radius, offset).Modify(v => v + Vector3.down * scale.y),
				CylindricalMeshGenerator.Generate(new SquareSource(res), scale, radius-depth, offset,true).Modify(v => v + Vector3.down * scale.y+ Vector3.forward * depth),
				CylindricalMeshGenerator.Generate(new FunctionSource(x => curveBottom.Evaluate(x), res, true), scale,
					radius, offset).Modify(v => v + Vector3.down * scale.y * 2f),
				CylindricalMeshGenerator.Generate(new FunctionSource(x => curveBottom.Evaluate(x), res, true), scale,
					radius-depth, offset,true).Modify(v => v + Vector3.down * scale.y * 2f+ Vector3.forward * depth)
			};
			var mesh = SubMesh.Combine(subMeshes);

			var meshFilter = GetComponent<MeshFilter>();
			DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = mesh;
		}
	}
}
