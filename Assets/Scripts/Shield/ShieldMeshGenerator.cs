using EquipmentGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMeshGenerator {

	public static SubMesh GenerateShield(System.Func<float, float> upperFunction, System.Func<float, float> lowerFunction, IOverlayShape overlayShape,
		int resolution, float depth,
		float leftMargin = -0.5f, float rightMargin = 0.5f) {
		var verts = new List<Vector3>();
		var tris = new List<int>();

		float value = leftMargin;
		float step = (rightMargin - leftMargin) / resolution;
		var oldValues = new Tuple<Vector2?, Vector2?>();
		var newValues = new Tuple<Vector2?, Vector2?>();
		for (int i = 0; i <= resolution; i++) {
			float lowerY = lowerFunction(value);
			float upperY = upperFunction(value);

			if (lowerY > upperY) {
				float temp = lowerY;
				lowerY = upperY;
				upperY = temp;
			}
			newValues.Value1 = new Vector2(value, lowerY);
			if (!Mathf.Approximately(lowerY, upperY)) {
				newValues.Value2 = new Vector2(value, upperY);
			}
			int oldOffset = verts.Count;
			int newOffset = oldOffset + 2 * Tuple.NullableHasValueCount(oldValues);
			if (oldValues.Value1.HasValue) {
				if (oldValues.Value2.HasValue) {
					// Both old values are set.

					if (newValues.Value2.HasValue) {
						// New values has two values as well
						tris.AddTriangle(oldOffset, oldOffset + 1, newOffset);
						tris.AddTriangle(oldOffset + 1, newOffset + 1, newOffset);

						tris.AddTriangle(oldOffset + 2, oldOffset + 3, newOffset + 2, true);
						tris.AddTriangle(oldOffset + 3, newOffset + 3, newOffset + 2, true);
					} else {
						// New values only has one value
						tris.AddTriangle(oldOffset, oldOffset + 1, newOffset);
						tris.AddTriangle(oldOffset + 2, oldOffset + 3, newOffset + 1, true);
					}
				} else {
					if (newValues.Value2.HasValue) {
						// New values has two values, old values just one
						tris.AddTriangle(oldOffset, newOffset + 1, newOffset);
						tris.AddTriangle(oldOffset + 1, newOffset + 3, newOffset + 2, true);
					}
				}
			}

			verts.AddPointTuple(oldValues);
			verts.AddPointTuple(oldValues, depth);
			oldValues.CopyFrom(newValues);
			newValues.Assign();
			value += step;
		}

		verts.AddPointTuple(oldValues);
		verts.AddPointTuple(oldValues, depth);
		overlayShape.Overlay(verts);
		return new SubMesh(verts, tris);
	}
}
