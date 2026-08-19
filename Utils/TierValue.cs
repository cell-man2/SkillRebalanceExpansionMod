using System;
using System.Collections.Generic;

namespace SkillRebalanceExpansionMod.Utils
{
    public class TierValue<T>
    {
        private bool _isSingleValue;
        public T Default;
        public Dictionary<int, T> Values = [];

        public T this[int tier]
        {
            get
            {
                if (_isSingleValue) return Default;
                if (Values.TryGetValue(tier, out T value)) return value;
                throw new KeyNotFoundException($"The given key '{tier}' was not present in the dictionary.");
            }
            set
            {
                _isSingleValue = false;
                Values[tier] = value;
            }
        }

        public bool ContainsKey(int tier)
        {
            return _isSingleValue || Values.ContainsKey(tier);
        }

        public bool TryGetValue(int tier, out T value)
        {
            if (_isSingleValue)
            {
                value = Default;
                return true;
            }
            return Values.TryGetValue(tier, out value);
        }

        public static implicit operator TierValue<T>(T value)
        {
            return new TierValue<T>
            {
                _isSingleValue = true,
                Default = value
            };
        }

        public static TierValue<T> Create(
            Func<int, T> selector,
            IEnumerable<int> tiers
        ) {
            TierValue<T> result = new();

            foreach (int tier in tiers)
            {
                result.Values[tier] = selector(tier);
            }

            return result;
        }
    }
}