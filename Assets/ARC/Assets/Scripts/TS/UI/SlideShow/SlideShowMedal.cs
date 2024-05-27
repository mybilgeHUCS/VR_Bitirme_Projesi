// Decription: SlideShowMedal: Attached to the medal sprite in the Main menu.
// Access to medal sprites.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class SlideShowMedal : MonoBehaviour
    {
        public Image        im;
        public List<Sprite> listMedal = new List<Sprite>();
    }

}
