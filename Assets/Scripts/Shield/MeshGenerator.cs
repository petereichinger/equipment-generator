using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator {

	public class MeshGenerator {

		[System.Flags]
		public enum Parts {
			Left = 0x4,
			Middle = 0x2,
			Right = 0x1,
			LeftRight = Left | Right,
			All = Left | Middle | Right,
		}

		/// <summary>Generate a mesh, whose triangles are looking along the overlay shape.</summary>
		/// <param name="source">Source for the points of the perimeter.</param>
		/// <param name="overlayShape">Shape to overlay.</param>
		/// <param name="inside"><c>true</c> if the triangles should face inwards.</param>
		/// <param name="depth">Depth of the perimeter.</param>
		/// <param name="parts">Specify which parts of the perimeter should be created.</param>
		/// <returns>A sub mesh with the generated mesh for the perimeter.</returns>
		public static SubMesh GenerateParallel(IPointSource source, IOverlayShape overlayShape, bool inside = false, float depth = 0.1f, Parts parts = Parts.All) {
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

			overlayShape.Overlay(points);

			return new SubMesh(points, triangles);
		}

		/// <summary>
		/// Generate an outward or inward (orthogonal to the overlayshape) facing <see cref="SubMesh"/>.
		/// </summary>
		/// <param name="source">Source for the points.</param>
		/// <param name="overlayShape">Shape to overlay</param>
		/// <param name="inside">
		/// <c>true</c> if the triangles should face outward of the cylinder, <c>false</c> otherwise.
		/// </param>
		/// <returns>A <see cref="SubMesh"/> with the mesh.</returns>
		public static SubMesh GenerateOrthogonal(IPointSource source, IOverlayShape overlayShape, bool inside = false) {
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

			overlayShape.Overlay(points);

			return new SubMesh(points, triangleIndices);
		}
	}
}
