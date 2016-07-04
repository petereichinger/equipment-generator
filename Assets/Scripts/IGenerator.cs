using System.Collections;
using UnityEngine;

namespace EquipmentGenerator {

	public interface IGenerator {

		Mesh Generate(int seed);
	}
}
