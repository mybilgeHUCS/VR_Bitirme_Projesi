//Description: Tools Delete All Player Prefs.
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;


namespace TS.Generics
{
	public class ToolsDeletePlayerPrefs : MonoBehaviour
	{
		// --> Init all PlayerPrefs
		[MenuItem("Tools/TS/Other/Delete all PlayerPrefs")]
		static void DeletePLayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}

		

	}
}
#endif
