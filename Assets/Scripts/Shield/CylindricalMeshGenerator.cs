using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator {

	public class CylindricalMeshGenerator {

		[System.Flags]
		public enum Parts {
			Left = 0x4,
			Middle = 0x2,
			Right = 0x1,
			LeftRight = Left | Right,
			All = Left | Middle | Right,
		}

		public static SubMesh GenerateOutside(IPointSource source, Vector2 scale, float radius = 1f, float offset = 0f, bool flip = false, float depth = 0.1f, Parts parts = Parts.All) {
			List<Vector3> points = new List<Vector3>();
			List<int> triangles = new List<int>();

			Vector2? oldPoint = null;

			for (int i = 0; i <= source.Resolution; i++) {
				Vector2 newPoint = source.GetPoint((float)i / source.Resolution);

				int offsetOld = points.Count;
				int offsetNew = offsetOld + 2;
				bool addTriangles = false;
				if (oldPoint.HasValue) {
					if ((parts & Parts.Middle) != 0) {
						points.Add(oldPoint.Value);
						points.Add((Vector3)oldPoint.Value + Vector3.forward * depth);

						points.Add(newPoint);
						points.Add((Vector3)newPoint + Vector3.forward * depth);
						addTriangles = true;
					}
				} else {
					if ((parts & Parts.Left) != 0) {
						if (source.ZeroOrigin) {
							points.Add(new Vector3(0, 0));
							points.Add(new Vector3(0, 0, depth));
						}
						points.Add(newPoint);
						points.Add((Vector3)newPoint + Vector3.forward * depth);
						if (!source.ZeroOrigin) {
							points.Add(new Vector3(0, 1));
							points.Add(new Vector3(0, 1, depth));
						}
						addTriangles = true;
					}
				}
				if (addTriangles) {
					triangles.AddTriangle(offsetOld, offsetOld + 1, offsetNew, flip);
					triangles.AddTriangle(offsetOld + 1, offsetNew + 1, offsetNew, flip);
				}
				oldPoint = newPoint;
			}
			if (oldPoint.HasValue) {
				if ((parts & Parts.Right) != 0) {
					int offsetOld = points.Count;
					int offsetNew = offsetOld + 2;
					points.Add(oldPoint.Value);
					points.Add((Vector3)oldPoint.Value + Vector3.forward * depth);
					if (source.ZeroTarget) {
						points.Add(new Vector3(1, 0));
						points.Add(new Vector3(1, 0, depth));
					} else {
						points.Add(new Vector3(1, 1));
						points.Add(new Vector3(1, 1, depth));
					}

					triangles.AddTriangle(offsetOld, offsetOld + 1, offsetNew, flip);
					triangles.AddTriangle(offsetOld + 1, offsetNew + 1, offsetNew, flip);
				}
			}

			OverlayOnCylinder(points, scale, radius, offset);

			return new SubMesh(points, triangles);
		}

		public static SubMesh Generate(IRangeSource rangeSource, Vector2 scale, float radius = 1f, float offset = 0f, bool flip = false) {
			List<Vector3> points = new List<Vector3>(rangeSource.Resolution * 2 + 1);
			List<int> triangleIndices = new List<int>();

			List<Vector2> newPoints = new List<Vector2>(3);
			List<Vector2> oldPoints = new List<Vector2>(3);

			for (int i = 0; i <= rangeSource.Resolution; i++) {
				newPoints.Clear();

				rangeSource.GetPoints((float)i / rangeSource.Resolution, newPoints);

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
