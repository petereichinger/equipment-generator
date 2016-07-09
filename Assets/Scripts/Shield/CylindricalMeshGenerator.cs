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

		/// <summary>Generate a mesh, whose triangles are looking along the cylinder.</summary>
		/// <param name="source">Source for the points of the perimeter.</param>
		/// <param name="scale">
		/// Scaling of the perimeter. <see cref="Vector2.x"/> is the angle of the shield. <see cref="Vector2.y"/> is the
		/// height of the shield.
		/// </param>
		/// <param name="radius">Radius of the cylinder for the mesh.</param>
		/// <param name="offset">
		/// Offset of the shield. Allows creation of only left or right part of a cylindrical mesh.Default is 0f.
		/// </param>
		/// <param name="inside"><c>true</c> if the triangles should face inwards.</param>
		/// <param name="depth">Depth of the perimeter.</param>
		/// <param name="parts">Specify which parts of the perimeter should be created.</param>
		/// <returns>A sub mesh with the generated mesh for the perimeter.</returns>
		public static SubMesh GenerateParallel(IPointSource source, Vector2 scale, float radius = 1f, float offset = 0f, bool inside = false, float depth = 0.1f, Parts parts = Parts.All) {
			List<Vector3> points = new List<Vector3>();
			List<int> triangles = new List<int>();

			Vector2? oldPoint = null;

			for (int i = 0; i <= source.Resolution; i++) {
				Vector2 newPoint = source.GetPoint((float)i / source.Resolution);

				int offsetOld = points.Count;
				int offsetNew = offsetOld + 2;
				bool addTriangles = false;
				if (!oldPoint.HasValue) {
					// No values calculated yet so we are at the left part.
					if ((parts & Parts.Left) != 0) {
						if (source.ZeroBase) {
							points.Add(new Vector3(0, 0));
							points.Add(new Vector3(0, 0, depth));
						}
						points.Add(newPoint);
						points.Add((Vector3)newPoint + Vector3.forward * depth);
						if (!source.ZeroBase) {
							points.Add(new Vector3(0, 1));
							points.Add(new Vector3(0, 1, depth));
						}
						addTriangles = true;
					}
				} else {
					// We already have the left part so we create the middle.
					if ((parts & Parts.Middle) != 0) {
						points.Add(oldPoint.Value);
						points.Add((Vector3)oldPoint.Value + Vector3.forward * depth);

						points.Add(newPoint);
						points.Add((Vector3)newPoint + Vector3.forward * depth);
						addTriangles = true;
					}
				}
				if (addTriangles) {
					triangles.AddTriangle(offsetOld, offsetOld + 1, offsetNew, inside);
					triangles.AddTriangle(offsetOld + 1, offsetNew + 1, offsetNew, inside);
				}
				oldPoint = newPoint;
			}
			if (oldPoint.HasValue) {
				if ((parts & Parts.Right) != 0) {
					int offsetOld = points.Count;
					int offsetNew = offsetOld + 2;
					points.Add(oldPoint.Value);
					points.Add((Vector3)oldPoint.Value + Vector3.forward * depth);
					if (source.ZeroBase) {
						points.Add(new Vector3(1, 0));
						points.Add(new Vector3(1, 0, depth));
					} else {
						points.Add(new Vector3(1, 1));
						points.Add(new Vector3(1, 1, depth));
					}

					triangles.AddTriangle(offsetOld, offsetOld + 1, offsetNew, inside);
					triangles.AddTriangle(offsetOld + 1, offsetNew + 1, offsetNew, inside);
				}
			}

			OverlayOnCylinder(points, scale, radius, offset);

			return new SubMesh(points, triangles);
		}

		/// <summary>Generate an outward or inward (orthogonal to the cylinder) facing <see cref="SubMesh"/>.</summary>
		/// <param name="source">Source for the points.</param>
		/// <param name="scale">
		/// Scaling of the perimeter. <see cref="Vector2.x"/> is the angle of the shield. <see cref="Vector2.y"/> is the
		/// height of the shield.
		/// </param>
		/// <param name="radius">Radius of the cylinder for the mesh.</param>
		/// <param name="offset">
		/// Offset of the shield. Allows creation of only left or right part of a cylindrical mesh.Default is 0f.
		/// </param>
		/// <param name="inside">
		/// <c>true</c> if the triangles should face outward of the cylinder, <c>false</c> otherwise.
		/// </param>
		/// <returns>A <see cref="SubMesh"/> with the mesh.</returns>
		public static SubMesh GenerateOrthogonal(IPointSource source, Vector2 scale, float radius = 1f, float offset = 0f, bool inside = false) {
			List<Vector3> points = new List<Vector3>(source.Resolution * 2 + 1);
			List<int> triangleIndices = new List<int>();

			Tuple<Vector2?, Vector2?> oldValues = new Tuple<Vector2?, Vector2?>(null, null);
			Tuple<Vector2?, Vector2?> newValues = new Tuple<Vector2?, Vector2?>(null, null);

			for (int i = 0; i <= source.Resolution; i++) {
				newValues.Clear();

				var fraction = (float)i / source.Resolution;
				newValues.Value1 = source.GetLowerPoint(fraction);
				newValues.Value2 = source.GetUpperPoint(fraction);

				int offsetOld = points.Count;
				int offsetNew = offsetOld + (oldValues.Value1 != null ? 1 : 0) + (oldValues.Value2 != null ? 1 : 0);

				if (oldValues.Value1 != null) {
					if (newValues.Value1 != null && newValues.Value2 != null) {
						if (oldValues.Value2 != null) {
							triangleIndices.AddTriangle(offsetOld, offsetOld + 1, offsetNew + 1, inside);
						}
						triangleIndices.AddTriangle(offsetOld, offsetNew + 1, offsetNew, inside);
					} else {
						triangleIndices.AddTriangle(offsetOld, offsetOld + 1, offsetNew, inside);
					}
				}

				points.AddPoint(oldValues.Value1);
				points.AddPoint(oldValues.Value2);

				oldValues.Value1 = newValues.Value1;
				oldValues.Value2 = newValues.Value2;
			}

			points.AddPoint(newValues.Value1);
			points.AddPoint(newValues.Value2);

			OverlayOnCylinder(points, scale, radius, offset);

			return new SubMesh(points, triangleIndices);
		}

		/// <summary>
		/// Modifies <paramref name="points"/> so that they are on a y-axis cylinder with the radius <paramref
		/// name="radius"/>.
		/// </summary>
		/// <param name="points">Points to overlay on the cylinder.</param>
		/// <param name="scale">
		/// Scaling. <see cref="Vector2.x"/> is the angle of the mesh. <see cref="Vector2.y"/> is the height of the mesh.
		/// </param>
		/// <param name="radius">Radius of the cylinder in Unity length-units. Must be greater than 0.</param>
		/// <param name="offset">
		/// Normalized offset of the points. Use this to create only left or right part of the cylinder.
		/// </param>
		private static void OverlayOnCylinder(List<Vector3> points, Vector2 scale, float radius, float offset) {
			// Scale and move points
			if (radius <= 0f) {
				Debug.LogError("radius must be > 0");
				return;
			}

			for (int i = 0; i < points.Count; i++) {
				var point = points[i];

				point.x -= offset + 0.5f;

				float realWidth = (radius - point.z) * Mathf.Deg2Rad * scale.x;

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
