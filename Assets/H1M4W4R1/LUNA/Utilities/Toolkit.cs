using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace H1M4W4R1.LUNA.Utilities
{
    public class Toolkit
    {
        /// <summary>
        /// Registers a list of data to the target list.
        /// </summary>
        /// <typeparam name="T">The type of the data. Must be unmanaged.</typeparam>
        /// <param name="data">The list of data to register.</param>
        /// <param name="targetList">The target list where the data will be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when the data list is null.</exception>
        public static void RegisterData<T>(List<T> data, out UnsafeList<T> targetList) where T : unmanaged
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            targetList = new UnsafeList<T>(data.Count, Allocator.Domain);
            foreach (var obj in data) targetList.Add(obj);
        }
    }
}