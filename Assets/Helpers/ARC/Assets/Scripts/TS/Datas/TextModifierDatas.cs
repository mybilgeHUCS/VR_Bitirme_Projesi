using UnityEngine;
using TMPro;
namespace TS.Generics
{
    [CreateAssetMenu(fileName = "TextModifierDatas", menuName = "TS/TextModifierDatas")]
    public class TextModifierDatas : ScriptableObject
    {
        //[Header("General")]
        public int          currentSection;
        public int          currentTextType;

        //[Header("Text Modifier")]
        public Font             fontFrom;
        public TMP_FontAsset    fontTMProFrom;
        public bool             b_UseFontStyleFrom = true;
        public FontStyle        fontStyleFrom;
        public bool             b_UseFontSizeFrom = true;
        public int              fontSizeFrom;
        public bool             b_UseFontColorFrom = true;
        public Color            fontColorFrom;

        public Font             fontTo;
        public TMP_FontAsset    fontTMProTo;
        public bool             b_UseFontStyleTo = true;
        public FontStyle        fontStyleTo;
        public bool             b_UseFontSizeTo = true;
        public int              fontSizeTo;
        public bool             b_UseFontColorTo = true;
        public Color            fontColorTo;


    }
}

