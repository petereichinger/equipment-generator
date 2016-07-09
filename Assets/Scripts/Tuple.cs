using System;
using System.Runtime.InteropServices;

/// <summary>Class representing a generic tuple.</summary>
/// <typeparam name="T1">Type of the first value in the tuple. See <see cref="Value1"/>.</typeparam>
/// <typeparam name="T2">Type of the second value in the tuple. See <see cref="Value2"/>.</typeparam>
public class Tuple<T1, T2> {

	/// <summary>First value in the tuple.</summary>
	public T1 Value1 { get; set; }

	/// <summary>Second value in the tuple.</summary>
	public T2 Value2 { get; set; }

	public Tuple(T1 val1, T2 val2) {
		Value1 = val1;
		Value2 = val2;
	}

	public void Clear(T1 val1 = default(T1), T2 val2 = default(T2)) {
		Value1 = default(T1);
		Value2 = default(T2);
	}
}

/// <summary>Class representing a generic triple.</summary>
/// <typeparam name="T1">Type of the first value in the triple. See <see cref="Value1"/>.</typeparam>
/// <typeparam name="T2">Type of the second value in the triple. See <see cref="Value2"/>.</typeparam>
/// <typeparam name="T3">Type of the third value in the triple. See <see cref="Value3"/>.</typeparam>
public class Tuple<T1, T2, T3> {

	/// <summary>First value in the triple.</summary>
	public T1 Value1 { get; set; }

	/// <summary>Second value in the triple.</summary>
	public T2 Value2 { get; set; }

	/// <summary>Second value in the triple.</summary>
	public T3 Value3 { get; set; }

	public Tuple(T1 val1, T2 val2, T3 val3) {
		Value1 = val1;
		Value2 = val2;
		Value3 = val3;
	}
}

/// <summary>Helper class for creating tuples. Just use <c>Tuple.Create(...)</c>.</summary>
public static class Tuple {

	public static Tuple<T1?, T1> CreateNullValue<T1>(T1 val1) where T1 : struct {
		return new Tuple<T1?, T1>(null, val1);
	}

	public static Tuple<T1, T1?> CreateValueNull<T1>(T1 val1) where T1 : struct {
		return new Tuple<T1, T1?>(val1, null);
	}

	/// <summary>Create a new tuple with the specified values.</summary>
	/// <param name="val1">First value.</param>
	/// <param name="val2">Second value.</param>
	/// <returns>An instance with the specified values.</returns>
	public static Tuple<T1, T2> Create<T1, T2>(T1 val1, T2 val2) {
		return new Tuple<T1, T2>(val1, val2);
	}

	/// <summary>Create a new tuple with the specified values.</summary>
	/// <param name="val1">First value.</param>
	/// <param name="val2">Second value.</param>
	/// <param name="val3">Third value.</param>
	/// <returns>An instance with the specified values.</returns>
	public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 val1, T2 val2, T3 val3) {
		return new Tuple<T1, T2, T3>(val1, val2, val3);
	}
}
