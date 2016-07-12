﻿using EquipmentGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMeshGenerator {

	public static SubMesh GenerateCircular(System.Func<float, float> innerBound, System.Func<float, float> outerBound,
		IOverlayShape shape, int resolution, float depth) {
		var verts = new List<Vector3>();
		var tris = new List<int>();

		float step = 2 * Mathf.PI / resolution;
		float value = 0f;

		var oldValues = new Tuple<Vector2?, Vector2?>();
		var newValues = new Tuple<Vector2?, Vector2?>();
		for (int i = 0; i <= resolution; i++) {
			float inner = innerBound(value);
			float outer = outerBound(value);

			Math.SwapIfGreater(ref inner, ref outer);

			float innery = inner * Mathf.Sin(value);
			float innerx = inner * Mathf.Cos(value);
			float outery = outer * Mathf.Sin(value);
			float outerx = outer * Mathf.Cos(value);
			newValues.Value1 = new Vector2(innerx, innery);
			if (!(Mathf.Approximately(innerx, outerx) && Mathf.Approximately(innery, outery))) {
				newValues.Value2 = new Vector2(outerx, outery);
			}
			int oldOffset = verts.Count;
			int newOffset = oldOffset + Tuple.NullableHasValueCount(oldValues);
			if (Tuple.NullableHasValueCount(oldValues) > 0) {
				if (Tuple.NullableHasValueCount(newValues) == 2) {
					if (oldValues.Value2.HasValue) {
						tris.AddTriangle(newOffset, newOffset + 1, oldOffset);
						tris.AddTriangle(oldOffset, newOffset + 1, oldOffset + 1);
					} else {
						tris.AddTriangle(newOffset, newOffset + 1, oldOffset);
					}
				} else {
					if (Tuple.NullableHasValueCount(oldValues) == 2) {
						tris.AddTriangle(oldOffset, newOffset, oldOffset + 1);
					}
				}
			}
			verts.AddPointTuple(oldValues);
			oldValues.CopyFrom(newValues);

			newValues.Clear();
			value += step;
		}

		verts.AddPointTuple(oldValues);
		shape.Overlay(verts);
		return new SubMesh(verts, tris);
	}

	/// <summary>Generate a shield like mesh.</summary>
	/// <param name="upperBound">Function for the upper bound of the shield.</param>
	/// <param name="lowerBound">Function for the lower bound of the shield.</param>
	/// <param name="overlayShape">A shape to overlay the vertices over.</param>
	/// <param name="resolution">Number of cells on the x-axis.</param>
	/// <param name="depth">Depth of the shield.</param>
	/// <param name="leftBound">
	/// Optional left margin for the shield. This is the first x value that will be evaluated.
	/// </param>
	/// <param name="rightBound">
	/// Optional right margin for the shield. This is the last x value that will be evaluated.
	/// </param>
	/// <returns>A <see cref="SubMesh"/> with the vertices and triangles of the shield.</returns>
	public static SubMesh GenerateLowerUpperBound(System.Func<float, float> upperBound, System.Func<float, float> lowerBound, IOverlayShape overlayShape,
		int resolution, float depth,
		float leftBound = -0.5f, float rightBound = 0.5f) {
		var verts = new List<Vector3>();
		var tris = new List<int>();

		float value = leftBound;
		float step = (rightBound - leftBound) / resolution;
		var oldValues = new Tuple<Vector2?, Vector2?>();
		var newValues = new Tuple<Vector2?, Vector2?>();
		for (int i = 0; i <= resolution; i++) {
			float lowerY = lowerBound(value);
			float upperY = upperBound(value);

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
			GenerateTriangles(oldValues, newValues, tris, oldOffset, newOffset);
			verts.AddPointTuple(oldValues);
			verts.AddPointTuple(oldValues, depth);
			oldValues.CopyFrom(newValues);
			newValues.Clear();
			value += step;
		}
		int offset = verts.Count;
		if (Tuple.NullableHasValueCount(oldValues) == 2) {
			// Only add right side triangles if there are two end values
			tris.AddTriangle(offset, offset + 1, offset + 2);
			tris.AddTriangle(offset + 2, offset + 1, offset + 3);
		}

		verts.AddPointTuple(oldValues);
		verts.AddPointTuple(oldValues, depth);
		overlayShape.Overlay(verts);
		return new SubMesh(verts, tris);
	}

	private static void GenerateTriangles(Tuple<Vector2?, Vector2?> oldValues, Tuple<Vector2?, Vector2?> newValues, List<int> tris, int oldOffset, int newOffset) {
		if (oldValues.Value1.HasValue) {
			if (oldValues.Value2.HasValue) {
				// Both old values are set.

				if (newValues.Value2.HasValue) {
					// New values has two values as well

					// Front
					tris.AddTriangle(oldOffset, oldOffset + 1, newOffset);
					tris.AddTriangle(oldOffset + 1, newOffset + 1, newOffset);

					// Top
					tris.AddTriangle(oldOffset + 1, oldOffset + 3, newOffset + 1);
					tris.AddTriangle(oldOffset + 3, newOffset + 3, newOffset + 1);

					// Back
					tris.AddTriangle(oldOffset + 2, oldOffset + 3, newOffset + 2, true);
					tris.AddTriangle(oldOffset + 3, newOffset + 3, newOffset + 2, true);

					// Bottom
					tris.AddTriangle(oldOffset, oldOffset + 2, newOffset, true);
					tris.AddTriangle(oldOffset + 2, newOffset + 2, newOffset, true);
				} else {
					// New values only has one value

					//Front
					tris.AddTriangle(oldOffset, oldOffset + 1, newOffset);

					// Top
					tris.AddTriangle(oldOffset + 1, oldOffset + 3, newOffset);
					tris.AddTriangle(oldOffset + 3, newOffset + 1, newOffset);

					// Back
					tris.AddTriangle(oldOffset + 2, oldOffset + 3, newOffset + 1, true);

					// Bottom
					tris.AddTriangle(oldOffset, oldOffset + 2, newOffset, true);
					tris.AddTriangle(oldOffset + 2, newOffset + 1, newOffset, true);
				}
			} else {
				if (newValues.Value2.HasValue) {
					// New values has two values, old values just one

					//Front
					tris.AddTriangle(oldOffset, newOffset + 1, newOffset);

					// Top
					tris.AddTriangle(oldOffset, oldOffset + 1, newOffset + 1);
					tris.AddTriangle(oldOffset + 1, newOffset + 3, newOffset + 1);

					// Back
					tris.AddTriangle(oldOffset + 1, newOffset + 3, newOffset + 2, true);

					// Bottom
					tris.AddTriangle(oldOffset, oldOffset + 1, newOffset, true);
					tris.AddTriangle(oldOffset + 1, newOffset + 2, newOffset, true);
				}
			}
		} else {
			if (newValues.Value1.HasValue && newValues.Value2.HasValue) {
				// Only add left side triangles if there are two values at the beginning.
				tris.AddTriangle(oldOffset, oldOffset + 2, oldOffset + 1);
				tris.AddTriangle(oldOffset + 2, oldOffset + 3, oldOffset + 1);
			}
		}
	}
}
