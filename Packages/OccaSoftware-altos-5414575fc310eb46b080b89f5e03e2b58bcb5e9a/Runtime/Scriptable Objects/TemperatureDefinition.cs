using System.Collections.Generic;

using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
    [CreateAssetMenu(fileName = "Temperature Definition", menuName = "Altos/Temperature Definition")]
    public class TemperatureDefinition : ScriptableObject
    {
        [Tooltip("Sets the temperature at sea level, in celsius.")]
        public float averageTemperatureAtSeaLevel = 15f;

        [Tooltip(
            "Diurnal temperature variation is the variation between a high air temperature and a low temperature that occurs during the same day. See https://en.wikipedia.org/wiki/Diurnal_air_temperature_variation"
        )]
        public float maximumDiurnalTemperatureVariation = TemperatureData.TypicalThermalAmplitude;

        [Tooltip("A temperature anomaly is the departure of a temperature from the average.")]
        public float maximumTemperatureAnomaly = TemperatureData.TypicalAnomaly;

        public bool dynamicTemperature = true;

        [Header("Debugging")]
        [Tooltip("Set automatically by Altos.")]
        public float currentTemperatureAtSeaLevel = 15f;

        // To do QoL:
        // Clarify units
        // Show freezing point altitude in editor

        public static class TemperatureData
        {
            // Reference: https://www.omnicalculator.com/physics/altitude-temperature#temperature-vs-altitude-chart-analysis-why-does-temperature-decrease-with-higher-altitude
            // "As the graph shows, temperature either decreases, remains constant, or increases with higher altitude. [...]
            // The temperature decreases by about 6.5 °C per km, or 18.8 °F per mi"
            public const float TemperatureChangePerMeter = -0.0065f;

            /// <summary>
            /// The freezing point is the temperature below which water will freeze.
            /// </summary>
            public const float FreezingPoint = 0f;

            /// <summary>
            /// A reference typical thermal amplitude, from Wikipedia.
            /// </summary>
            public const float TypicalThermalAmplitude = 10f;

            /// <summary>
            /// A reference typical thermal anomaly, from Wikipedia.
            /// </summary>
            public const float TypicalAnomaly = 3f;
        }

        /// <summary>
        /// This method is called when we want the temperature definition to update the current temperature.
        /// </summary>
        /// <param name="time"></param>
        public void UpdateTemperature(float time)
        {
            if (overrideState == OverrideState.Overriden)
                return;

            if (dynamicTemperature)
            {
                currentTemperatureAtSeaLevel = GetTemperatureAtTime(time);
            }
            else
            {
                currentTemperatureAtSeaLevel = averageTemperatureAtSeaLevel;
            }
        }

        /// <summary>
        /// Get the temperate at a given altitude.
        /// </summary>
        /// <param name="altitude">Altitude. Assumed to be in offset space.</param>
        /// <returns>Temperature (in Celsius)</returns>
        public float GetTemperatureAtAltitude(float altitude)
        {
            // Linear, y = mx+b
            // Where x = altitude
            altitude = AltosSkyDirector.Instance.TransformOffsetToWorldPosition(altitude);
            return TemperatureData.TemperatureChangePerMeter * altitude + currentTemperatureAtSeaLevel;
        }

        /// <summary>
        /// Get the freezing altitude based on the current temperature at sea level.
        /// </summary>
        /// <returns>Freezing altitude in world space.</returns>
        public float GetFreezingAltitude()
        {
            // Reverse, x = (y - b) / m
            // Where y = freezing
            float altitude = (TemperatureData.FreezingPoint - currentTemperatureAtSeaLevel) / TemperatureData.TemperatureChangePerMeter;
            return altitude;
        }

        public class Temperature
        {
            public float low;
            public float anomaly;
            public float average;
            public float high;

            public Temperature(float typicalAverage, float maximumTemperatureAnomaly, float maximumDiurnalTemperatureVariation, int day)
            {
                average = typicalAverage;
                anomaly = (StaticHelpers.Random(day, 362) * 2.0f - 1.0f) * maximumTemperatureAnomaly;
                average += anomaly;
                low = average - StaticHelpers.Random(day, 240) * maximumDiurnalTemperatureVariation * 0.5f;
                high = average + StaticHelpers.Random(day, 757) * maximumDiurnalTemperatureVariation * 0.5f;
            }
        }

        public float GetTemperatureAtTime(float time)
        {
            if (!dynamicTemperature)
                return averageTemperatureAtSeaLevel;

            Temperature a = new Temperature(
                averageTemperatureAtSeaLevel,
                maximumTemperatureAnomaly,
                maximumDiurnalTemperatureVariation,
                SkyDefinition.SystemTimeToDay(time) - 1
            );
            Temperature b = new Temperature(
                averageTemperatureAtSeaLevel,
                maximumTemperatureAnomaly,
                maximumDiurnalTemperatureVariation,
                SkyDefinition.SystemTimeToDay(time)
            );
            Temperature c = new Temperature(
                averageTemperatureAtSeaLevel,
                maximumTemperatureAnomaly,
                maximumDiurnalTemperatureVariation,
                SkyDefinition.SystemTimeToDay(time) + 1
            );

            float t = SkyDefinition.SystemTimeToTime024(time);
            float interpolator;
            float z;

            if (t < 6)
            {
                interpolator = StaticHelpers.Remap(t, 0, 6, 0.5f, 1);
                z = StaticHelpers.Smoothstep(a.high, b.low, interpolator);
            }
            else if (t < 18)
            {
                interpolator = StaticHelpers.Remap(t, 6, 18, 0, 1);
                z = StaticHelpers.Smoothstep(b.low, b.high, interpolator);
            }
            else
            {
                interpolator = StaticHelpers.Remap(t, 18, 24, 0, 0.5f);

                z = StaticHelpers.Smoothstep(b.high, c.low, interpolator);
            }

            return z;
        }

        private OverrideState overrideState = OverrideState.Normal;

        public void OverrideTemperature(float temperature)
        {
            currentTemperatureAtSeaLevel = temperature;
            overrideState = OverrideState.Overriden;
        }

        public void ReleaseOverride()
        {
            overrideState = OverrideState.Normal;
        }
    }
}
