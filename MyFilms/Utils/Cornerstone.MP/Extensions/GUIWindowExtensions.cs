﻿using System.Collections;
using System.Reflection;
using MediaPortal.GUI.Library;

namespace MyFilmsPlugin.Utils.Cornerstone.MP.Extensions
{
  public static class GUIWindowExtensions
    {
        /// <summary>
        /// Same as Children and controlList but used for backwards compatibility between mediaportal 1.1 and 1.2
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable GetControlList(this GUIWindow self)
        {
            PropertyInfo property = GUIFacadeControlExtensions.GetPropertyInfo<GUIWindow>("Children", null);
            return (IEnumerable)property.GetValue(self, null);
        }
    }
}
