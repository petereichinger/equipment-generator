using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class MeshDebug : MonoBehaviour {
	public float length = 0.1f;

	public bool showVertices = false;

	private void Update() {
		var mesh = GetComponent<MeshFilter>().sharedMesh;
		var origin = transform.TransformPoint(Vector3.zero);
		for (int i = 0; i < mesh.vertexCount; i++) {
			var pos = transform.TransformPoint(mesh.vertices[i]);

			Debug.DrawRay(pos, transform.TransformDirection(mesh.normals[i]) * length, Color.red, 10f);

			if (showVertices) {
				Debug.DrawLine(origin, pos, Color.blue);
			}
		}
	}
}
