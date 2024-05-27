// Description: TS_PostProcess: Access Post Process profile and methods to modified the Post Process at runtime
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TS.Generics
{
    public class TS_PostProcess : MonoBehaviour
    {
        //-> URP
        private Volume              volume;
        private VolumeProfile       volumeProfile;
        private Vignette            vignette;
        private ChromaticAberration chromaticAberration;
        private ColorAdjustments    colorAdjustments;


        // Start is called before the first frame update
        void Start()
        {
            // Access volume component
            volume = GetComponent<Volume>();

            // Access volume profile
            volumeProfile = volume.profile;

            // Access Vignette Fx
            volumeProfile.TryGet(out vignette);

            // Access chromaticAberration Fx
            volumeProfile.TryGet(out chromaticAberration);

            // Access colorAdjustments Fx
            volumeProfile.TryGet(out colorAdjustments);


            // Modify a property
            if (vignette)
                vignette.intensity.Override(0.5f);


        }

        private float currentchromaticAberration;
        public void BoosterFxOn()
        {
            if (chromaticAberration)
            {
                currentchromaticAberration = Mathf.MoveTowards(currentchromaticAberration, 1, Time.deltaTime*2);
                chromaticAberration.intensity.Override(currentchromaticAberration);
            }
            
        }

        public void BoosterFxOff()
        {
            if (chromaticAberration)
            {
                currentchromaticAberration = Mathf.MoveTowards(currentchromaticAberration, 0, Time.deltaTime);
                chromaticAberration.intensity.Override(currentchromaticAberration);
            }
        }


        private float currentColorAdjustments;
        public void ColorAdjustmentsOn()
        {
            if (colorAdjustments && currentColorAdjustments != 100)
            {
                currentColorAdjustments = Mathf.MoveTowards(currentColorAdjustments, -10, Time.deltaTime * 50);
                colorAdjustments.saturation.Override(currentColorAdjustments);
            }
            
        }

        public void ColorAdjustmentsOff()
        {
            if (colorAdjustments && currentColorAdjustments != 0)
            {
                currentColorAdjustments = Mathf.MoveTowards(currentColorAdjustments, 0, Time.deltaTime * 50);
                colorAdjustments.saturation.Override(currentColorAdjustments);
            }
            
        }

    }
}

