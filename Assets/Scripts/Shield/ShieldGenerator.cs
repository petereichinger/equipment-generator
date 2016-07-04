using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator.Shield {

	public class ShieldGenerator : IGenerator {
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
			Vector3[] vertices = new Vector3[2 * (roundVertices + 1)];
			// Center vertices for front and back
			vertices[0] = new Vector3(0, 0, -thick);
			vertices[roundVertices + 1] = new Vector3(0, 0, thick);
			for (int i = 0; i < roundVertices; i++) {
				float angle = 2 * Mathf.PI * (i / (float)roundVertices);
				vertices[i + 1] = new Vector3(size * Mathf.Cos(angle), size * Mathf.Sin(angle), -thick);
				vertices[i + roundVertices + 2] = new Vector3(size * Mathf.Cos(angle), size * Mathf.Sin(angle), thick);
			}
			int[] triIndices = new int[4 * roundVertices * 3];

			for (int i = 0; i < roundVertices; i++) {
				int idxFront = i * 3;
				int idxBack = (i + roundVertices) * 3;
				int idxOutside = 2 * (i + roundVertices) * 3;
				var front1 = Math.Mod(i + 1, roundVertices) + 1;
				var front2 = i + 1;
				var back1 = roundVertices + i + 2;
				var back2 = roundVertices + Math.Mod(i + 1, roundVertices) + 2;

				// Triangles of front
				triIndices[idxFront] = 0;
				triIndices[idxFront + 1] = front1;
				triIndices[idxFront + 2] = front2;

				// Triangles of back
				triIndices[idxBack] = roundVertices + 1;
				triIndices[idxBack + 1] = back1;
				triIndices[idxBack + 2] = back2;

				// Triangles of ouside
				triIndices[idxOutside] = front2;
				triIndices[idxOutside + 1] = front1;
				triIndices[idxOutside + 2] = back2;

				triIndices[idxOutside + 3] = front2;
				triIndices[idxOutside + 5] = back1;
				triIndices[idxOutside + 4] = back2;
			}

			mesh.vertices = vertices;
			mesh.triangles = triIndices;

			return mesh;
		}
	}
}
