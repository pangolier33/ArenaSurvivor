using UnityEngine;

namespace Bones.Gameplay.Utils
{
    public static class ColorExtensions
    {
        public static Color SetRGB(this Color original, Color rgb)
        {
            return new Color(rgb.r, rgb.g, rgb.b, original.a);
        }
    }
}