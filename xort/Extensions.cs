//************************************************************************************************
// Copyright © 2016 Steven M Cohn.  All rights reserved.
//************************************************************************************************

namespace xort
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Linq;


	/// <summary>
	/// Extension methods used by xort.
	/// </summary>

	public static class Extensions
	{

		/// <summary>
		/// While XML is case sensitive, this provides a clean encapsulation of comparing
		/// two strings ignoring case.
		/// </summary>
		/// <param name="s">The primary string</param>
		/// <param name="value">The secondary string to compare to primary</param>
		/// <returns>True if secondary is equal to primary.</returns>

		public static bool EqualsICIC (this string s, string value)
		{
			return s.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}


		/// <summary>
		/// Determines whether two XML elements are equivalent by weighting the name and
		/// attributes of the element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="candidate"></param>
		/// <returns>
		/// The weighted score of the comparison.  A minimum score of 50 means the element
		/// names are the same.  Then for each attribute, a value is added to the score that
		/// weighs attribute names higher than attribute values.
		/// </returns>

		public static int Equivalence (this XElement element, XElement candidate)
		{
			double score = 0.0; // 0..100

			if (element.Name.LocalName.EqualsICIC(candidate.Name.LocalName) &&
				element.Name.NamespaceName.EqualsICIC(candidate.Name.NamespaceName))
			{
				score += 50.0; // element name is worth 50%

				var attributes = element.Attributes().Where(
					a => candidate.Attributes().Any(k => k.Name.LocalName.Equals(a.Name.LocalName)));

				if (attributes.Count() > 0)
				{
					int n = element.Attributes().Count();
					double nvvalue = 50.0 / n;			// att nam+val may equal up to 1/nth of 50%
					double namvalue = nvvalue * 0.75;	// att nam equals 75% of (1/nth of 50%)
					double valvalue = nvvalue * 0.25;	// att val equals 25% of (1/nth of 50%)

					foreach (var attribute in attributes)
					{
						score += namvalue;
						var value = candidate.Attribute(attribute.Name).Value;
						if (attribute.Value.EqualsICIC(value))
						{
							score += valvalue;
						}
					}
				}
			}

			return (int)score;
		}

		public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
		{
			return source.MaxBy(selector, Comparer<TKey>.Default);
		}

		public static T MaxBy<T, TKey>(
			this IEnumerable<T> source, Func<T, TKey> selector, IComparer<TKey> comparer)
		{
			using (var e = source.GetEnumerator())
			{
				if (!e.MoveNext())
				{
					return default(T);
				}
				var max = e.Current;
				var maxKey = selector(max);
				while (e.MoveNext())
				{
					var candidate = e.Current;
					var candidateProjected = selector(candidate);
					if (comparer.Compare(candidateProjected, maxKey) > 0)
					{
						max = candidate;
						maxKey = candidateProjected;
					}
				}
				return max;
			}
		}
	}
}
