using System.Collections;
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
	}
}
