using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EquipmentGenerator {

	public static class MeshHelpers {

		/// <summary>Add 2D points to a list of vertices.</summary>
		/// <param name="vertices">List of vertices.</param>
		/// <param name="points">List of points.</param>
		public static void AddPoints(this List<Vector3> vertices, List<Vector2> points) {
			for (int i = 0; i < points.Count; i++) {
				vertices.Add(points[i]);
			}
		}

		/// <summary>Add a triangle to the list of triangles.</summary>
		/// <param name="triangles">List of triangles.</param>
		/// <param name="idx1">Index of first triangle.</param>
		/// <param name="idx2">Index of second triangle.</param>
		/// <param name="idx3">Index of third triangle.</param>
		/// <param name="insideOut">
		/// <c>true</c>, if the triangle should be added flipped, <c>false</c> otherwise. Default is <c>false</c>.
		/// </param>
		/// <returns>A reference to the list. This is useful to chain multiple additions.</returns>
		public static List<int> AddTriangle(this List<int> triangles, int idx1, int idx2, int idx3, bool insideOut = false) {
			if (insideOut) {
				triangles.Add(idx1);
				triangles.Add(idx3);
				triangles.Add(idx2);
			} else {
				triangles.Add(idx1);
				triangles.Add(idx2);
				triangles.Add(idx3);
			}

			return triangles;
		}
	}

	/// <summary>
	/// Class containing vertices and triangles for sub meshes. Use method <see cref="Combine"/> to combine multiple sub
	/// meshes into a <see cref="Mesh"/>.
	/// </summary>
	public class SubMesh {

		/// <summary>The vertices of this submesh.</summary>
		private readonly List<Vector3> _vertices;

		/// <summary>The triangle indices of this submesh.</summary>
		private readonly List<int> _triangles;

		/// <summary>Create a new sub mesh with the specified vertices and triangles.</summary>
		/// <param name="vertices">A list of vertices.</param>
		/// <param name="triangles">A list of triangle indices.</param>
		public SubMesh(List<Vector3> vertices, List<int> triangles) {
			_vertices = vertices;
			_triangles = triangles;
		}

		/// <summary>Modify the vertices of this sub mesh with the function <paramref name="modifyFunc"/>.</summary>
		/// <param name="modifyFunc">Modification function.</param>
		public SubMesh Modify(System.Func<Vector3, Vector3> modifyFunc) {
			for (int i = 0; i < _vertices.Count; i++) {
				_vertices[i] = modifyFunc(_vertices[i]);
			}
			return this;
		}

		/// <summary>Combine multiple sub meshes into a Unity <see cref="Mesh"/>.</summary>
		/// <param name="subMeshes">List of <see cref="SubMesh"/>.</param>
		/// <param name="singleSubMesh">
		/// <c>true</c> if the resulting mesh should only have one sub mesh, <c>false</c> if every <see cref="SubMesh"/>
		/// should be made into a separate unity sub mesh.
		/// </param>
		/// <returns>A <see cref="Mesh"/> with all submeshes.</returns>
		public static Mesh Combine(List<SubMesh> subMeshes, bool singleSubMesh = true) {
			var mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();
			List<List<int>> subMeshTriangles = new List<List<int>>();
			int numberOfVertices = 0;
			for (int i = 0; i < subMeshes.Count; i++) {
				vertices.AddRange(subMeshes[i]._vertices);
				List<int> addList;
				if (singleSubMesh) {
					addList = triangles;
				} else {
					addList = new List<int>();
				}
				addList.AddRange(subMeshes[i]._triangles.Select(index => index + numberOfVertices));
				if (!singleSubMesh) {
					subMeshTriangles.Add(addList);
				}
				numberOfVertices += subMeshes[i]._vertices.Count;
			}
			mesh.SetVertices(vertices);

			if (singleSubMesh) {
				mesh.subMeshCount = 1;
				mesh.SetTriangles(triangles, 0);
			} else {
				mesh.subMeshCount = subMeshTriangles.Count;

				for (int i = 0; i < subMeshTriangles.Count; i++) {
					mesh.SetTriangles(subMeshTriangles[i], i);
				}
			}

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}
	}
}
