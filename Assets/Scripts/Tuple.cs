﻿using System;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>Class representing a generic tuple.</summary>
/// <typeparam name="T1">Type of the first value in the tuple. See <see cref="Value1"/>.</typeparam>
/// <typeparam name="T2">Type of the second value in the tuple. See <see cref="Value2"/>.</typeparam>
public class Tuple<T1, T2> {

	/// <summary>First value in the tuple.</summary>
	public T1 Value1 { get; set; }

	/// <summary>Second value in the tuple.</summary>
	public T2 Value2 { get; set; }

	/// <summary>
	/// Create a new Tuple with the default values of <typeparamref name="T1"/> and <typeparamref name="T2"/>.
	/// </summary>
	public Tuple() {
		Value1 = default(T1);
		Value2 = default(T2);
	}

	/// <summary>
	/// Create a new Tuple with the specified values for <see cref="Value1"/> and <see cref="Value2"/>.
	/// </summary>
	/// <param name="val1">First value.</param>
	/// <param name="val2">Second value.</param>
	public Tuple(T1 val1, T2 val2) {
		Value1 = val1;
		Value2 = val2;
	}

	/// <summary>
	/// Clear the tuple. This assigns <c>default(T)</c> to <see cref="Value1"/> and <see cref="Value2"/>.
	/// </summary>
	public void Clear() {
		Value1 = default(T1);
		Value2 = default(T2);
	}

	/// <summary>Copy the values from another Tuple.</summary>
	/// <param name="other">The other tuple.</param>
	public void CopyFrom(Tuple<T1, T2> other) {
		Value1 = other.Value1;
		Value2 = other.Value2;
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

	/// <summary>Check if both values of a Tuple with nullable values are set.</summary>
	/// <typeparam name="T1">Type <typeparamref name="T1"/> of <see cref="Tuple{T1,T2}"/>.</typeparam>
	/// <typeparam name="T2">Type <typeparamref name="T2"/> of <see cref="Tuple{T1,T2}"/>.</typeparam>
	/// <param name="tuple">Tuple to check.</param>
	/// <returns><c>true</c> if both values are set, <c>false</c> otherwise.</returns>
	public static bool BothNotNull<T1, T2>(Tuple<T1?, T2?> tuple)
		where T1 : struct
		where T2 : struct {
		return tuple.Value1.HasValue && tuple.Value2.HasValue;
	}

	/// <summary>
	/// Get a number representing the number of values that are not null in <paramref name="tuple"/>. <c>0</c> if both
	/// <see cref="Tuple{T1,T2}.Value1"/> and <see cref="Tuple{T1,T2}.Value2"/> have no value, <c>1</c> if either <see
	/// cref="Tuple{T1,T2}.Value1"/> or <see cref="Tuple{T1,T2}.Value2"/> have a value, <c>2</c> otherwise.
	/// </summary>
	/// <typeparam name="T1">Type of <see cref="Tuple{T1,T2}.Value1"/>. Must be a <c>struct</c>.</typeparam>
	/// <typeparam name="T2">Type of <see cref="Tuple{T1,T2}.Value2"/>. Must be a <c>struct</c>.</typeparam>
	/// <param name="tuple">Tuple to count.</param>
	/// <returns>
	/// <c>0</c> if both <see cref="Tuple{T1,T2}.Value1"/> and <see cref="Tuple{T1,T2}.Value2"/> have no value, <c>1</c>
	/// if either <see cref="Tuple{T1,T2}.Value1"/> or <see cref="Tuple{T1,T2}.Value2"/> have a value, <c>2</c>
	/// otherwise.
	/// </returns>
	public static int NullableHasValueCount<T1, T2>(Tuple<T1?, T2?> tuple)
		where T1 : struct
		where T2 : struct {
		return (tuple.Value1.HasValue ? 1 : 0) +
			(tuple.Value2.HasValue ? 1 : 0);
	}

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
