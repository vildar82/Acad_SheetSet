// Khisyametdinovvt Хисяметдинов Вильдар Тямильевич
// 2018 04 25 15:29

using System;
using System.Collections.Generic;
using System.Linq;
using Acad_SheetSet.Props;
#if v2016
using ACSMCOMPONENTS20Lib;
#elif v2017
using acsmcomponents21;
#elif v2018
using ACSMCOMPONENTS22Lib;
#endif
using JetBrains.Annotations;

namespace Acad_SheetSet.Data
{
    public static class SheetSetExt
    {
        [CanBeNull]
        public static object GetCustomPropertyValue([NotNull] this IAcSmComponent comp, [NotNull] string propName)
        {
            var prop = comp.GetProperty(propName);
            return prop?.GetValue();
        }

        public static AcSmCustomPropertyValue GetProperty([NotNull] this IAcSmComponent comp, string propName)
        {
            var bag = comp.GetCustomPropertyBag();
            return bag.GetProperty(propName);
        }

        public static void SetCustomPropertyValue([NotNull] this IAcSmComponent comp, [NotNull] string propName, object value)
        {
            var prop = comp.GetProperty(propName);
            if (prop == null) throw new KeyNotFoundException($"Не найден параметр '{propName}'");
            prop.SetValue(value);
        }

        public static IEnumerable<TItem> SsToList<TItem, TEnum>(TEnum enumerator, [NotNull] Func<TEnum, TItem> next)
        {
            while (true)
            {
                var item = next(enumerator);
                if (item == null) break;
                yield return item;
            }
        }

        [NotNull]
        public static List<SSProp> GetCustomProperties([NotNull] this IAcSmComponent comp, bool removeProps = false, 
            [CanBeNull] SheetSet ss = null)
        {
            var cpb = comp.GetCustomPropertyBag();
            var props = new List<SSProp>();
            var cpbPropNames = new List<string>();
            var cpbList = SsToList(cpb.GetPropertyEnumerator(), e=>
            {
                e.Next(out var propName, out var item);
                cpbPropNames.Add(propName);
                return item;
            }).ToList();
            for (var i = 0; i < cpbList.Count; i++)
            {
                var cpbItem = cpbList[i];
                var propName = cpbPropNames[i];
                var flags = cpbItem.GetFlags();
                if (flags != PropertyFlags.CUSTOM_SHEETSET_PROP && flags != PropertyFlags.CUSTOM_SHEET_PROP) continue;
                var value = cpbItem.GetValue()?.ToString();
                var prop = new SSProp
                {
                    Name = propName,
                    Value = value,
                    Type = GetPropType(flags)
                };
                props.Add(prop);
                if (removeProps)
                {
                    if (flags == PropertyFlags.CUSTOM_SHEET_PROP && ss != null)
                    {
                        // Удалить это свойство во всех листах
                        var sheets = ss.Nodes.SelectMany(s => s.GetSheets());
                        foreach (var sheet in sheets)
                        {
                            try
                            {
                                sheet.sheet.GetCustomPropertyBag().SetProperty(propName, null);
                            }
                            catch
                            {
                                //
                            }
                        }
                    }
                    cpb.SetProperty(propName,null);
                }
            }
            return props;
        }

        private static PropType GetPropType(PropertyFlags flags)
        {
            return flags == PropertyFlags.CUSTOM_SHEET_PROP ? PropType.Sheet : PropType.SheetSet;
        }
        private static PropertyFlags  GetPropType(PropType flags)
        {
            return flags == PropType.SheetSet ? PropertyFlags.CUSTOM_SHEETSET_PROP : PropertyFlags.CUSTOM_SHEET_PROP;
        }

        public static void AddCustomProperty([NotNull] this AcSmCustomPropertyBag bag, [NotNull] IAcSmComponent comp,
            [NotNull] SSProp prop, [CanBeNull] SheetSet ss = null)
        {
            var customProp = new AcSmCustomPropertyValue();
            customProp.InitNew(comp);
            customProp.SetFlags(GetPropType(prop.Type));
            customProp.SetValue(prop.Value);
            bag.SetProperty(prop.Name, customProp);
            if (prop.Type == PropType.Sheet && ss != null)
            {
                foreach (var sheet in ss.Nodes.SelectMany(s=>s.GetSheets()))
                {
                    var sheetBag = sheet.sheet.GetCustomPropertyBag();
                    sheetBag?.AddCustomProperty(sheet.sheet, prop);
                }
            }
        }
    }
}