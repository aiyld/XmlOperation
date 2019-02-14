using System;
using System.Collections.Generic;

namespace XmlOperation.LiDongYang
{
    public class BaseTypeCollection
    {
        /// <summary>
        /// Get all base types
        /// </summary>
        /// <returns>All base types</returns>
        public static IEnumerable<Type> GetBaseTypes()
        {
            yield return typeof(int);
            yield return typeof(Int16);
            yield return typeof(Int32);
            yield return typeof(Int64);
            yield return typeof(string);
            yield return typeof(decimal);
            yield return typeof(double);
            yield return typeof(DateTime);
            yield return typeof(object);
            yield return typeof(byte);
            yield return typeof(float);
            yield return typeof(Nullable);
            yield return typeof(bool);
            yield return typeof(char);
            yield return typeof(IntPtr);
            yield return typeof(short);
            yield return typeof(long);
            yield return typeof(sbyte);
            yield return typeof(Single);
            yield return typeof(uint);
            yield return typeof(UInt16);
            yield return typeof(UInt32);
            yield return typeof(UInt64);
            yield return typeof(UInt64);
            yield return typeof(UIntPtr);
            yield return typeof(ulong);
        }
    }
}
