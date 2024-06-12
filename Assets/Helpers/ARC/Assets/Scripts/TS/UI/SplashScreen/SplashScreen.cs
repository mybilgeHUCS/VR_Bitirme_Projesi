// Description: SplashScreen: Methods to Manage the scene Demo
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TS.Generics
{
    public class SplashScreen : MonoBehaviour
    {
        public RectTransform    cloudSprite;
        public float            cloudScaleSpeed = 1;

        public float            slashScreenDuration = 3;

        public int              mainMenuSceneInBuildID = 1;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(IntroRoutine());
        }

        IEnumerator IntroRoutine()
        {
            float t = 0;

            while(t < slashScreenDuration)
            {
                if (Input.anyKeyDown)
                    t = slashScreenDuration;

                t += Time.deltaTime;

                cloudSprite.localScale += new Vector3(1, 1, 1) * Time.deltaTime * cloudScaleSpeed;


                yield return null;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneInBuildID);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                cloudSprite.localScale += new Vector3(1, 1, 1) * Time.deltaTime * cloudScaleSpeed;
                yield return null;
            }

            yield return null;
        }

    }
}

