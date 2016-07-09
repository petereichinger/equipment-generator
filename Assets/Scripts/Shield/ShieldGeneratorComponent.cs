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
		public float offset = 0f;

		public Vector2 scale = Vector2.one;

		public AnimationCurve curve;
		public AnimationCurve curveBottom;

		public void GenerateShield() {
			var upperSource = new FunctionSource(x => curve.Evaluate(x), res);
			var middleSource = new SquareSource(res);
			var lowerSource = new FunctionSource(x => curveBottom.Evaluate(x), res, true);
			var outerOverlay = new CylinderOverlayShape(radius, scale, offset);
			var innerOverlay = new CylinderOverlayShape(radius - depth, scale, offset);
			var subMeshes = new List<SubMesh> {
				MeshGenerator.GenerateOrthogonal(upperSource,outerOverlay),
				MeshGenerator.GenerateParallel(upperSource,outerOverlay,false,depth),
				MeshGenerator.GenerateOrthogonal(upperSource, innerOverlay, true).Modify(v=>v + Vector3.forward * depth),
				MeshGenerator.GenerateOrthogonal(middleSource,outerOverlay).Modify(v => v + Vector3.down *scale.y),
				MeshGenerator.GenerateParallel(middleSource,outerOverlay,false,depth,MeshGenerator.Parts.LeftRight).Modify(v => v + Vector3.down *scale.y),
				MeshGenerator.GenerateOrthogonal(middleSource,innerOverlay,true).Modify(v => v +Vector3.down * scale.y+ Vector3.forward * depth),
				MeshGenerator.GenerateOrthogonal(lowerSource, outerOverlay).Modify(v => v + Vector3.down * scale.y * 2f),
				MeshGenerator.GenerateParallel(lowerSource, outerOverlay,true,depth).Modify(v => v + Vector3.down * scale.y * 2f),
				MeshGenerator.GenerateOrthogonal(lowerSource, innerOverlay,true).Modify(v => v + Vector3.down * scale.y * 2f+ Vector3.forward * depth)
			};
			var mesh = SubMesh.Combine(subMeshes);

			var meshFilter = GetComponent<MeshFilter>();
			DestroyImmediate(meshFilter.sharedMesh);
			meshFilter.sharedMesh = mesh;
		}
	}
}
