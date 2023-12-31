﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoEditor
{
    public static class GUIStyleGetter
    {
        private static Dictionary<string, GUIStyle> m_StyleMap = new Dictionary<string, GUIStyle>();

        public static GUIStyle Get(string styleName)
        {
            if (m_StyleMap.TryGetValue(styleName, out GUIStyle style) == false)
            {
                style                 = styleName;
                m_StyleMap[styleName] = style;
            }

            return style;
        }
    }
}