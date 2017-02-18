﻿using System;

namespace PerformanceTypes
{
    /// <summary>
    /// Allows a string to be hashed character by character using the FNV-1a hashing algorithm.
    /// Always use StringHash.Begin() or StringHash.GetHash() to instantiate.
    /// </summary>
    public struct StringHash : IEquatable<StringHash>
    {
        const uint FNV_PRIME = 16777619;
        const uint FNV_OFFSET_BASIS = 2166136261;

        public uint Value { get; private set; }

        /// <summary>
        /// Returns an initialized StringHash struct which can be used for iterating over characters via Iterate().
        /// </summary>
        public static StringHash Begin()
        {
            var hash = new StringHash();
            hash.Value = FNV_OFFSET_BASIS;
            return hash;
        }

        /// <summary>
        /// Allows you to iteratively calculate the hash value, one character at a time.
        /// </summary>
        public unsafe void Iterate(char c)
        {
            var bytes = (byte*)&c;
            var v = Value;

            v = unchecked((bytes[0] ^ v) * FNV_PRIME);
            v = unchecked((bytes[1] ^ v) * FNV_PRIME);

            Value = v;
        }

        /// <summary>
        /// Returns a calculated StringHash for the string.
        /// </summary>
        public static StringHash GetHash(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            var hash = Begin();
            foreach(var c in s)
                hash.Iterate(c);

            return hash;
        }

        /// <summary>
        /// Returns a calculated StringHash for the characters in the buffer.
        /// </summary>
        /// <param name="buffer">The characters which represent a string to hash.</param>
        /// <param name="start">The offset into the buffer where your string starts.</param>
        /// <param name="count">The length of the string you are hashing.</param>
        public static StringHash GetHash(char[] buffer, int start, int count)
        {
            AssertBufferArgumentsAreSane(buffer.Length, start, count);

            var end = start + count;
            var hash = Begin();
            for (var i = start; i < end; i++)
            {
                hash.Iterate(buffer[i]);
            }

            return hash;
        }

        public static unsafe StringHash GetHash(char* chars, int count)
        {
            var end = chars + count;
            var hash = Begin();
            while (chars < end)
            {
                hash.Iterate(*chars);
                chars++;
            }

            return hash;
        }

        internal static void AssertBufferArgumentsAreSane(int bufferLength, int start, int count)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Start argument cannot be negative");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Length argument cannot be negative");

            if (start + count > bufferLength)
                throw new Exception($"Start ({start}) plus length ({count}) arguments must be less than or equal to buffer.Length ({bufferLength}).");
        }

        public static bool operator ==(StringHash a, StringHash b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(StringHash a, StringHash b)
        {
            return a.Value != b.Value;
        }

        public bool Equals(StringHash other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is StringHash && Equals((StringHash)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}