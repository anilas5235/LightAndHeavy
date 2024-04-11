using System;
using AttributesLibrary.DrawIfAttribute.Exceptions;
using AttributesLibrary.DrawIfAttribute.Utilities;

namespace AttributesLibrary.DrawIfAttribute
{
    public class NumericType : IEquatable<NumericType>
    {
        private object _value;
        private readonly Type _type;

        public NumericType(object obj)
        {
            if (!obj.IsNumbericType())
            {
                throw new NumericTypeExpectedException("The type of object in the NumericType constructor must be numeric.");
            }
            _value = obj;
            _type = obj.GetType();
        }

        public object GetValue()
        {
            return _value;
        }

        public void SetValue(object newValue)
        {
            _value = newValue;
        }

        public bool Equals(NumericType other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is not NumericType) return GetValue() == obj;

            return Equals((NumericType)obj);
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        public override string ToString()
        {
            return GetValue().ToString();
        }

        /// <summary>
        /// Checks if the value of left is smaller than the value of right.
        /// </summary>
        public static bool operator <(NumericType left, NumericType right)
        {
            var leftValue = left.GetValue();
            var rightValue = right.GetValue();

            return Type.GetTypeCode(left._type) switch
            {
                TypeCode.Byte => (byte)leftValue < (byte)rightValue,
                TypeCode.SByte => (sbyte)leftValue < (sbyte)rightValue,
                TypeCode.UInt16 => (ushort)leftValue < (ushort)rightValue,
                TypeCode.UInt32 => (uint)leftValue < (uint)rightValue,
                TypeCode.UInt64 => (ulong)leftValue < (ulong)rightValue,
                TypeCode.Int16 => (short)leftValue < (short)rightValue,
                TypeCode.Int32 => (int)leftValue < (int)rightValue,
                TypeCode.Int64 => (long)leftValue < (long)rightValue,
                TypeCode.Decimal => (decimal)leftValue < (decimal)rightValue,
                TypeCode.Double => (double)leftValue < (double)rightValue,
                TypeCode.Single => (float)leftValue < (float)rightValue,
                _ => throw new NumericTypeExpectedException("Please compare valid numeric types.")
            };
        }

        /// <summary>
        /// Checks if the value of left is greater than the value of right.
        /// </summary>
        public static bool operator >(NumericType left, NumericType right)
        {
            var leftValue = left.GetValue();
            var rightValue = right.GetValue();

            return Type.GetTypeCode(left._type) switch
            {
                TypeCode.Byte => (byte)leftValue > (byte)rightValue,
                TypeCode.SByte => (sbyte)leftValue > (sbyte)rightValue,
                TypeCode.UInt16 => (ushort)leftValue > (ushort)rightValue,
                TypeCode.UInt32 => (uint)leftValue > (uint)rightValue,
                TypeCode.UInt64 => (ulong)leftValue > (ulong)rightValue,
                TypeCode.Int16 => (short)leftValue > (short)rightValue,
                TypeCode.Int32 => (int)leftValue > (int)rightValue,
                TypeCode.Int64 => (long)leftValue > (long)rightValue,
                TypeCode.Decimal => (decimal)leftValue > (decimal)rightValue,
                TypeCode.Double => (double)leftValue > (double)rightValue,
                TypeCode.Single => (float)leftValue > (float)rightValue,
                _ => throw new NumericTypeExpectedException("Please compare valid numeric types.")
            };
        }

        /// <summary>
        /// Checks if the value of left is the same as the value of right.
        /// </summary>
        public static bool operator ==(NumericType left, NumericType right)
        {
            return !(left > right) && !(left < right);
        }

        /// <summary>
        /// Checks if the value of left is not the same as the value of right.
        /// </summary>
        public static bool operator !=(NumericType left, NumericType right)
        {
            return !(left > right) || !(left < right);
        }

        /// <summary>
        /// Checks if left is either equal or smaller than right.
        /// </summary>
        public static bool operator <=(NumericType left, NumericType right)
        {
            return left == right || left < right;
        }

        /// <summary>
        /// Checks if left is either equal or greater than right.
        /// </summary>
        public static bool operator >=(NumericType left, NumericType right)
        {
            return left == right || left > right;
        }
    }
}