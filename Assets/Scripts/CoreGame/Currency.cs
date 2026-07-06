using System;

namespace Game
{
    public struct Currency
    {
        public int money;

        public Currency(int money)
        {
            this.money = money;
        }

        public override bool Equals(object obj)
        {
            return obj is Currency currency && currency == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(money);
        }

        public static Currency operator +(Currency a, Currency b) => new Currency(a.money + b.money);
        public static Currency operator -(Currency a, Currency b) => new Currency(a.money - b.money);
        public static Currency operator *(Currency a, int b) => new Currency(a.money * b);
        public static Currency operator *(int a, Currency b) => new Currency(b.money * a);
        public static Currency operator /(Currency a, int b) => new Currency(a.money / b);

        public static bool operator ==(Currency left, Currency right)
        {
            float n1 = left.money - right.money;
            return n1 * n1 <= float.Epsilon;
        }

        public static bool operator >(Currency left, Currency right)
        {
            return left.money > right.money;
        }

        public static bool operator <(Currency left, Currency right)
        {
            return left.money < right.money;
        }

        public static bool operator >=(Currency left, Currency right)
        {
            return left.money >= right.money;
        }

        public static bool operator <=(Currency left, Currency right)
        {
            return left.money <= right.money;
        }

        public static bool operator !=(Currency left, Currency right) => !(left == right);
    }
}
