using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator {

	/// <summary>Interface for a shape to overlay vertices on</summary>
	public interface IOverlayShape {

		SubMesh Overlay(SubMesh mesh);
	}

	public class PyramidOverlayShape : IOverlayShape {
		private readonly float _gradient;
		private readonly Vector2 _scale;
		private readonly float _offset;

		/// <summary>Create a new overlay shape.</summary>
		/// <param name="gradient">Gradient of the pyramid.</param>
		/// <param name="scale">Scaling.</param>
		/// <param name="offset">Normalized offset for the overlay shape.</param>
		public PyramidOverlayShape(float gradient, Vector2 scale, float offset = 0f) {
			_gradient = gradient;
			_scale = scale;
			_offset = offset;
		}

		public SubMesh Overlay(SubMesh mesh) {
			for (int i = 0; i < mesh.Vertices.Count; i++) {
				var point = mesh.Vertices[i];
				point.x -= _offset;

				point.Scale(new Vector3(_scale.x, _scale.y, 1f));
				float offsetX = Mathf.Abs(_gradient * point.x);
				float offsetY = Mathf.Abs(_gradient * point.y);
				point.z += Mathf.Max(offsetX, offsetY);
				mesh.Vertices[i] = point;
			}
			return mesh;
		}
	}

	/// <summary>Spherical overlay shape.</summary>
	public class SphereOverlayShape : IOverlayShape {
		private readonly float _scale;
		private readonly float _radius;
		private readonly float _offset;

		/// <summary>Create a new instance of a <see cref="SphereOverlayShape"/>.</summary>
		/// <param name="radius">Radius of the sphere.</param>
		/// <param name="scale"></param>
		/// <param name="offset">Normalized offset for the overlay shape.</param>
		public SphereOverlayShape(float radius, float scale, float offset = 0f) {
			_scale = scale;
			_radius = radius;
			_offset = offset;
		}

		public SubMesh Overlay(SubMesh mesh) {
			// Scale and move points
			if (_radius <= 0f) {
				Debug.LogError("radius must be > 0");
				return mesh;
			}

			for (int i = 0; i < mesh.Vertices.Count; i++) {
				var point = mesh.Vertices[i];

				point.x -= _offset;

				float realWidth = (_radius - point.z) * Mathf.Deg2Rad * _scale;

				point.Scale(new Vector3(realWidth, realWidth, 1f));

				float xPow = Mathf.Pow(point.x, 2f);
				float yPow = Mathf.Pow(point.y, 2f);
				float sqrRadius = Mathf.Pow(_radius - point.z, 2f);
				if (sqrRadius > xPow) {
					float z = -Mathf.Sqrt(sqrRadius - xPow - yPow);
					point.z = z + _radius;
				}
				mesh.Vertices[i] = point;
			}
			return mesh;
		}
	}

	/// <summary>Flat overlay shape that just scales and has a normalized offset.</summary>
	public class FlatOverlayShape : IOverlayShape {
		private readonly Vector2 _scale;
		private readonly float _offset;

		/// <summary>Create a new FlatOverlayShape.</summary>
		/// <param name="scale">Scaling.</param>
		/// <param name="offset">Optional normalized offset.</param>
		public FlatOverlayShape(Vector2 scale, float offset = 0f) {
			_scale = scale;
			_offset = offset;
		}

		public SubMesh Overlay(SubMesh mesh) {
			for (int i = 0; i < mesh.Vertices.Count; i++) {
				var point = mesh.Vertices[i];
				point.x -= _offset;
				point.Scale(new Vector3(_scale.x, _scale.y, 1f));

				mesh.Vertices[i] = point;
			}
			return mesh;
		}
	}

	/// <summary>Overlay shape that angles the mesh with a corner in the center.</summary>
	public class AngledOverlayShape : IOverlayShape {
		private readonly float _gradient;
		private readonly Vector2 _scale;
		private readonly float _offset;

		/// <summary>Create a new overlay shape.</summary>
		/// <param name="gradient">Gradient of the angle.</param>
		/// <param name="scale">Scaling.</param>
		/// <param name="offset">Normalized offset for the overlay shape.</param>
		public AngledOverlayShape(float gradient, Vector2 scale, float offset = 0f) {
			_gradient = gradient;
			_scale = scale;
			_offset = offset;
		}

		public SubMesh Overlay(SubMesh mesh) {
			for (int i = 0; i < mesh.Vertices.Count; i++) {
				var point = mesh.Vertices[i];
				point.x -= _offset;

				point.Scale(new Vector3(_scale.x, _scale.y, 1f));

				point.z += Mathf.Abs(_gradient * point.x);
				mesh.Vertices[i] = point;
			}
			return mesh;
		}
	}

	/// <summary>Overlay shape for a cylinder.</summary>
	public class CylinderOverlayShape : IOverlayShape {
		private readonly Vector2 _scale;
		private readonly float _radius;
		private readonly float _offset;

		/// <summary>Create a new instance of a Cylinder OverlayShape</summary>
		/// <param name="radius">Radius of the cylinder.</param>
		/// <param name="scale">Scale of the overlay shape. <see cref="Vector2.x"/> is the angle of the</param>
		/// <param name="offset">Normalized offset for the overlay shape.</param>
		public CylinderOverlayShape(float radius, Vector2 scale, float offset = 0f) {
			_scale = scale;
			_radius = radius;
			_offset = offset;
		}

		public SubMesh Overlay(SubMesh mesh) {
			// Scale and move points
			if (_radius <= 0f) {
				Debug.LogError("radius must be > 0");
				return mesh;
			}

			for (int i = 0; i < mesh.Vertices.Count; i++) {
				var point = mesh.Vertices[i];

				point.x -= _offset;

				float realWidth = (_radius - point.z) * Mathf.Deg2Rad * _scale.x;

				point.Scale(new Vector3(realWidth, _scale.y, 1f));

				float finalX = point.x;
				float pow = Mathf.Pow(finalX, 2f);
				float sqrRadius = Mathf.Pow(_radius - point.z, 2f);
				if (sqrRadius > pow) {
					float z = -Mathf.Sqrt(sqrRadius - pow);
					point.z = z + _radius;
				}
				mesh.Vertices[i] = point;
			}
			return mesh;
		}
	}
}
