using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
    public static class StaticHelpers
    {
        public static Texture2D GetBlueNoise(Runtime.AltosData altosData)
        {
            int noiseId = Time.renderedFrameCount % 64;
            return altosData.textures.blue[noiseId];
        }

        public static Texture2D GetHaltonSequence(Runtime.AltosData altosData)
        {
            return altosData.textures.halton;
        }

        /// <summary>
        /// Maps value in range [iMin, iMax] to range [oMin, oMax].
        /// </summary>
        /// <param name="value"></param>
        /// <param name="iMin"></param>
        /// <param name="iMax"></param>
        /// <param name="oMin"></param>
        /// <param name="oMax"></param>
        /// <returns></returns>
        public static float Remap(float value, float iMin, float iMax, float oMin, float oMax)
        {
            value = Mathf.Clamp(value, iMin, iMax);
            float a = Mathf.InverseLerp(iMin, iMax, value);
            return Mathf.Lerp(oMin, oMax, a);
        }

        /// <summary>
        /// Maps value in range [0, 1] to range [oMin, oMax]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oMin"></param>
        /// <param name="oMax"></param>
        /// <returns></returns>
        public static float RemapFrom01(float value, float oMin, float oMax)
        {
            value = Mathf.Clamp01(value);
            return oMin + (oMax - oMin) * value;
        }

        /// <summary>
        /// Maps value in range [iMin, iMax] to range [0, 1]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="iMin"></param>
        /// <param name="iMax"></param>
        /// <returns></returns>
        public static float RemapTo01(float value, float iMin, float iMax)
        {
            // Ensure value is within the input range
            value = Mathf.Clamp(value, iMin, iMax);

            // Calculate the remapped value in the [0, 1] range
            return (value - iMin) / (iMax - iMin);
        }

        /// <summary>
        /// Returns x mapped to easeOut (equivalent to pow(x, 2.0f)).
        /// </summary>
        /// <param name="x">Value to map along easeIn. Valid range is [0,1].</param>
        /// <returns></returns>
        public static float EaseIn(float x)
        {
            return x * x;
        }

        /// <summary>
        /// Returns x mapped to easeOut (equivalent to pow(x, 0.5)).
        /// </summary>
        /// <param name="x">Value to map along easeOut. Valid range is [0,1].</param>
        /// <returns></returns>
        public static float EaseOut(float x)
        {
            return 1.0f - EaseIn(1.0f - x);
        }

        /// <summary>
        /// Returns x mapped to the easeInOut function.
        /// </summary>
        /// <param name="x">Value to map along easeInOut. Valid range is [0,1].</param>
        /// <returns></returns>
        public static float EaseInOut(float x)
        {
            float a = EaseIn(x);
            float b = EaseOut(x);
            return Mathf.Lerp(a, b, x);
        }

        /// <summary>
        /// Returns the fractional part of x. Unsigned.
        /// </summary>
        /// <param name="x"></param>
        public static float Frac(float x)
        {
            return Mathf.Abs(x - (int)x);
        }

        /// <summary>
        /// Random value from x and seed (optional). Range [0,1].
        /// </summary>
        /// <param name="x"></param>
        /// <param name="seed"></param>
        public static float Random(float x, float seed = 0.546f)
        {
            return Frac(Mathf.Sin((x + 0.3804f) * seed) * 143758.5453f);
        }

        public static Vector2 RandomVector2(float x, float seed = 0.546f)
        {
            return new Vector2(Random(x, seed) * 2.0f - 1.0f, Random(x, seed + 44f) * 2.0f - 1.0f);
        }

        /// <summary>
        /// Returns Smoothstep (EaseInOut) between given inputs a and b by interpolation t.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Smoothstep(float a, float b, float t)
        {
            float x = EaseInOut(t);
            return RemapFrom01(x, a, b);
        }

        /// <summary>
        /// Returns gradient noise at position x. (Warning: Consistently returns to 0 for each integer step).
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float GradientNoise(float x)
        {
            float f = Frac(x);
            float t = EaseInOut(f);

            float previousInclination = (Random(Mathf.Floor(x)) * 2f) - 1f;
            float previousPoint = previousInclination * f;

            float nextInclination = (Random(Mathf.Ceil(x)) * 2f) - 1f;
            float nextPoint = nextInclination * (f - 1f);
            return Mathf.Lerp(previousPoint, nextPoint, t);
        }

        /// <summary>
        /// Returns layered gradient noise in the range [-0.5, 0.5]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="frequency"></param>
        /// <param name="octaves"></param>
        /// <param name="lacunarity"></param>
        /// <param name="gain"></param>
        /// <returns></returns>
        public static float GradientNoiseLayered(float x, float frequency, int octaves, float lacunarity, float gain)
        {
            float v = 0;
            float amp = 1;
            for (int i = 0; i < octaves; i++)
            {
                v += GradientNoise(x * frequency + i * 0.16291f) * amp;
                x *= lacunarity;
                amp *= gain;
            }
            return v;
        }

        /// <summary>
        /// Returns the Density to be used in falloff = (1.0 / (exp(density * distance)) that would yield 98% of the falloff attenuated at the given distance
        /// </summary>
        /// <returns></returns>
        public static float GetDensityFromVisibilityDistance(float distance)
        {
            const float factor = 3.912023005f;
            return factor / distance;
        }

        /// <summary>
        /// Returns the Density to be used in falloff = (1.0 / (exp(density * distance)) that would yield 98% of the falloff attenuated at the given distance
        /// </summary>
        /// <returns></returns>
        public static float GetDensityFromVisibilityDistance(float start, float end)
        {
            return GetDensityFromVisibilityDistance(end - start);
        }

        public static class MatrixOperations
        {
            public static Vector3 GetRightFromMatrix(Matrix4x4 m)
            {
                return new Vector3(m[0, 0], m[1, 0], m[2, 0]).normalized;
            }

            public static Vector3 GetUpFromMatrix(Matrix4x4 m)
            {
                return new Vector3(m[0, 1], m[1, 1], m[2, 1]).normalized;
            }

            public static Vector3 GetForwardFromMatrix(Matrix4x4 m)
            {
                return new Vector3(m[0, 2], m[1, 2], m[2, 2]).normalized;
            }

            public static void DebugMatrixFormatted(Matrix4x4 m)
            {
                string row1 = $"{m[0, 0]:00000.0000}  {m[0, 1]:00000.0000}  {m[0, 2]:00000.0000}  {m[0, 3]:00000.0000}\n";
                string row2 = $"{m[1, 0]:00000.0000}  {m[1, 1]:00000.0000}  {m[1, 2]:00000.0000}  {m[1, 3]:00000.0000}\n";
                string row3 = $"{m[2, 0]:00000.0000}  {m[2, 1]:00000.0000}  {m[2, 2]:00000.0000}  {m[2, 3]:00000.0000}\n";
                string row4 = $"{m[3, 0]:00000.0000}  {m[3, 1]:00000.0000}  {m[3, 2]:00000.0000}  {m[3, 3]:00000.0000}\n";

                Debug.Log(row1 + row2 + row3 + row4);
            }
        }

        public static void ClearRenderTexture(RenderTexture textureToClear)
        {
            RenderTexture activeTexture = RenderTexture.active;
            RenderTexture.active = textureToClear;
            GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 1.0f));
            RenderTexture.active = activeTexture;
        }

        public static void AssignDefaultDescriptorSettings(
            ref RenderTextureDescriptor desc,
            RenderTextureFormat format = RenderTextureFormat.DefaultHDR
        )
        {
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            desc.width = Mathf.Max(1, desc.width);
            desc.height = Mathf.Max(1, desc.height);
            desc.useDynamicScale = false;
            desc.colorFormat = format;
        }

        /// <summary>
        /// Returns a Blackbody color (RGB range [0,1]) from an input color temperature (in Kelvin) of range [2400, 40000].
        /// <br/>
        /// We use a novel model with an r-squared in range [0.916, 0.9966] for accurate, low-cost blackbody calculation.
        /// <br/>
        /// Modeled based on source data of blackbody colors provided by Mitchell Charity:
        /// http://www.vendian.org/mncharity/dir3/blackbody/UnstableURLs/bbr_color.html
        /// </summary>
        /// <param name="temperature">Input temperate, in Kelvins. Clamped to [2400, 40000]K</param>
        /// <returns>Blackbody Color RGB value in range [0,1].</returns>
        public static Vector3 GetBlackbodyColor(float temperature)
        {
            Vector3 col = Vector3.one;

            Vector2 VALID_RANGE = new Vector2(2400, 40000);
            temperature = Mathf.Clamp(temperature, VALID_RANGE.x, VALID_RANGE.y);

            // Handle Red
            // (R^2 = 0.943) across full range
            col.x = 74.4f * Mathf.Pow(temperature, -0.522f);

            // Handle Green
            if (temperature <= 6600)
            {
                // R^2 = 0.987
                col.y = 0.000146f * temperature + 0.0327f;
            }
            else if (temperature > 6600 && temperature < 10500)
            {
                // Roll off adjustment factor towards 10.5K
                col.y = 9.51f * Mathf.Pow(temperature, -0.283f) + Remap(temperature, 6600, 10500, 0.2069f, 0);
            }
            else
            {
                // R^2 = 0.916
                col.y = 9.51f * Mathf.Pow(temperature, -0.283f);
            }

            // Handle Blue (R^2 = 0.998)
            // Returns 0.9966 at 6600
            if (temperature < 6600)
            {
                col.z = 0.000236f * temperature + -0.561f;
            }

            return Saturate(col);
        }

        private static Vector3 Saturate(Vector3 v3)
        {
            return new Vector3(Mathf.Clamp01(v3.x), Mathf.Clamp01(v3.y), Mathf.Clamp01(v3.z));
        }

        /// <summary>
        /// Take in a value in range [0, 1], return back color temperature between [2400, 40000] based on real-world star temperature distribution.
        /// <br/>
        /// Model based on Effective Temperature data available from VizieR:
        /// http://vizier.u-strasbg.fr/viz-bin/VizieR-4
        /// </summary>
        /// <param name="v01">A value between 0 and 1. Used like a "probability" mask against the underlying star temperature algorithm.</param>
        /// <returns>A value in the range of [2400, 40000] in Kelvins</returns>
        //
        public static float GetStarTemperature(float v01)
        {
            float EPSILON = 0.0001f;
            v01 = Mathf.Clamp(v01, EPSILON, 1.0f);
            v01 = 2400 * Mathf.Pow(v01, -0.378f);
            return Mathf.Clamp(v01, 2400, 40000);
        }

        public static float GetStarBrightness(float v01)
        {
            v01 = Mathf.Pow(100, v01) * 0.01f;
            return v01;
        }

        /// <summary>
        /// Creates a quad mesh.
        /// </summary>
        /// <param name="width">Width of the quad mesh.</param>
        /// <param name="height">Height of the quad mesh.</param>
        /// <returns>New mesh of size width x height.</returns>
        public static Mesh CreateQuad(float width = 1f, float height = 1f)
        {
            Mesh mesh = new Mesh();

            float w = width * 0.5f;
            float h = height * 0.5f;

            Vector3[] verts = new Vector3[4] { new Vector3(w, -h, 0), new Vector3(-w, -h, 0), new Vector3(w, h, 0), new Vector3(-w, h, 0) };

            int[] tris = new int[6] { 0, 2, 1, 2, 3, 1 };

            Vector3[] normals = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward, };

            Vector2[] uvs = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), };

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uvs;

            return mesh;
        }

        public static void RenderFeatureOnEnable(
            UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.Scene> action
        )
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += action;
#endif

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += action;
        }

        public static void RenderFeatureOnDisable(
            UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.Scene> action
        )
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode -= action;
#endif

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= action;
        }
    }
}
