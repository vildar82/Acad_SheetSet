using System;
using System.Collections.Generic;
using ACSMCOMPONENTS20Lib;
using JetBrains.Annotations;

namespace Acad_SheetSet.Data
{
    public static class SheetSetExt
    {
        public static IEnumerable<TItem> SsToList<TItem, TEnum>(TEnum enumerator,[NotNull] Func<TEnum,TItem> next)
        {
            while (true)
            {
                var item = next(enumerator);
                if (item == null) break;
                yield return item;
            }
        }

        public static object GetCustomPropertyValue([NotNull] this IAcSmComponent comp, [NotNull] string propName)
        {
            var bag = comp.GetCustomPropertyBag();
            var prop = bag.GetProperty(propName);
            return prop.GetValue();
        }
    }
}
