using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPAStarTest01.Infrastructure
{
    public interface IId { }

    public class Id<T> : IId
    {
        private readonly int _value; //id는 int로 관리

        private Id(int value)
        {
            _value = value;
        }

        public bool Equals(Id<T> other)
        {
            return _value == other._value;
        }

        public int IdValue
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Id<T> && Equals((Id<T>)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(Id<T> left, Id<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Id<T> left, Id<T> right)
        {
            return !left.Equals(right);
        }

        public static explicit operator int(Id<T> id)
        {
            return id._value;
        }

        public static Id<T> From(int value)
        {
            return new Id<T>(value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
