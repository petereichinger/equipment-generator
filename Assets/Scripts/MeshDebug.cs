using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class MeshDebug : MonoBehaviour {

	private void Update() {
		var mesh = GetComponent<MeshFilter>().sharedMesh;
		for (int i = 0; i < mesh.vertexCount; i++) {
			var pos = transform.TransformPoint(mesh.vertices[i]);

			Debug.DrawRay(pos, transform.TransformDirection(mesh.normals[i]) * 0.1f, Color.red, 10f);
		}
	}
}
