//Description: CountdownAssistant: Methods used by object Couintdown in the Hierachy
using System.Collections;
using UnityEngine;


namespace TS.Generics
{
    public class CountdownAssistant : MonoBehaviour
    {
		public AnimationCurve animCurve;
		public AnimationCurve animCurve2;

		private CurrentText txtCountdownP1;
		private CurrentText txtCountdownP2;

		private RectTransform SquareP1;
		private RectTransform SquareP2;

		


		private void Start()
        {
			CanvasInGameTag canvasInGameTag = CanvasInGameTag.instance;

			SquareP1 = canvasInGameTag.objList[0].GetComponent<RectTransform>();
			txtCountdownP1 = canvasInGameTag.objList[1].GetComponent<CurrentText>();
			SquareP2 = canvasInGameTag.objList[2].GetComponent<RectTransform>();
			txtCountdownP2 = canvasInGameTag.objList[3].GetComponent<CurrentText>();

		}

        public void Step3(int PlayerNumber)
        {
			if(PlayerNumber == 0)
            {
				txtCountdownP1.DisplayTextComponent(txtCountdownP1.gameObject, LanguageManager.instance.String_ReturnText(0, 50));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP1));
            }
            else
            {
				txtCountdownP2.DisplayTextComponent(txtCountdownP2.gameObject, LanguageManager.instance.String_ReturnText(0, 50));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP2));
			}
		}

		public void Step2(int PlayerNumber)
		{
			if (PlayerNumber == 0)
			{
				txtCountdownP1.DisplayTextComponent(txtCountdownP1.gameObject, LanguageManager.instance.String_ReturnText(0, 51));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP1));
			}
			else
			{
				txtCountdownP2.DisplayTextComponent(txtCountdownP2.gameObject, LanguageManager.instance.String_ReturnText(0, 51));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP2));
			}
		}

		public void Step1(int PlayerNumber)
		{
			if (PlayerNumber == 0)
			{
				txtCountdownP1.DisplayTextComponent(txtCountdownP1.gameObject, LanguageManager.instance.String_ReturnText(0, 52));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP1));
			}
			else
			{
				txtCountdownP2.DisplayTextComponent(txtCountdownP2.gameObject, LanguageManager.instance.String_ReturnText(0, 52));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP2));
			}
		}

		public void StepGO(int PlayerNumber)
		{
			if (PlayerNumber == 0)
			{
				txtCountdownP1.DisplayTextComponent(txtCountdownP1.gameObject, LanguageManager.instance.String_ReturnText(0, 53));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP1));
			}
			else
			{
				txtCountdownP2.DisplayTextComponent(txtCountdownP2.gameObject, LanguageManager.instance.String_ReturnText(0, 53));
				StartCoroutine(StepScaleTextRoutine(txtCountdownP2));
			}
		}

		IEnumerator StepScaleTextRoutine(CurrentText txtCountdown)
		{
			float t = 0;

			while (t < 1)
			{
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
					t += Time.deltaTime;
					txtCountdown.transform.localScale = new Vector2(animCurve.Evaluate(t), animCurve.Evaluate(t));
				}
				yield return null;
			}

			txtCountdown.transform.localScale = Vector3.zero;
			yield return null;
		}


        public void SquareAnim(int PlayerNumber)
        {
			if(PlayerNumber == 0)
				StartCoroutine(SquareAnimRoutine(SquareP1));
			if (PlayerNumber == 1)
				StartCoroutine(SquareAnimRoutine(SquareP2));
		}

        IEnumerator SquareAnimRoutine(RectTransform Square)
        {
            if (!Square.gameObject.activeInHierarchy)
            {
				Square.gameObject.SetActive(true);
			}

			float t = 0;

			while (t < 1)
			{
				t += Time.deltaTime;
				Square.localScale = new Vector2(animCurve2.Evaluate(t), animCurve2.Evaluate(t));
				yield return null;
			}

			Square.localScale = Vector3.zero;
			yield return null;
        }



		public void StepCongratulation(CurrentText txtCountdown)
		{
			txtCountdown.DisplayTextComponent(txtCountdown.gameObject, LanguageManager.instance.String_ReturnText(0, 181));
			StartCoroutine(StepScaleTextRoutine(txtCountdown));
		}

		public void PlayAudioClip(int aClipID)
		{
			SoundFxManager.instance.Play(SfxList.instance.listAudioClip[aClipID], 1);
		}

		public void SquareAnim(RectTransform Square)
		{
			StartCoroutine(SquareAnimRoutine(Square));
		}
	}
}
