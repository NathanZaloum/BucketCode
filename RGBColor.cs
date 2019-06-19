using UnityEngine;

namespace ColorConverter {

    public class RGBColor {

	    public static Color RGBtoFloat (float r, float g, float b, float a) {

            Color c;

            // Check if any inputed values are greater than 255 and if they are then round them to 255.
            if (r > 255.0f) {
                r = 255.0f;
            }
            if (g > 255.0f) {
                g = 255.0f;
            }
            if (b > 255.0f) {
                b = 255.0f;
            }
            if (a > 255.0f) {
                a = 255.0f;
            }

            // Check if any inputed values are less than 0 and if they are then round them to 0.
            if (r < 0.0f) {
                r = 0.0f;
            }
            if (g < 0.0f) {
                g = 0.0f;
            }
            if (b < 0.0f) {
                b = 0.0f;
            }
            if (a < 0.0f) {
                a = 0.0f;
            }

            // Divide the values by 255 to get a decimal between 0 and 1.
            r /= 255.0f;
            g /= 255.0f;
            b /= 255.0f;
            a /= 255.0f;

            // Combine all variables into a new Color and return that color.
            c = new Color (r, g, b, a);
            return c;
        }
    }
}