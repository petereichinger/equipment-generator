using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator.Shield {

	public class ShieldGenerator : IGenerator {

		private class ShieldValues {
			public List<Vector3> Vertices { get; set; }
			public List<List<int>> SubMeshes { get; set; }

			public ShieldValues() {
				Vertices = new List<Vector3>();
				SubMeshes = new List<List<int>>();
			}
		}

		public int roundVertices = 16;
		public float minRadius = .75f;
		public float maxRadius = 1.5f;
		public float minThickness = 0.05f;
		public float maxThickness = 0.15f;

		public Mesh Generate(int seed) {
			Random.seed = seed;
			var mesh = new Mesh { name = seed.ToString() };
			// Random radius and thickness
			float size = Random.Range(minRadius, maxRadius);
			float thick = Random.Range(minThickness, maxThickness);

			var values = GenerateFullShield(size, thick, true, false, true);

			mesh.SetVertices(values.Vertices);
			mesh.subMeshCount = values.SubMeshes.Count;
			for (int i = 0; i < values.SubMeshes.Count; i++) {
				mesh.SetTriangles(values.SubMeshes[i], i);
			}
			return mesh;
		}

		private ShieldValues GenerateFullShield(float size, float thick, bool front = true, bool back = true, bool outside = true) {
			var values = new ShieldValues();
			Vector3[] vertices = new Vector3[2 * (roundVertices + 1)];
			// Center vertices for front and back
			vertices[0] = new Vector3(0, 0, -thick);
			vertices[roundVertices + 1] = new Vector3(0, 0, thick);
			for (int i = 0; i < roundVertices; i++) {
				float angle = 2 * Mathf.PI * (i / (float)roundVertices);
				vertices[i + 1] = new Vector3(size * Mathf.Cos(angle), size * Mathf.Sin(angle), -thick);
				vertices[i + roundVertices + 2] = new Vector3(size * Mathf.Cos(angle), size * Mathf.Sin(angle), thick);
			}

			List<int> frontIndices = new List<int>();
			List<int> backIndices = new List<int>();
			List<int> outsideIndices = new List<int>();

			for (int i = 0; i < roundVertices; i++) {
				var front1 = Math.Mod(i + 1, roundVertices) + 1;
				var front2 = i + 1;
				var back1 = roundVertices + i + 2;
				var back2 = roundVertices + Math.Mod(i + 1, roundVertices) + 2;

				if (front) {
					// Triangles of front
					frontIndices.Add(0);
					frontIndices.Add(front1);
					frontIndices.Add(front2);
				}

				if (back) {
					// Triangles of back
					backIndices.Add(roundVertices + 1);
					backIndices.Add(back1);
					backIndices.Add(back2);
				}

				if (outside) {
					// Triangles of ouside
					outsideIndices.Add(front2);
					outsideIndices.Add(front1);
					outsideIndices.Add(back2);

					outsideIndices.Add(front2);
					outsideIndices.Add(back2);
					outsideIndices.Add(back1);
				}
			}
			values.Vertices.AddRange(vertices);

			if (front) {
				values.SubMeshes.Add(frontIndices);
			}
			if (back) {
				values.SubMeshes.Add(backIndices);
			}
			if (outside) {
				values.SubMeshes.Add(outsideIndices);
			}
			return values;
		}
	}
}
