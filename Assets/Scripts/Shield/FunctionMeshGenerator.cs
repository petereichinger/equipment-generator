using System.Collections.Generic;
using UnityEngine;

public class FunctionMeshGenerator {

	public interface IPointSource {
		int Resolution { get; }

		void GetNextPoints(float fraction, List<Vector3> nextPointsList);
	}

	public class FunctionPointSource : IPointSource {
		public int Resolution { get; private set; }

		private System.Func<float, float> _function;
		private float _minY;
		private float _maxY;
		private bool _inverted;

		public void GetNextPoints(float fraction, List<Vector3> nextPointsList) {
			float x = fraction;
			float value = _function(x);

			if (!_inverted) {
				nextPointsList.Add(new Vector3(x, _minY, 0));
				if (value > _minY) {
					nextPointsList.Add(new Vector3(x, value, 0));
				}
			} else {
				if (value < _maxY) {
					nextPointsList.Add(new Vector3(x, value, 0));
				}
				nextPointsList.Add(new Vector3(x, _maxY, 0));
			}
		}

		public FunctionPointSource(System.Func<float, float> function, int resolution, bool inverted = false) {
			Resolution = resolution;
			_function = function;
			_inverted = inverted;
			_minY = _function(0f);
			_maxY = _function(0f);

			for (int i = 0; i < resolution; i++) {
				float val = _function((float)i / resolution);
				if (val < _minY) {
					_minY = val;
				}
				if (val > _maxY) {
					_maxY = val;
				}
			}
		}
	}

	public Mesh Generate(IPointSource pointSource, Vector2 scale, float radius = 0f, float offset = 0f) {
		var mesh = new Mesh();

		float sqrRadius = Mathf.Pow(radius, 2);

		List<Vector3> points = new List<Vector3>(pointSource.Resolution * 2 + 1);

		List<int> triangleIndices = new List<int>();

		List<Vector3> newPoints = new List<Vector3>(3);
		List<Vector3> oldPoints = new List<Vector3>(3);

		for (int i = 0; i <= pointSource.Resolution; i++) {
			newPoints.Clear();

			pointSource.GetNextPoints((float)i / pointSource.Resolution, newPoints);

			int offsetOld = points.Count;
			int offsetNew = offsetOld + oldPoints.Count;

			if (oldPoints.Count > 0) {
				if (newPoints.Count == 2) {
					if (oldPoints.Count == 2) {
						triangleIndices.Add(offsetOld);
						triangleIndices.Add(offsetOld + 1);
						triangleIndices.Add(offsetNew + 1);
					}
					triangleIndices.Add(offsetOld);
					triangleIndices.Add(offsetNew + 1);
					triangleIndices.Add(offsetNew);
				} else {
					triangleIndices.Add(offsetOld);
					triangleIndices.Add(offsetOld + 1);
					triangleIndices.Add(offsetNew);
				}
			}

			points.AddRange(oldPoints);
			oldPoints.Clear();
			oldPoints.AddRange(newPoints);
		}

		points.AddRange(newPoints);

		// Scale and move points
		float realWidth;
		if (radius > 0f) {
			realWidth = radius * Mathf.Deg2Rad * scale.x;
		} else {
			realWidth = scale.x;
		}
		Vector3 finalScale = new Vector3(realWidth, scale.y);

		for (int i = 0; i < points.Count; i++) {
			var point = points[i];

			point.x -= offset + 0.5f;

			point.Scale(finalScale);

			float finalX = point.x;
			float pow = Mathf.Pow(finalX, 2f);
			if (radius > pow) {
				float z = -Mathf.Sqrt(sqrRadius - pow);
				point.z = z + radius;
			}
			points[i] = point;
		}

		mesh.SetVertices(points);
		mesh.subMeshCount = 1;
		mesh.SetTriangles(triangleIndices, 0);

		mesh.Optimize();

		return mesh;
	}
}
