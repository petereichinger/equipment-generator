using System;
using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator {

	public class CylindricalMeshGenerator {

		public static SubMesh Generate(IRangeSource rangeSource, Vector2 scale, float radius = 1f, float offset = 0f, bool flip = false) {
			float sqrRadius = Mathf.Pow(radius, 2);

			List<Vector3> points = new List<Vector3>(rangeSource.Resolution * 2 + 1);
			List<int> triangleIndices = new List<int>();

			List<Vector2> newPoints = new List<Vector2>(3);
			List<Vector2> oldPoints = new List<Vector2>(3);

			for (int i = 0; i <= rangeSource.Resolution; i++) {
				newPoints.Clear();

				rangeSource.GetNextPoints((float)i / rangeSource.Resolution, newPoints);

				int offsetOld = points.Count;
				int offsetNew = offsetOld + oldPoints.Count;

				if (oldPoints.Count > 0) {
					if (newPoints.Count == 2) {
						if (oldPoints.Count == 2) {
							triangleIndices.AddTriangle(offsetOld, offsetOld + 1, offsetNew + 1, flip);
						}
						triangleIndices.AddTriangle(offsetOld, offsetNew + 1, offsetNew, flip);
					} else {
						triangleIndices.AddTriangle(offsetOld, offsetOld + 1, offsetNew, flip);
					}
				}

				points.AddPoints(oldPoints);
				oldPoints.Clear();
				oldPoints.AddRange(newPoints);
			}

			points.AddPoints(newPoints);

			OverlayOnCylinder(points, scale, radius, offset);

			return new SubMesh(points, triangleIndices);
		}

		private static void OverlayOnCylinder(List<Vector3> points, Vector2 scale, float radius, float offset) {
			// Scale and move points

			// float sqrRadius = Mathf.Pow(radius, 2);

			for (int i = 0; i < points.Count; i++) {
				var point = points[i];

				point.x -= offset + 0.5f;

				float realWidth;
				if (radius > 0f) {
					realWidth = (radius - point.z) * Mathf.Deg2Rad * scale.x;
				} else {
					realWidth = scale.x;
				}

				point.Scale(new Vector3(realWidth, scale.y, 1f));

				float finalX = point.x;
				float pow = Mathf.Pow(finalX, 2f);
				float sqrRadius = Mathf.Pow(radius - point.z, 2f);
				if (sqrRadius > pow) {
					float z = -Mathf.Sqrt(sqrRadius - pow);
					point.z = z + radius;
				}
				points[i] = point;
			}
		}
	}
}
