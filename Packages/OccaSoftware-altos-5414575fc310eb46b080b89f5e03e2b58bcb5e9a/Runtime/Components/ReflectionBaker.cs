using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

namespace OccaSoftware.Altos.Runtime
{
    /// <summary>
    /// This component makes it easy to keep reflections in sync with Altos' dynamic sky. </br>
    /// This component automatically re-bakes local reflections and sets environmental reflections.
    /// </summary>
    [RequireComponent(typeof(ReflectionProbe))]
    [ExecuteAlways]
    [AddComponentMenu("OccaSoftware/Altos/Reflection Baker")]
    public class ReflectionBaker : MonoBehaviour
    {
        [Header("Time")]
        [Tooltip("Set whether the reflection probe should update based on changes in the world time.")]
        public bool updateOnGameTime = true;

        [Tooltip("Set whether the reflection probe should require a reflection trigger to update based on time changes.")]
        public bool requireReflectionTriggerForTimeUpdates;

        [Tooltip("Set the maximum world time between updates. (1 = 1 hour).")]
        public float timeThreshold = 0.1f;

        [Header("Position")]
        [Tooltip("Set whether the reflection probe should update based on changes in position.")]
        public bool updateOnPositionChanged = true;

        [Tooltip("Set whether the reflection probe should require a reflection trigger to update based on position changes.")]
        public bool requireReflectionTriggerForPositionUpdates;

        [Tooltip("Set the maximum position delta between updates (1 = 1 unit)")]
        public float positionThreshold = 1f;

        [Header("Settings")]
        [Tooltip("When enabled, this component will update your global reflection cubemap in your Environment Settings tab.")]
        public bool updateEnvironmentReflections = true;

        [Tooltip(
            "The baker needs a reference to a sky director to support time delta tracking. You can provide a specific sky director. If you don't set a value here, the baker will attempt to find a sky director for you. If it can't find a sky director, then time differential baking will be disabled."
        )]
        public AltosSkyDirector altosSkyDirector;

        [Header("Debugging")]
        public bool forceRebakeNow = false;

        [HideInInspector]
        public HashSet<ITriggerReflectionBaking> ReflectionTriggers = new HashSet<ITriggerReflectionBaking>();

        float previousUpdateTime = -1;
        Vector3 previousUpdatePositionWS = Vector3.zero;
        ReflectionProbe probe;
        int renderId = -1;

        private void OnEnable()
        {
            probe = GetComponent<ReflectionProbe>();

            SetupDefaultProbeSettings();

            if (altosSkyDirector == null)
            {
                altosSkyDirector = AltosSkyDirector.Instance;
            }

            UpdateProbe();
        }

        private void LateUpdate()
        {
            if (ShouldUpdateProbe())
            {
                UpdateProbe();
            }
            UpdateEnvironmentReflections();
        }

        /// <summary>
        /// Call this method to manually initiate a probe update.
        /// </summary>
        public void UpdateProbe()
        {
            Render();
            UpdateTrackedProperties();
        }

        private void SetupDefaultProbeSettings()
        {
            probe.mode = ReflectionProbeMode.Realtime;
            probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.IndividualFaces;
        }

        private void Render()
        {
            renderId = probe.RenderProbe();
        }

        private void UpdateEnvironmentReflections()
        {
            if (!updateEnvironmentReflections)
                return;

            SetReflectionTexture();
        }

        private void SetReflectionTexture()
        {
            if (probe.realtimeTexture == null)
                return;

            probe.realtimeTexture.name = "Altos Reflection Texture";
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.customReflectionTexture = probe.realtimeTexture;
        }

        private bool IsPositionDrivenUpdate()
        {
            if (requireReflectionTriggerForPositionUpdates && ReflectionTriggers.Count <= 0)
                return false;

            if (!updateOnPositionChanged)
                return false;

            if ((transform.position - previousUpdatePositionWS).sqrMagnitude < positionThreshold * positionThreshold)
                return false;

            return true;
        }

        private bool HasSkyDefinition()
        {
            if (altosSkyDirector?.skyDefinition == null)
                return false;

            return true;
        }

        private bool IsTimeDrivenUpdate()
        {
            if (requireReflectionTriggerForTimeUpdates && ReflectionTriggers.Count <= 0)
                return false;

            if (!HasSkyDefinition())
                return false;

            float time = altosSkyDirector.skyDefinition.timeSystem;
            if (Mathf.Abs(time - previousUpdateTime) < timeThreshold)
                return false;

            return true;
        }

        private bool ShouldUpdateProbe()
        {
            if (forceRebakeNow)
            {
                forceRebakeNow = false;
                return true;
            }

            if (!probe.IsFinishedRendering(renderId))
                return false;

            if (!IsTimeDrivenUpdate() && !IsPositionDrivenUpdate())
                return false;

            return true;
        }

        private void UpdateTrackedProperties()
        {
            previousUpdatePositionWS = probe.transform.position;

            if (HasSkyDefinition())
            {
                previousUpdateTime = altosSkyDirector.skyDefinition.timeSystem;
            }
        }
    }
}
