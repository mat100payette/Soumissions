﻿using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Details.Framework;
using Microsoft.LightSwitch.Details.Framework.Base;
using Microsoft.LightSwitch.Framework.Base;
using System;
using System.Reflection;
using System.Windows.Media;

namespace LightSwitchApplication.UserCode
{
    public static class ClientUtilities
    {
        // Convert an HLS value into an RGB value.
        public static Color HSLColor(double h, double s, double l)
        {
            byte r, g, b;

            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (byte)(double_r * 255.0);
            g = (byte)(double_g * 255.0);
            b = (byte)(double_b * 255.0);

            return Color.FromArgb(255, r, g, b);
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }

        public static string Name<TEntity, TDetails>(this EntityStorageProperty<TEntity, TDetails, string>.Entry prop)
            where TEntity : EntityObject<TEntity, TDetails>
            where TDetails : EntityDetails<TEntity, TDetails>, new()
        {
            PropertyInfo pi = prop.GetType().GetInterfaces()[0].GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            object value = pi.GetGetMethod(true).Invoke(prop, new object[]{ });
            return value == null ? string.Empty : value.ToString();
        }

        public static string Name<TEntity, TDetails>(this EntityStorageProperty<TEntity, TDetails, int>.Entry prop)
            where TEntity : EntityObject<TEntity, TDetails>
            where TDetails : EntityDetails<TEntity, TDetails>, new()
        {
            PropertyInfo pi = prop.GetType().GetInterfaces()[0].GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            object value = pi.GetGetMethod(true).Invoke(prop, new object[] { });
            return value == null ? string.Empty : value.ToString();
        }

        public static string Name<TEntity, TDetails, TValue>(this EntityReferenceProperty<TEntity, TDetails, TValue>.Entry prop)
            where TEntity : EntityObject<TEntity, TDetails>
            where TDetails : EntityDetails<TEntity, TDetails>, new()
            where TValue : IEntityObject
        {
            PropertyInfo pi = prop.GetType().GetInterfaces()[0].GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            object value = pi.GetGetMethod(true).Invoke(prop, new object[] { });
            return value == null ? string.Empty : value.ToString();
        }
    }
}
