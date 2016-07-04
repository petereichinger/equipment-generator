using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator.Shield {

	public class ShieldGenerator : IGenerator {
		public int roundVertices = 50;

		public Mesh Generate(int seed) {
			Random.seed = seed;
			var mesh = new Mesh { name = seed.ToString() };
			Vector3[] vertices = new Vector3[roundVertices + 1];
			vertices[0] = new Vector3(0, 0, 0);
			for (int i = 0; i < roundVertices; i++) {
				float angle = 2 * Mathf.PI * (i / (float)roundVertices);
				vertices[i + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
			}
			int[] triIndices = new int[roundVertices * 3];

			for (int i = 0; i < roundVertices; i++) {
				int idx = i * 3;
				triIndices[idx] = 0;
				triIndices[idx + 1] = Math.Mod(i + 1, roundVertices) + 1;
				triIndices[idx + 2] = i + 1;
			}

			mesh.vertices = vertices;
			mesh.triangles = triIndices;

			return mesh;
		}
	}
}
