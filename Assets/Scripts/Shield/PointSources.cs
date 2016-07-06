﻿using System;
using UnityEngine;
using System.Collections.Generic;

namespace EquipmentGenerator {
	/// <summary>
	/// Interface for a point source that can be used in <see cref="Generate"/>. Point sources always operate in the
	/// range [0:1].
	/// </summary>
	public interface IPointSource {
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
		void GetNextPoints(float fraction, List<Vector3> nextPointsList);
	}

	/// <summary>
	/// Point source that uses a <see cref="System.Func{T,TResult}"/> with ( <see cref="float"/> as generic parameters).
	/// </summary>
	public class FunctionPointSource : IPointSource {
		public int Resolution { get; private set; }

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
		public void GetNextPoints(float fraction, List<Vector3> nextPointsList) {
			float x = fraction;
			float value = _function(x);

			if (!_inverted) {
				nextPointsList.Add(new Vector3(x, _minY, 0));
				if (value > _minY) {
					nextPointsList.Add(new Vector3(x, value, 0));
				}
			} else {
				if (value < _maxY) {
					nextPointsList.Add(new Vector3(x, value, 0));
				}
				nextPointsList.Add(new Vector3(x, _maxY, 0));
			}
		}

		/// <summary>Create a new instance of this point source.</summary>
		/// <param name="function">Function to use.</param>
		/// <param name="resolution">Resolution.</param>
		/// <param name="inverted">True if should be inverted or not.</param>
		public FunctionPointSource(Func<float, float> function, int resolution, bool inverted = false) {
			Resolution = resolution;
			_function = function;
			_inverted = inverted;
			_minY = _function(0f);
			_maxY = _function(0f);

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

	public class SquarePointSource : IPointSource {
		public int Resolution { get; private set; }

		public void GetNextPoints(float fraction, List<Vector3> nextPointsList) {
			nextPointsList.Add(new Vector3(fraction, 0));
			nextPointsList.Add(new Vector3(fraction, 1));
		}

		public SquarePointSource(int resolution) {
			Resolution = resolution;
		}
	}
}