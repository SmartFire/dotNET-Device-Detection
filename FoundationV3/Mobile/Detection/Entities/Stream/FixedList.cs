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
using FiftyOne.Foundation.Mobile.Detection.Entities.Headers;
using FiftyOne.Foundation.Mobile.Detection.Factories;
using FiftyOne.Foundation.Mobile.Detection.Readers;

namespace FiftyOne.Foundation.Mobile.Detection.Entities.Stream
{
    /// <summary>
    /// <para>
    /// Lists can be stored as a set of related objects entirely within memory, 
    /// or as the relevent objects loaded as required from a file or other 
    /// permanent store.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Delegate methods are used to create new instances of items to add to 
    /// the list in order to avoid creating many inherited list classes for 
    /// each item type.
    /// </remarks>
    /// <remarks>
    /// Should not be referenced directly.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of items the list will contain.
    /// </typeparam>
    /// <typeparam name="D">
    /// The type of the shared data set the item is contained within.
    /// </typeparam>
    public class FixedList<T, D> : BaseList<T, D>, IReadonlyList<T>
        where D : IStreamDataSet
    {
        #region Constructor

        /// <summary>
        /// Constructs a new instance of <see cref="BaseList{T,D}"/> ready to 
        /// read entities from the source.
        /// </summary>
        /// <param name="dataSet">
        /// Dataset being created.
        /// </param>
        /// <param name="reader">
        /// Reader used to initialise the header only.
        /// </param>
        /// <param name="entityFactory">
        /// Used to create new instances of the entity.
        /// </param>
        internal FixedList(
            D dataSet,
            Reader reader,
            BaseEntityFactory<T, D> entityFactory)
            : base(dataSet, reader, entityFactory)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new entity of type T.
        /// </summary>
        /// <param name="index">
        /// The index of the entity being created.
        /// </param>
        /// <param name="reader">
        /// Reader connected to the source data structure and positioned to 
        /// start reading.
        /// </param>
        /// <returns>
        /// A new entity of type T at the index provided.
        /// </returns>
        internal override T CreateEntity(int index, Reader reader)
        {
            reader.BaseStream.Position = 
                Header.StartPosition + (EntityFactory.GetLength() * index);
            return (T)EntityFactory.Create(_dataSet, index, reader);
        }

        /// <summary>
        /// An enumerable that can return a range of T between index and the 
        /// count provided.
        /// </summary>
        /// <param name="index">
        /// First index of the range required.
        /// </param>
        /// <param name="count">
        /// Number of elements to return.
        /// </param>
        /// <returns>
        /// An enumerator for the list.
        /// </returns>
        public IEnumerable<T> GetRange(int index, int count)
        {
            var reader = _dataSet.Pool.GetReader();
            try
            {
                reader.BaseStream.Position = 
                    Header.StartPosition + (EntityFactory.GetLength() * index);
                for (int i = 0; i < count; i++)
                {
                    yield return (T)EntityFactory.Create(_dataSet, index, reader);
                }
            }
            finally
            {
                _dataSet.Pool.Release(reader);
            }
        }

        /// <summary>
        /// An enumeration for the underlying list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in GetRange(0, Count))
            {
                yield return item;
            }
        }

        /// <summary>
        /// An enumeration for the underlying list.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var item in GetRange(0, Count))
            {
                yield return item;
            }
        }

        #endregion
    }
}
