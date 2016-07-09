using System;
using System.Collections.Generic;
using UnityEngine;

namespace EquipmentGenerator {

	/// <summary>Interface for a source that only returns a single value.</summary>
	public interface IPointSource {

		/// <summary>
		/// Resolution of this point source. This is the number of cells that will be created in the horizontal.
		/// </summary>
		int Resolution { get; }

		/// <summary>Indicates wether the point source should return a range from zero beginning or to one.</summary>
		bool ZeroBase { get; }

		/// <summary>Get the points for a certain fraction.</summary>
		/// <param name="fraction">Fraction.</param>
		/// <returns>The value for the specified fraction.</returns>
		Vector2 GetPoint(float fraction);

		Vector2? GetLowerPoint(float fraction);

		Vector2? GetUpperPoint(float fraction);
	}

	/// <summary>
	/// Point source that uses a <see cref="System.Func{T,TResult}"/> with ( <see cref="float"/> as generic parameters).
	/// </summary>
	public class FunctionSource : IPointSource {
		public int Resolution { get; private set; }

		public bool ZeroBase { get { return !_inverted; } }

		/// <summary>Function that is evaluated.</summary>
		private readonly Func<float, float> _function;

		/// <summary>
		/// Flag that indicates wether points from minimum y to the function value should be created or from the function
		/// value to the maximum y value.
		/// </summary>
		private readonly bool _inverted;

		public Vector2 GetPoint(float fraction) {
			return new Vector2(fraction, _function(fraction));
		}

		public Vector2? GetLowerPoint(float fraction) {
			return new Vector2(fraction, ZeroBase ? 0f : _function(fraction));
		}

		public Vector2? GetUpperPoint(float fraction) {
			float value = _function(fraction);
			if (ZeroBase) {
				if (value <= 0f) {
					return null;
				}
			} else {
				if (value >= 1f) {
					return null;
				}
			}
			return new Vector2(fraction, ZeroBase ? _function(fraction) : 1f);
		}

		/// <summary>Create a new instance of this point source.</summary>
		/// <param name="function">Function to use.</param>
		/// <param name="resolution">Resolution.</param>
		/// <param name="inverted">True if should be inverted or not.</param>
		public FunctionSource(Func<float, float> function, int resolution, bool inverted = false) {
			Resolution = resolution;
			_function = function;
			_inverted = inverted;
		}
	}

	public class SquareSource : IPointSource {
		public int Resolution { get; private set; }

		public bool ZeroBase { get { return true; } }

		public Vector2 GetPoint(float fraction) {
			return new Vector2(fraction, 1f);
		}

		public Vector2? GetLowerPoint(float fraction) {
			return new Vector2(fraction, 0f);
		}

		public Vector2? GetUpperPoint(float fraction) {
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
