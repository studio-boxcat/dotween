// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/06 18:11
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening.Plugins.Core
{
    static class PluginsManager
    {
        // Default plugins
        static ITweenPlugin _floatPlugin;
        static ITweenPlugin _vector3Plugin;
        static ITweenPlugin _vector3ArrayPlugin;

        // ===================================================================================
        // INTERNAL METHODS ------------------------------------------------------------------

        internal static ABSTweenPlugin<T1, T2, TPlugOptions> GetDefaultPlugin<T1, T2, TPlugOptions>() where TPlugOptions : struct, IPlugOptions
        {
            return GetDefaultPlugin<T1, T2>() as ABSTweenPlugin<T1, T2, TPlugOptions>;
        }

        static ITweenPlugin GetDefaultPlugin<T1, T2>()
        {
            var t1 = typeof(T1);
            var t2 = typeof(T2);

            if (t1 == typeof(float)) return FloatPlugin.Instance;
            if (t1 == typeof(int)) return IntPlugin.Instance;
            if (t1 == typeof(Vector2)) return Vector2Plugin.Instance;
            if (t1 == typeof(Vector3) && t1 == t2) return _vector3Plugin ??= new Vector3Plugin();
            if (t1 == typeof(Color)) return ColorPlugin.Instance;
            if (t1 == typeof(Vector3) && t2 == typeof(Vector3[])) return _vector3ArrayPlugin ??= new Vector3ArrayPlugin();
            throw new NotSupportedException($"Type {t1} is not tweenable");
        }
    }
}