using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Echo.Editor
{
    public static class EditorHelper
    {
        static EditorHelper()
        {
            s_AllTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .ToArray();
        }

        private static GUIContent s_TempContent = new GUIContent();
        private static Type[]     s_AllTypes;

        public static GUIContent TempContent(string text, string tooltip = null, Texture icon = null)
        {
            s_TempContent.text    = text;
            s_TempContent.tooltip = tooltip;
            s_TempContent.image   = icon;
            return s_TempContent;
        }

        public static IEnumerable<Type> SearchTypes(Func<Type, bool> condition)
        {
            return s_AllTypes.Where(condition);
        }
    }
}