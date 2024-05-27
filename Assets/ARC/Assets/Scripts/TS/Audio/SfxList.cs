//Description: SfxList. Work in association with SfxManager.
//It is a list of AudioClip
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
	public class SfxList : MonoBehaviour
	{
		public static SfxList           instance = null;

		public bool                     SeeInspector;
		public bool                     helpBox;
		public bool                     moreOptions;

		public List<AudioClip>          listAudioClip = new List<AudioClip>();


		[System.Serializable]
		public class CustomAudioList
		{
			public string _Name;
			public List<AudioClip>      listAudioClip = new List<AudioClip>();
		}
		public List<CustomAudioList>    listRandomAudioList = new List<CustomAudioList>();

		void Awake()
		{
			#region Create only one instance of the gameObject in the Hierarchy
			//Check if instance already exists
			if (instance == null)
				//if not, set instance to this
				instance = this;

			#endregion
		}
    }
}

