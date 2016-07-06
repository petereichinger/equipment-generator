using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EquipmentGenerator {

	/// <summary>
	/// Class containing vertices and triangles for sub meshes. Use method <see cref="Combine"/> to combine multiple sub
	/// meshes into a <see cref="Mesh"/>.
	/// </summary>
	public class SubMesh {
		private List<Vector3> _vertices;
		private List<int> _triangles;

		/// <summary>Create a new sub mesh with the specified vertices and triangles.</summary>
		/// <param name="vertices">A list of vertices.</param>
		/// <param name="triangles">A list of triangle indices.</param>
		public SubMesh(List<Vector3> vertices, List<int> triangles) {
			_vertices = vertices;
			_triangles = triangles;
		}

		/// <summary>Modify the vertices of this sub mesh with the function <paramref name="modifyFunc"/>.</summary>
		/// <param name="modifyFunc">Modification function.</param>
		public void Modify(System.Func<Vector3, Vector3> modifyFunc) {
			for (int i = 0; i < _vertices.Count; i++) {
				_vertices[i] = modifyFunc(_vertices[i]);
			}
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
			return mesh;
		}
	}
}
