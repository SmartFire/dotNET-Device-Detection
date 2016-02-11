﻿/* *********************************************************************
 * This Source Code Form is copyright of 51Degrees Mobile Experts Limited. 
 * Copyright © 2015 51Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY
 * 
 * This Source Code Form is the subject of the following patent 
 * applications, owned by 51Degrees Mobile Experts Limited of 5 Charlotte
 * Close, Caversham, Reading, Berkshire, United Kingdom RG4 7BY: 
 * European Patent Application No. 13192291.6; and
 * United States Patent Application Nos. 14/085,223 and 14/085,301.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0.
 * 
 * If a copy of the MPL was not distributed with this file, You can obtain
 * one at http://mozilla.org/MPL/2.0/.
 * 
 * This Source Code Form is “Incompatible With Secondary Licenses”, as
 * defined by the Mozilla Public License, v. 2.0.
 * ********************************************************************* */

using System.Collections.Generic;
using System.IO;
using System;
namespace FiftyOne.Foundation.Mobile.Detection.Entities
{
    /// <summary>
    /// Base class for all entities in the <see cref="DataSet"/>.
    /// </summary>
    /// <para>
    /// All entities must belong to a data set and contain a unique integer key.
    /// This class provides this functionality along with many common methods
    /// used by multiple entities.
    /// </para>
    /// <para>
    /// For more information see https://51degrees.com/Support/Documentation/Net
    /// </para>
    public abstract class BaseEntity : IComparable<BaseEntity>, IComparable<int>, IEquatable<int>
    {
        #region Constants

        /// <summary>
        /// List if powers used to determine numeric differences.
        /// </summary>
        private static readonly int[] Powers = new[] { 1, 10, 100, 1000, 10000 };

        #endregion

        #region Properties

        /// <summary>
        /// The data set the entity relates to.
        /// </summary>
        public readonly DataSet DataSet;

        /// <summary>
        /// The unique index of the item in the collection of items, or
        /// the unique offset to the item in the source data structure.
        /// </summary>
        public readonly int Index;

        #endregion

        #region Delegates

        /// <summary>
        /// Used to create a new instance of an entity.
        /// </summary>
        /// <param name="dataSet">
        /// The <see cref="DataSet"/> being created or added to.
        /// </param>
        /// <param name="indexOrOffset">
        /// The unique index of offset of the entity in the list
        /// </param>
        /// <param name="reader">
        /// Reader connected to the source data structure and positioned to 
        /// start reading.
        /// </param>
        /// <returns>A new instance of the entity</returns>
        /// <para>
        /// The delegate is used to enable inheriting classes to control how 
        /// lists containing entities of their type are created. When generic 
        /// types support parameterised constructors the need for this 
        /// delegate will be removed.
        /// </para>
        internal delegate BaseEntity Create(DataSet dataSet, int indexOrOffset, BinaryReader reader);

        /// <summary>
        /// Used to return the length in bytes of an entity when stored in the 
        /// underlying data structure.
        /// </summary>
        /// <param name="entity">Entity whose length is required</param>
        /// <returns>The length in bytes of the entity</returns>
        internal delegate int GetLength(BaseEntity entity);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the base item for the data set and index provided.
        /// </summary>
        /// <param name="dataSet">
        /// The <see cref="DataSet"/> being created
        /// </param>
        /// <param name="indexOrOffset">
        /// The unique index of the item in the collection of items, or
        /// the unique offset to the item in the source data structure.
        /// </param>
        internal BaseEntity(DataSet dataSet, int indexOrOffset)
        {
            DataSet = dataSet;
            Index = indexOrOffset;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the integer index or offset provided to the this entities.
        /// </summary>
        /// <param name="indexOrOffset">
        /// The unique index of the item in the collection of items, or
        /// the unique offset to the item in the source data structure.
        /// </param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and 
        /// value.
        /// <list type="bullet">
        /// <item>Less than zero: This instance is less than value.</item>
        /// <item>Zero: This instance is equal to value.</item>
        /// <item>Greater than zero: This instance is greater than value.</item>
        /// </list>
        /// </returns>
        public int CompareTo(int indexOrOffset)
        {
            return Index.CompareTo(indexOrOffset);
        }

        /// <summary>
        /// Compares entities based on their Index properties.
        /// </summary>
        /// <param name="other">
        /// The entity to be compared against.
        /// </param>
        /// <returns>
        /// The position of one entity over the other.
        /// </returns>
        public int CompareTo(BaseEntity other)
        {
            return CompareTo(other.Index);
        }
        
        /// <summary>
        /// Evaluates the index of the entity to the value provided for equality.
        /// </summary>
        /// <param name="other">Value to evaluate for equality</param>
        /// <returns>True if the other value equals the index, otherwise false</returns>
        public bool Equals(int other)
        {
            return Index.Equals(other);
        }

        /// <summary>
        /// Returns the index as the hash code for the entity. This can only be considered
        /// when using entities from the same data set.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Index;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// An enumerable that can be used to read through the entries.
        /// </summary>
        /// <param name="reader">Reader set to the position at the start of the list</param>
        /// <param name="count">The number of integers to read to form the array</param>
        /// <returns>Iterator to read each integer entry.</returns>
        protected static IEnumerable<int> GetIntegerEnumerator(BinaryReader reader, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return reader.ReadInt32();
            }
        }

        /// <summary>
        /// Reads an integer array where the first integer is the number of 
        /// following integers.
        /// </summary>
        /// <param name="reader">Reader set to the position at the start of the list</param>
        /// <param name="count">The number of integers to read to form the array</param>
        /// <returns>An array of integers</returns>
        protected static int[] ReadIntegerArray(BinaryReader reader, int count)
        {
            var array = new int[count];
            for (int i = 0; i < array.Length; i++)
                array[i] = reader.ReadInt32();
            return array;
        }

        /// <summary>
        /// Returns an integer representation of the characters between start and end.
        /// Assumes that all the characters are numeric characters.
        /// </summary>
        /// <param name="array">
        /// Array of characters with numeric characters present between start and end
        /// </param>
        /// <param name="start">
        /// The first character to use to convert to a number
        /// </param>
        /// <param name="length">
        /// The number of characters to use in the conversion
        /// </param>
        /// <returns></returns>
        internal static int GetNumber(byte[] array, int start, int length)
        {
            int value = 0;
            for (int i = start + length - 1, p = 0; i >= start && p < Powers.Length; i--, p++)
            {
                value += Powers[p] * ((byte)array[i] - (byte)'0');
            }
            return value;
        }

        /// <summary>
        /// Determines if the value is an ASCII numeric value.
        /// </summary>
        /// <param name="value">Byte value to be checked</param>
        /// <returns>True if the value is an ASCII numeric character</returns>
        internal static bool GetIsNumeric(byte value)
        {
            return (value >= (byte)'0' && value <= (byte)'9');
        }

        #endregion
    }
}
