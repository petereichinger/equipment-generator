using System;
using System.Collections;
using System.Net;
using UnityEngine;

namespace EquipmentGenerator {

	public static class Math {

		/// <summary>
		/// Calculate x mod m. Correctly handles negative values. Resuling value is always in the range [0;m-1]
		/// </summary>
		/// <param name="x">Value</param>
		/// <param name="m">Modus</param>
		/// <returns>Modulus m of x. Value between 0 and m-1.</returns>
		public static int Mod(int x, int m) {
			int r = x % m;
			return r < 0 ? r + m : r;
		}

		/// <summary>Swap the values <paramref name="first"/> and <paramref name="second"/>.</summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="first">First variable.</param>
		/// <param name="second">Second variable.</param>
		public static void Swap<T>(ref T first, ref T second) {
			T temp = first;
			first = second;
			second = temp;
		}

		/// <summary>
		/// Swap the values <paramref name="first"/> and <paramref name="second"/> if <paramref name="first"/> is greater
		/// than <paramref name="second"/>.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="first">First variable.</param>
		/// <param name="second">Second variable.</param>
		public static void SwapIfGreater<T>(ref T first, ref T second) where T : IComparable<T> {
			if (first.CompareTo(second) > 0) {
				Swap(ref first, ref second);
			}
		}
	}
}
