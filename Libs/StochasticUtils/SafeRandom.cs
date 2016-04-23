﻿using System;
using System.Numerics;
using System.Security.Cryptography;

namespace MpcLib.Common.StochasticUtils
{
	/// <summary>
	/// Implements a safe random number generator seeded using RNGCryptoServiceProvider.
	/// This RNG is safe meaning that the default seed value is time-independent so if
	/// more than one instance of it is created at the same "time", the random sequence
	/// generated by each instance is completely different (independent) from other instances'.
    /// WARNING: The sequence of random numbers generated by this method is NOT cryptographically-secure. 
    /// DO NOT USE IN REAL SITUATIONS.
	/// </summary>
	public class SafeRandom
	{
		public int Seed { get; private set; }

		private Random rand;

		public SafeRandom()
		{
			var rng = new RNGCryptoServiceProvider();
			var buffer = new byte[4];
			rng.GetBytes(buffer);
			Seed = BitConverter.ToInt32(buffer, 0);
			rand = new Random(Seed);
		}

		public SafeRandom(int seed)
		{
			rand = new Random(seed);
			Seed = seed;
		}

		/// <summary>
		/// Returns a random number within a specified range.
		/// </summary>
		/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
		/// <param name="maxValue">
		/// The exclusive upper bound of the random number returned. maxValue must be
		/// greater than or equal to minValue.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to minValue and less than maxValue;
		/// that is, the range of return values includes minValue but not maxValue. If
		/// minValue equals maxValue, minValue is returned.
		/// </returns>
		public int Next(int minValue, int maxValue)
		{
			return rand.Next(minValue, maxValue);
		}

        public BigInteger Next(BigInteger maxValue)
        {
            int bits = (int) Math.Ceiling(BigInteger.Log(maxValue, 2));
            int bytes = bits / 8 + ((bits % 8 == 0) ? 0 : 1);

            int bitsToZero = (bits % 8 == 0) ? 0 : (8 - bits % 8);


            byte[] data = new byte[bytes];
            while (true)
            {
                rand.NextBytes(data);
                for (int i = 0; i < bitsToZero; i++)
                {
                    data[data.Length - 1] &= (byte) ~(1 << (8 - i));
                }

                BigInteger ret = new BigInteger(data);
                if (ret < maxValue)
                    return ret;
            }
        }
	}
}