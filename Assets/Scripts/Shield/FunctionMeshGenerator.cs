using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class FunctionMeshGenerator {

	public Mesh Generate(System.Func<float, Vector3> function, int resolution, bool invert = false) {
		var mesh = new Mesh();

		List<Vector3> points = new List<Vector3>(resolution * 2 + 1);

		List<int> triangleIndices = new List<int>();

		List<Vector3> newPoints = new List<Vector3>(3);
		List<Vector3> oldPoints = new List<Vector3>(3);

		float minY = function(0f).y;

		for (int i = 0; i < resolution; i++) {
			float val = function((float)i / resolution).y;
			if (val < minY) {
				minY = val;
			}
		}

		for (int i = 0; i <= resolution; i++) {
			float x = (float)i / resolution;
			Vector3 value = function(x);

			newPoints.Clear();

			if (!invert) {
				newPoints.Add(new Vector3(x, minY, normalizedZ));
				if (value.y > minY) {
					newPoints.Add(value);
				}
			} else {
				if (value.y < maxY) {
					newPoints.Add(value);
				}
				newPoints.Add(new Vector3(x, maxY, normalizedZ));
			}

			int offsetOld = points.Count;
			int offsetNew = offsetOld + oldPoints.Count;

			if (oldPoints.Count > 0) {
				if (oldPoints.Count == 2) {
					triangleIndices.Add(offsetOld);
					triangleIndices.Add(offsetOld + 1);
					triangleIndices.Add(offsetNew + 1);
				}
				triangleIndices.Add(offsetOld);
				triangleIndices.Add(offsetNew + 1);
				triangleIndices.Add(offsetNew);
			}

			points.AddRange(oldPoints);
			oldPoints.Clear();
			oldPoints.AddRange(newPoints);
		}

		points.AddRange(newPoints);

		mesh.SetVertices(points);
		mesh.subMeshCount = 1;
		mesh.SetTriangles(triangleIndices, 0);

		mesh.Optimize();

		return mesh;
	}
}
