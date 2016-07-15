using EquipmentGenerator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMeshGenerator {

	/// <summary>Generate a round shield with two polar functions.</summary>
	/// <param name="innerBound">The inner bound polar function for the shield.</param>
	/// <param name="outerBound">The outer bound polar function for the shield.</param>
	/// <param name="numberOfSegments">Number of segements the shield will consist of.</param>
	/// <param name="subdivisions">Number of subdivisions of each shield.</param>
	/// <param name="depth">The depth of the shield.</param>
	/// <returns>A <see cref="SubMesh"/> representing the shield.</returns>
	public static SubMesh GeneratePolar(Func<float, float> innerBound, Func<float, float> outerBound, int numberOfSegments, int subdivisions, float depth) {
		var verts = new List<Vector3>();
		var tris = new List<int>();

		float step = 2 * Mathf.PI / numberOfSegments;
		float value = 0f;

		var oldValues = new Tuple<Vector2?, Vector2?>();
		var newValues = new Tuple<Vector2?, Vector2?>();
		Tuple<Vector2?, Vector2?> startValues = null;
		for (int i = 0; i < numberOfSegments; i++) {
			float inner = innerBound(value);
			float outer = outerBound(value);

			EquipmentGenerator.Math.SwapIfGreater(ref inner, ref outer);

			float innery = inner * Mathf.Sin(value);
			float innerx = inner * Mathf.Cos(value);
			float outery = outer * Mathf.Sin(value);
			float outerx = outer * Mathf.Cos(value);
			newValues.Value1 = new Vector2(innerx, innery);
			if (!(Mathf.Approximately(innerx, outerx) && Mathf.Approximately(innery, outery))) {
				newValues.Value2 = new Vector2(outerx, outery);
			}
			if (startValues == null) {
				startValues = new Tuple<Vector2?, Vector2?>(newValues.Value1, newValues.Value2);
			}

			GenerateShieldSegment(verts, tris, oldValues, newValues, subdivisions, i, numberOfSegments, depth);
			oldValues.CopyFrom(newValues);
			newValues.Clear();
			value += step;
		}
		if (Tuple.BothNotNull(oldValues)) {
			GenerateShieldSegment(verts, tris, oldValues, startValues, subdivisions, numberOfSegments, numberOfSegments, depth);
		}

		return new SubMesh(verts, tris);
	}

	private static void GenerateShieldSegment(List<Vector3> verts, List<int> tris, Tuple<Vector2?, Vector2?> oldValues, Tuple<Vector2?, Vector2?> newValues,
		int subdivisions, int segment, int numberOfSegments, float depth) {
		if (oldValues.Value1.HasValue) {
			float subDivStep = 1f / subdivisions;
			int oldOffset = verts.Count;
			int newOffset = segment < numberOfSegments ? oldOffset + 2 * (subdivisions + 1) : 0;

			Vector2 oldStart = oldValues.Value2.HasValue ? oldValues.Value1.Value : newValues.Value1.Value;
			Vector2 oldEnd = oldValues.Value2.HasValue ? oldValues.Value2.Value : oldValues.Value1.Value;
			bool triangle = !oldValues.Value2.HasValue || !newValues.Value2.HasValue;

			var subDouble = 2 * subdivisions;
			for (int i = 0; i < subDouble; i += 2) {
				float t = i * subDivStep;

				Vector2 oldPos = Vector2.Lerp(oldStart, oldEnd, t);

				if (depth > 0 && i == 0 && !triangle) {
					//Inside

					tris.AddTriangle(oldOffset, oldOffset + 1, newOffset + 1);
					tris.AddTriangle(oldOffset, newOffset + 1, newOffset);
				}

				// Front
				if (!(triangle && i == 0)) {
					tris.AddTriangle(oldOffset + i, newOffset + i, newOffset + i + 2);
				}
				tris.AddTriangle(oldOffset + i, newOffset + i + 2, oldOffset + i + 2);

				// Back
				if (!(triangle && i == 0)) {
					tris.AddTriangle(oldOffset + i + 1, newOffset + i + 1, newOffset + i + 3, true);
				}
				tris.AddTriangle(oldOffset + i + 1, newOffset + i + 3, oldOffset + i + 3, true);

				verts.AddPoint(oldPos);
				verts.AddPoint(oldPos, depth);
			}
			// Outside
			if (depth > 0) {
				tris.AddTriangle(oldOffset + subDouble, newOffset + subDouble, newOffset + subDouble + 1);
				tris.AddTriangle(oldOffset + subDouble, newOffset + subDouble + 1, oldOffset + subDouble + 1);
			}
			verts.AddPoint(oldEnd);
			verts.AddPoint(oldEnd, depth);
		}
	}

	/// <summary>Generate a shield mesh with two carthesian functions.</summary>
	/// <param name="upperBound">Function for the upper bound of the shield.</param>
	/// <param name="lowerBound">Function for the lower bound of the shield.</param>
	/// <param name="resolution">Number of cells on the x-axis.</param>
	/// <param name="depth">Depth of the shield.</param>
	/// <param name="leftBound">
	/// Optional left margin for the shield. This is the first x value that will be evaluated.
	/// </param>
	/// <param name="rightBound">
	/// Optional right margin for the shield. This is the last x value that will be evaluated.
	/// </param>
	/// <returns>A <see cref="SubMesh"/> with the vertices and triangles of the shield.</returns>
	public static SubMesh GenerateCarthesian(System.Func<float, float> upperBound, System.Func<float, float> lowerBound,
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
