using System;
using System.Collections.Generic;
using Quidgest.Persistence.GenericQuery;
using System.Linq;
using CSGenio.framework;

namespace CSGenio.business.async
{
    public static class Extension
    {
        public static bool IsOk(this StatusMessage sm)
        {
            if (sm == null)
            {
                throw new ArgumentNullException(nameof(sm));
            }

            return sm.Status.IsOk();
        }

        public static bool IsError(this StatusMessage sm)
        {
            if (sm == null)
            {
                throw new ArgumentNullException(nameof(sm));
            }

            return sm.Status.IsError();
        }

        public static bool IsWarning(this StatusMessage sm)
        {
            if (sm == null)
            {
                throw new ArgumentNullException(nameof(sm));
            }

            return sm.Status.IsWarning();
        }

        public static bool IsOk(this Status st)
        {
            if (st == null)
            {
                throw new ArgumentNullException(nameof(st));
            }

            return st == Status.OK;
        }

        public static bool IsError(this Status st)
        {
            if (st == null)
            {
                throw new ArgumentNullException(nameof(st));
            }

            return st == Status.E;
        }

        public static bool IsWarning(this Status st)
        {
            if (st == null)
            {
                throw new ArgumentNullException(nameof(st));
            }

            return st == Status.W;
        }


        public static void RemoveCriteria(this CriteriaSet criteriaSet, Criteria criteria)
        {
            criteriaSet.Criterias.Remove(criteria);
            IList<CriteriaSet> subSets = new List<CriteriaSet>(criteriaSet.SubSets);
            foreach (CriteriaSet subCrit in subSets)
            {
                RemoveCriteria(subCrit, criteria);
                if (subCrit.Criterias.Count == 0 && subCrit.SubSets.Count == 0)
                    criteriaSet.SubSets.Remove(subCrit);
            }
        }

        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Contains(item);
        }


        public static bool In<T>(this T item, IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Contains(item);
        }


        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            System.Reflection.MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static bool Between(this int num, int lower, int upper, bool inclusive = true)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }

        public static T GetValue<K, T>(this IDictionary<K, T> dict, K key, T @default)
        {
            if (key == null || dict.ContainsKey(key))
                return dict[key];

            return @default;
        }
    }
}