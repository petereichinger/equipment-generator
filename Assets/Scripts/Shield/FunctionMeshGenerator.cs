using System.Collections.Generic;
using UnityEngine;

public class FunctionMeshGenerator {

	public Mesh Generate(System.Func<float, float> function, int resX, int resY, bool invert = false) {
		var mesh = new Mesh();

		float xStepSize = 1f / resX;
		float yStepSize = 1f / resY;

		List<Vector3> points = new List<Vector3>();

		List<int> triangleIndices = new List<int>();

		List<Vector3> newPoints = new List<Vector3>(resY + 1);
		List<Vector3> oldPoints = new List<Vector3>(resY + 1);
		for (int i = 0; i <= resX; i++) {
			float x = (float)i / resX;
			float y = function(x);

			newPoints.Clear();
			if (invert) {
				for (float yGrid = y; yGrid < 1; yGrid += yStepSize) {
					newPoints.Add(new Vector3(x, yGrid));
				}
				newPoints.Add(new Vector3(x, 1));
			} else {
				for (float yGrid = 0; yGrid < y; yGrid += yStepSize) {
					newPoints.Add(new Vector3(x, yGrid));
				}
				newPoints.Add(new Vector3(x, y));
			}

			if (oldPoints.Count > 0) {
				int offsetOld = points.Count;
				int offsetNew = offsetOld + oldPoints.Count;
				int minCount = Mathf.Min(oldPoints.Count, newPoints.Count) - 1;
				int maxCount = Mathf.Max(oldPoints.Count, newPoints.Count);
				int index = 0;

				while (index < minCount) {
					triangleIndices.Add(offsetOld + index);
					triangleIndices.Add(offsetOld + index + 1);
					triangleIndices.Add(offsetNew + index + 1);

					triangleIndices.Add(offsetOld + index);
					triangleIndices.Add(offsetNew + index + 1);
					triangleIndices.Add(offsetNew + index);

					index++;
				}

				if (oldPoints.Count < newPoints.Count) {
					while (index < newPoints.Count - 1) {
						triangleIndices.Add(offsetNew - 1);
						triangleIndices.Add(offsetNew + index + 1);
						triangleIndices.Add(offsetNew + index);
						index++;
					}
				} else if (newPoints.Count < oldPoints.Count) {
					while (index < oldPoints.Count - 1) {
						triangleIndices.Add(offsetOld + index);
						triangleIndices.Add(offsetOld + index + 1);
						triangleIndices.Add(offsetNew + newPoints.Count - 1);
						index++;
					}
				}
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
