using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EchoEditor
{
    public static class EditorHelper
    {
        static EditorHelper()
        {
            s_AllTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToArray();
        }

        private static readonly Type[] s_AllTypes;

        public static IReadOnlyList<Type> AllTypes => s_AllTypes;

        public static IEnumerable<Type> SearchTypes(Predicate<Type> predicate)
        {
            return s_AllTypes.Where(new Func<Type, bool>(predicate));
        }
    }
}