// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/06 18:11
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DOVector2 = UnityEngine.Vector2;
using DOVector3 = UnityEngine.Vector3;
using DOQuaternion = UnityEngine.Quaternion;
using DOColor = UnityEngine.Color;
using DOVector2Plugin = DG.Tweening.Plugins.Vector2Plugin;
using DOVector3Plugin = DG.Tweening.Plugins.Vector3Plugin;
using DOQuaternionPlugin = DG.Tweening.Plugins.QuaternionPlugin;
using DOColorPlugin = DG.Tweening.Plugins.ColorPlugin;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening.Plugins.Core
{
    internal static class PluginsManager
    {
        // Default plugins
        static ITweenPlugin _floatPlugin;
        static ITweenPlugin _intPlugin;
        static ITweenPlugin _vector2Plugin;
        static ITweenPlugin _vector3Plugin;
        static ITweenPlugin _quaternionPlugin;
        static ITweenPlugin _colorPlugin;
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

            if (t1 == typeof(DOVector3) && t1 == t2) return _vector3Plugin ??= new DOVector3Plugin();
            if (t1 == typeof(DOVector2)) return _vector2Plugin ??= new DOVector2Plugin();
            if (t1 == typeof(DOColor)) return _colorPlugin ??= new DOColorPlugin();
            if (t1 == typeof(float)) return _floatPlugin ??= new FloatPlugin();
            if (t1 == typeof(int)) return _intPlugin ??= new IntPlugin();

            if (t1 == typeof(DOVector3) && t2 == typeof(Vector3[])) return _vector3ArrayPlugin ??= new Vector3ArrayPlugin();

            if (t1 == typeof(DOQuaternion))
            {
                Assert.AreEqual(t2, typeof(DOVector3), "Quaternion tweens require a Vector3 endValue");
                return _quaternionPlugin ??= new DOQuaternionPlugin();
            }

            throw new NotSupportedException($"Type {t1} is not tweenable");
        }
    }
}