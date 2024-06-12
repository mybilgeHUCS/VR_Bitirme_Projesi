using UnityEngine;


namespace TS.Generics
{
    [CreateAssetMenu(fileName = "GlobalDatas", menuName = "TS/GlobalDatas")]
    public class GlobalDatas : ScriptableObject
    {
        public bool helpBox;

        public int selectedPlatform;

        // Mobile Options
        public bool b_MobileUIEnterSelect;
        public bool b_MobilePageInitSetSelected;
        public bool b_MobilePageInSetSelected;
        public bool b_MobilePageOutSetSelected;

        // Desktop | Other Options
        public bool b_DesktopOtherUIEnterSelect;
        public bool b_DesktopOtherPageInitSetSelected;
        public bool b_DesktopOtherPageInSetSelected;
        public bool b_DesktopOtherPageOutSetSelected;

        public int mainMenuScenesInBuildID = 1;

        public bool returnPageInitSetSelectedButtonAllowed()
        {
            if (IntroInfo.instance.globalDatas.selectedPlatform == 1 &&
                 !IntroInfo.instance.globalDatas.b_MobilePageInitSetSelected

                 ||

                 IntroInfo.instance.globalDatas.selectedPlatform == 0 &&
                 !IntroInfo.instance.globalDatas.b_DesktopOtherPageInitSetSelected)
                return false;
            else
                return true;
        }

        public bool returnPageInSetSelectedButtonAllowed()
        {
            if (IntroInfo.instance.globalDatas.selectedPlatform == 1 &&
                 !IntroInfo.instance.globalDatas.b_MobilePageInSetSelected

                 ||

                 IntroInfo.instance.globalDatas.selectedPlatform == 0 &&
                 !IntroInfo.instance.globalDatas.b_DesktopOtherPageInSetSelected)
                return false;
            else
                return true;
        }

        public bool returnPageOutSetSelectedButtonAllowed()
        {
            if (IntroInfo.instance.globalDatas.selectedPlatform == 1 &&
                 !IntroInfo.instance.globalDatas.b_MobilePageOutSetSelected

                 ||

                 IntroInfo.instance.globalDatas.selectedPlatform == 0 &&
                 !IntroInfo.instance.globalDatas.b_DesktopOtherPageOutSetSelected)
                return false;
            else
                return true;
        }

        public bool returnButtonNavigationAllowed()
        {
            if (IntroInfo.instance.globalDatas.selectedPlatform == 1 &&
                 !IntroInfo.instance.globalDatas.b_MobileUIEnterSelect

                 ||

                 IntroInfo.instance.globalDatas.selectedPlatform == 0 &&
                 !IntroInfo.instance.globalDatas.b_DesktopOtherUIEnterSelect)
                return false;
            else
                return true;
        }
    }
}

