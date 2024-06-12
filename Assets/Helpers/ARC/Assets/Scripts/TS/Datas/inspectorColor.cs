#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;

namespace TS.Generics
{
    public static class inspectorColor
    {
        // Editor Light Theme
        public static Color[] listColor = new Color[6] {
        new Color(.9f,.9f,.9f),
        new Color(.8f,.8f,.8f),
        new Color(.7f,.7f,.7f),
        new Color(.5f,.5f,.5f),
        new Color(.4f,.4f,.4f),
        Color.green };

        // Editor Dark Theme
        public static Color[] listColorDarkSkin = new Color[6] {
        new Color(.34f,.34f,.34f),
        new Color(0.33f,0.33f,0.33f),
        new Color(0.207f,0.207f,0.207f),
        new Color(0.4f,0.4f,0.4f),
        new Color(0.42f,0.42f,0.42f),
        Color.green };

        public static Color ReturnColor(int i = 0)
        {
            if (!EditorGUIUtility.isProSkin)
                return listColor[i];
            else
                return listColorDarkSkin[i];
        }
    }
}
#endif