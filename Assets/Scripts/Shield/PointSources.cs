using System;
using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator {

	/// <summary>
	/// Interface for a point source that can be used in <see cref="CylindricalMeshGenerator.Generate"/>. Range sources
	/// return a range with the minimum and maximum always in the range [0:1].
	/// </summary>
	public interface IRangeSource {

		/// <summary>
		/// Resolution of this point source. This is the number of cells that will be created in the horizontal.
		/// </summary>
		int Resolution { get; }

		/// <summary>Get the points for a certain fraction.</summary>
		/// <param name="fraction">Fraction.</param>
		/// <param name="nextPointsList">
		/// List where the next points should be stored. This list should be cleared by the caller before calling this
		/// method.
		/// </param>
		void GetPoints(float fraction, List<Vector2> nextPointsList);
	}

	/// <summary>Interface for a source that only returns a single value.</summary>
	public interface IPointSource {

		/// <summary>
		/// Resolution of this point source. This is the number of cells that will be created in the horizontal.
		/// </summary>
		int Resolution { get; }

		bool ZeroOrigin { get; }

		bool ZeroTarget { get; }

		/// <summary>Get the points for a certain fraction.</summary>
		/// <param name="fraction">Fraction.</param>
		/// <returns>The value for the specified fraction.</returns>
		Vector2 GetPoint(float fraction);
	}

	/// <summary>
	/// Point source that uses a <see cref="System.Func{T,TResult}"/> with ( <see cref="float"/> as generic parameters).
	/// </summary>
	public class FunctionSource : IRangeSource, IPointSource {
		public int Resolution { get; private set; }

		public bool ZeroOrigin { get { return !_inverted; } }

		public bool ZeroTarget { get { return !_inverted; } }

		/// <summary>Function that is evaluated.</summary>
		private readonly Func<float, float> _function;

		/// <summary>Minimum Y value.</summary>
		private readonly float _minY;

		/// <summary>Maximum Y value.</summary>
		private readonly float _maxY;

		/// <summary>
		/// Flag that indicates wether points from minimum y to the function value should be created or from the function
		/// value to the maximum y value.
		/// </summary>
		private readonly bool _inverted;

		/// <summary>Get the next points with the function.</summary>
		/// <param name="fraction">Fraction.</param>
		/// <param name="nextPointsList">List where the new points are stored.</param>
		public void GetPoints(float fraction, List<Vector2> nextPointsList) {
			float x = fraction;

			float value = _function(x);

			if (!_inverted) {
				nextPointsList.Add(new Vector2(x, _minY));
				if (value > _minY) {
					nextPointsList.Add(new Vector2(x, value));
				}
			} else {
				if (value < _maxY) {
					nextPointsList.Add(new Vector2(x, value));
				}
				nextPointsList.Add(new Vector2(x, _maxY));
			}
		}

		public Vector2 GetPoint(float fraction) {
			return new Vector2(fraction, _function(fraction));
		}

		/// <summary>Create a new instance of this point source.</summary>
		/// <param name="function">Function to use.</param>
		/// <param name="resolution">Resolution.</param>
		/// <param name="inverted">True if should be inverted or not.</param>
		public FunctionSource(Func<float, float> function, int resolution, bool inverted = false) {
			Resolution = resolution;
			_function = function;
			_inverted = inverted;
			_minY = 0f;
			_maxY = 1f;

			for (int i = 0; i < resolution; i++) {
				float val = _function((float)i / resolution);
				if (val < _minY) {
					_minY = val;
				}
				if (val > _maxY) {
					_maxY = val;
				}
			}
		}
	}

	public class SquareSource : IRangeSource, IPointSource {
		public int Resolution { get; private set; }

		public bool ZeroOrigin { get { return true; } }

		public bool ZeroTarget { get { return true; } }

		public Vector2 GetPoint(float fraction) {
			return new Vector2(fraction, 1f);
		}

		public void GetPoints(float fraction, List<Vector2> nextPointsList) {
			nextPointsList.Add(new Vector2(fraction, 0));
			nextPointsList.Add(new Vector2(fraction, 1));
		}

		public SquareSource(int resolution) {
			Resolution = resolution;
		}
	}
}
