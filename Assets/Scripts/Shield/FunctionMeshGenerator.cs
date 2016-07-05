using System.Collections.Generic;
using UnityEngine;

public class FunctionMeshGenerator {

	public Mesh Generate(System.Func<float, float> function, int resolution) {
		var mesh = new Mesh();

		List<Vector3> points = new List<Vector3>(resolution * 2 + 1);

		List<int> triangleIndices = new List<int>();

		List<Vector3> newPoints = new List<Vector3>(3);
		List<Vector3> oldPoints = new List<Vector3>(3);
		for (int i = 0; i <= resolution; i++) {
			float x = (float)i / resolution;
			float y = function(x);

			newPoints.Clear();
			newPoints.Add(new Vector3(x, 0));
			if (y > 0f) {
				newPoints.Add(new Vector3(x, y));
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
