using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System.IO;
//[System.Serializable]
//public class ImageAsset
//{
//	public Texture2D image;
//	public bool isLocked;
//}


// NB : this class read file like this :
/*
CATEGORY Calm, Contemplation, Dreaming, Indie, Inspiration, Introspection, Karma, Meditation, Mindfulness, Nirvana, Rebirth, Relax, Silence, Timeless, Boho
MAINIMAGEPATH Category/0_CATEGORY ICONS/calm, Category/0_CATEGORY ICONS/contemplation, Category/0_CATEGORY ICONS/dreaming, Category/0_CATEGORY ICONS/indie, Category/0_CATEGORY ICONS/inspiration, Category/0_CATEGORY ICONS/introspection, Category/0_CATEGORY ICONS/karma, Category/0_CATEGORY ICONS/meditation, Category/0_CATEGORY ICONS/mind, Category/0_CATEGORY ICONS/nirvana, Category/0_CATEGORY ICONS/rebirth, Category/0_CATEGORY ICONS/relax, Category/0_CATEGORY ICONS/silence, Category/0_CATEGORY ICONS/timeless, Category/0_CATEGORY ICONS/boho
SUBIMAGEPATH Calm1|False, Calm2|False, Calm3|False, Calm4|False, Calm5|False, Calm6|False, Calm7|False, Calm8|False, Calm9|False, Calm10|False, Contemplation1|False, Contemplation2|False, Contemplation3|False, Contemplation4|False, Contemplation5|False, Contemplation6|False, Contemplation7|False, Contemplation8|False, Contemplation9|False, Contemplation10|False, Dreaming1|False, Dreaming2|False, Dreaming3|False, Dreaming4|False, Dreaming5|False, Dreaming6|False, Dreaming7|False, Dreaming8|False, Dreaming9|False, Dreaming10|False, Indie1|False, Indie2|False, Indie3|False, Indie4|False, Indie5|False, Indie6|False, Indie7|False, Indie8|False, Indie9|False, Indie10|False, Inspiration1|False, Inspiration2|False, Inspiration3|False, Inspiration4|False, Inspiration5|False, Inspiration6|False, Inspiration7|False, Inspiration8|False, Inspiration9|False, Inspiration10|False, Introspection1|False, Introspection2|False, Introspection3|False, Introspection4|False, Introspection5|False, Introspection6|False, Introspection7|False, Introspection8|False, Introspection9|False, Introspection10|False, Karma1|False, Karma2|False, Karma3|False, Karma4|False, Karma5|False, Karma6|False, Karma7|False, Karma8|False, Karma9|False, Karma10|False, Meditation1|False, Meditation2|False, Meditation3|False, Meditation4|False, Meditation5|False, Meditation6|False, Meditation7|False, Meditation8|False, Meditation9|False, Meditation10|False, Mindfulness1|False, Mindfulness2|False, Mindfulness3|False, Mindfulness4|False, Mindfulness5|False, Mindfulness6|False, Mindfulness7|False, Mindfulness8|False, Mindfulness9|False, Mindfulness10|False, Nirvana1|False, Nirvana2|False, Nirvana3|False, Nirvana4|False, Nirvana5|False, Nirvana6|False, Nirvana7|False, Nirvana8|False, Nirvana9|False, Nirvana10|False, Rebirth1|False, Rebirth2|False, Rebirth3|False, Rebirth4|False, Rebirth5|False, Rebirth6|False, Rebirth7|False, Rebirth8|False, Rebirth9|False, Rebirth10|False, Relax1|False, Relax2|False, Relax3|False, Relax4|False, Relax5|False, Relax6|False, Relax7|False, Relax8|False, Relax9|False, Relax10|False, Silence1|False, Silence2|False, Silence3|False, Silence4|False, Silence5|False, Silence6|False, Silence7|False, Silence8|False, Silence9|False, Silence10|False, Timeless1|False, Timeless2|False, Timeless3|False, Timeless4|False, Timeless5|False, Timeless6|False, Timeless7|False, Timeless8|False, Timeless9|False, Timeless10|False, Boho1|False, Boho2|False, Boho3|False, Boho4|False, Boho5|False, Boho6|False, Boho7|False, Boho8|False, Boho9|False, Boho10|False
EDITEDSUBIMAGEPATH Calm1D|False, Calm2D|False, Calm3D|False, Calm4D|False, Calm5D|False, Calm6D|False, Calm7D|False, Calm8D|False, Calm9D|False, Calm10D|False, Contemplation1D|False, Contemplation2D|False, Contemplation3D|False, Contemplation4D|False, Contemplation5D|False, Contemplation6D|False, Contemplation7D|False, Contemplation8D|False, Contemplation9D|False, Contemplation10D|False, Dreaming1D|False, Dreaming2D|False, Dreaming3D|False, Dreaming4D|False, Dreaming5D|False, Dreaming6D|False, Dreaming7D|False, Dreaming8D|False, Dreaming9D|False, Dreaming10D|False, Indie1D|False, Indie2D|False, Indie3D|False, Indie4D|False, Indie5D|False, Indie6D|False, Indie7D|False, Indie8D|False, Indie9D|False, Indie10D|False, Inspiration1D|False, Inspiration2D|False, Inspiration3D|False, Inspiration4D|False, Inspiration5D|False, Inspiration6D|False, Inspiration7D|False, Inspiration8D|False, Inspiration9D|False, Inspiration10D|False, Introspection1D|False, Introspection2D|False, Introspection3D|False, Introspection4D|False, Introspection5D|False, Introspection6D|False, Introspection7D|False, Introspection8D|False, Introspection9D|False, Introspection10D|False, Karma1D|False, Karma2D|False, Karma3D|False, Karma4D|False, Karma5D|False, Karma6D|False, Karma7D|False, Karma8D|False, Karma9D|False, Karma10D|False, Meditation1D|False, Meditation2D|False, Meditation3D|False, Meditation4D|False, Meditation5D|False, Meditation6D|False, Meditation7D|False, Meditation8D|False, Meditation9D|False, Meditation10D|False, Mindfulness1D|False, Mindfulness2D|False, Mindfulness3D|False, Mindfulness4D|False, Mindfulness5D|False, Mindfulness6D|False, Mindfulness7D|False, Mindfulness8D|False, Mindfulness9D|False, Mindfulness10D|False, Nirvana1D|False, Nirvana2D|False, Nirvana3D|False, Nirvana4D|False, Nirvana5D|False, Nirvana6D|False, Nirvana7D|False, Nirvana8D|False, Nirvana9D|False, Nirvana10D|False, Rebirth1D|False, Rebirth2D|False, Rebirth3D|False, Rebirth4D|False, Rebirth5D|False, Rebirth6D|False, Rebirth7D|False, Rebirth8D|False, Rebirth9D|False, Rebirth10D|False, Relax1D|False, Relax2D|False, Relax3D|False, Relax4D|False, Relax5D|False, Relax6D|False, Relax7D|False, Relax8D|False, Relax9D|False, Relax10D|False, Silence1D|False, Silence2D|False, Silence3D|False, Silence4D|False, Silence5D|False, Silence6D|False, Silence7D|False, Silence8D|False, Silence9D|False, Silence10D|False, Timeless1D|False, Timeless2D|False, Timeless3D|False, Timeless4D|False, Timeless5D|False, Timeless6D|False, Timeless7D|False, Timeless8D|False, Timeless9D|False, Timeless10D|False, Boho1D|False, Boho2D|False, Boho3D|False, Boho4D|False, Boho5D|False, Boho6D|False, Boho7D|False, Boho8D|False, Boho9D|False, Boho10D|False
IMAGESINCATEGORYCOUNT 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10
IMAGEPATHINRESOURCE Category/CALM/OK_comp/calm_573, Category/CALM/OK_comp/calm_578, Category/CALM/OK_comp/calm_589, Category/CALM/OK_comp/calm_5692, Category/CALM/OK_comp/calm_OTP40N0, Category/CALM/OK_comp/calm_024comp, Category/CALM/OK_comp/calm_128, Category/CALM/OK_comp/calm_172, Category/CALM/OK_comp/calm_371, Category/CALM/OK_comp/calm_377, Category/CONTEMPLATION/OK_comp/contemplation_036comp, Category/CONTEMPLATION/OK_comp/contemplation_033comp, Category/CONTEMPLATION/OK_comp/contemplation_024comp, Category/CONTEMPLATION/OK_comp/contemplation_1, Category/CONTEMPLATION/OK_comp/contemplation_OTP41B0, Category/CONTEMPLATION/OK_comp/contemplation_ODHHUV0, Category/CONTEMPLATION/OK_comp/contemplation_353, Category/CONTEMPLATION/OK_comp/contemplation_179, Category/CONTEMPLATION/OK_comp/contemplation_49, Category/CONTEMPLATION/OK_comp/contemplation_42, Category/DREAMING/OK_comp/dreaming_1679, Category/DREAMING/OK_comp/dreaming_587, Category/DREAMING/OK_comp/dreaming_371, Category/DREAMING/OK_comp/dreaming_347, Category/DREAMING/OK_comp/dreaming_170, Category/DREAMING/OK_comp/dreaming_122, Category/DREAMING/OK_comp/dreaming_2603, Category/DREAMING/OK_comp/dreaming_2877, Category/DREAMING/OK_comp/dreaming_OBMR4W0, Category/DREAMING/OK_comp/dreaming_OVI5AO0, Category/INDIE/OK_comp/indie_011comp, Category/INDIE/OK_comp/indie_116, Category/INDIE/OK_comp/indie_150, Category/INDIE/OK_comp/indie_155, Category/INDIE/OK_comp/indie_351, Category/INDIE/OK_comp/indie_555, Category/INDIE/OK_comp/indie_578, Category/INDIE/OK_comp/indie_001comp, Category/INDIE/OK_comp/indie_002comp, Category/INDIE/OK_comp/indie_008comp, Category/INSPIRATION/OK_comp/inspir_592, Category/INSPIRATION/OK_comp/inspir_548, Category/INSPIRATION/OK_comp/inspir_128, Category/INSPIRATION/OK_comp/inspir_118, Category/INSPIRATION/OK_comp/inspir_015comp, Category/INSPIRATION/OK_comp/inspir_003comp, Category/INSPIRATION/OK_comp/inspir_OTP4180, Category/INSPIRATION/OK_comp/inspir_OTP40R0, Category/INSPIRATION/OK_comp/inspir_OTP40M0, Category/INSPIRATION/OK_comp/inspir_1633, Category/INTROSPECTION/OK_comp/introspection_032comp, Category/INTROSPECTION/OK_comp/introspection_037comp, Category/INTROSPECTION/OK_comp/introspection_46, Category/INTROSPECTION/OK_comp/introspection_99, Category/INTROSPECTION/OK_comp/introspection_184, Category/INTROSPECTION/OK_comp/introspection_584, Category/INTROSPECTION/OK_comp/introspection_OBMR7W0, Category/INTROSPECTION/OK_comp/introspection_OTP40I0, Category/INTROSPECTION/OK_comp/introspection_21, Category/INTROSPECTION/OK_comp/introspection_022comp, Category/KARMA/OK_comp/karma_355, Category/KARMA/OK_comp/karma_519, Category/KARMA/OK_comp/karma_522, Category/KARMA/OK_comp/karma_136577-OSCSZW-640, Category/KARMA/OK_comp/karma_OTP40S0, Category/KARMA/OK_comp/karma_010comp, Category/KARMA/OK_comp/karma_018comp, Category/KARMA/OK_comp/karma_027comp, Category/KARMA/OK_comp/karma_101, Category/KARMA/OK_comp/karma_180, Category/MEDITATION/OK_comp/meditation_017comp, Category/MEDITATION/OK_comp/meditation_020comp, Category/MEDITATION/OK_comp/meditation_61, Category/MEDITATION/OK_comp/meditation_018comp, Category/MEDITATION/OK_comp/meditation_99, Category/MEDITATION/OK_comp/meditation_145, Category/MEDITATION/OK_comp/meditation_175, Category/MEDITATION/OK_comp/meditation_211, Category/MEDITATION/OK_comp/meditation_344, Category/MEDITATION/OK_comp/meditation_4160, Category/MINDFULNESS/OK_comp/mind_014comp, Category/MINDFULNESS/OK_comp/mind_40, Category/MINDFULNESS/OK_comp/mind_91, Category/MINDFULNESS/OK_comp/mind_93, Category/MINDFULNESS/OK_comp/mind_174, Category/MINDFULNESS/OK_comp/mind_205, Category/MINDFULNESS/OK_comp/mind_326, Category/MINDFULNESS/OK_comp/mind_349, Category/MINDFULNESS/OK_comp/mind_360, Category/MINDFULNESS/OK_comp/mind_574, Category/NIRVANA/OK_comp/nirvana_006comp, Category/NIRVANA/OK_comp/nirvana_11, Category/NIRVANA/OK_comp/nirvana_98, Category/NIRVANA/OK_comp/nirvana_105, Category/NIRVANA/OK_comp/nirvana_111, Category/NIRVANA/OK_comp/nirvana_158, Category/NIRVANA/OK_comp/nirvana_169, Category/NIRVANA/OK_comp/nirvana_581, Category/NIRVANA/OK_comp/nirvana_1493, Category/NIRVANA/OK_comp/nirvana_OTP40Q0, Category/REBIRTH/OK_comp/rebirth_007comp, Category/REBIRTH/OK_comp/rebirth_012comp, Category/REBIRTH/OK_comp/rebirth_119, Category/REBIRTH/OK_comp/rebirth_151, Category/REBIRTH/OK_comp/rebirth_192, Category/REBIRTH/OK_comp/rebirth_520, Category/REBIRTH/OK_comp/rebirth_585, Category/REBIRTH/OK_comp/rebirth_593, Category/REBIRTH/OK_comp/rebirth_OTP40T0, Category/REBIRTH/OK_comp/rebirth_OTP40U0, Category/RELAXING/OK_comp/relax_031comp, Category/RELAXING/OK_comp/relax_107, Category/RELAXING/OK_comp/relax_201, Category/RELAXING/OK_comp/relax_350, Category/RELAXING/OK_comp/relax_375, Category/RELAXING/OK_comp/relax_376, Category/RELAXING/OK_comp/relax_524, Category/RELAXING/OK_comp/relax_583, Category/RELAXING/OK_comp/relax_ODHHUV0, Category/RELAXING/OK_comp/relax_OTP40N0, Category/SILENCE/OK_comp/silence_013comp, Category/SILENCE/OK_comp/silence_021comp, Category/SILENCE/OK_comp/silence_114, Category/SILENCE/OK_comp/silence_126, Category/SILENCE/OK_comp/silence_136, Category/SILENCE/OK_comp/silence_138, Category/SILENCE/OK_comp/silence_143, Category/SILENCE/OK_comp/silence_336, Category/SILENCE/OK_comp/silence_OVI5AM0, Category/SILENCE/OK_comp/silence_OVI5AN0, Category/TIMELESS/OK_comp/timeless_OVI5AQ0, Category/TIMELESS/OK_comp/timeless_2203, Category/TIMELESS/OK_comp/timeless_3866, Category/TIMELESS/OK_comp/timeless_2857, Category/TIMELESS/OK_comp/timeless_2036, Category/TIMELESS/OK_comp/timeless_346, Category/TIMELESS/OK_comp/timeless_345, Category/TIMELESS/OK_comp/timeless_039comp, Category/TIMELESS/OK_comp/timeless_029comp, Category/TIMELESS/OK_comp/timeless_025comp, Category/BOHO/OK_comp/boho_317, Category/BOHO/OK_comp/boho_330, Category/BOHO/OK_comp/boho_2850, Category/BOHO/OK_comp/boho_2862, Category/BOHO/OK_comp/boho_2868, Category/BOHO/OK_comp/boho_2874, Category/BOHO/OK_comp/boho_2880, Category/BOHO/OK_comp/boho_2899, Category/BOHO/OK_comp/boho_2904, Category/BOHO/OK_comp/boho_2905
*/


[System.Serializable]
public class ImagePath {
    public string imagePath;
    
    public bool isLocked;
    public ImagePath(string Path, bool lockStatus) {
        this.imagePath = Path;
        this.isLocked = lockStatus;
    }
}



public class ImagePathHolder {
    public static string[] HomeButtNames,
        HomeButtImgPaths,
        PopUpButtNames,
        UIIconImagePaths,
        SocialIconImagePaths,
        HomeButtTransColors,
        PopUpButtColors;

    public static string InspWebLink, WaterMark, gameIDAndroid, gameIDiOS;
    public static bool IAPTestMode;
    public static List<ScriptableObject> ImageCat;
    private static ImagePathHolder instanceRef;
    //	public List<Category> list.ListOfCategories;

    [HideInInspector]
    public static List<string> imageCategories, imagesInSubCategory, mainCategoryImages, SKUs, Pricing;

    [HideInInspector]
    public static List<ImagePath> imagesInCategory, editedImagesInCategory, waterMarkedImages;

    [HideInInspector]
    public static List<int> imagesInCategoryCount, imagesInSubcategoryCount;

    //	public static ImagePathHolder myInstance;
    //	public static ImagePathHolder Instance
    //	{
    //		get{
    //			if(myInstance==null)
    //				myInstance=FindObjectOfType(typeof(ImagePathHolder)) as ImagePathHolder;
    //			return myInstance;
    //		}
    //	}

    static void GetSettAsset(SettingAssets s) {
        int i = 0;
        HomeButtNames = new string[s.HomeButtNames.Length];
        HomeButtImgPaths = new string[s.HomeButtNames.Length];
        HomeButtTransColors = new string[s.HomeButtNames.Length];
        PopUpButtNames = new string[s.PopUpButtNames.Length];
        PopUpButtColors = new string[s.PopUpButtonTextColors.Length];
        UIIconImagePaths = new string[s.UIIconImages.Length];
        SocialIconImagePaths = new string[s.SocialIconImages.Length];
        WaterMark = "";

        for (i = 0; i < s.HomeButtNames.Length; i++) {
            HomeButtNames[i] = s.HomeButtNames[i];
            #if UNITY_EDITOR
            HomeButtImgPaths[i] = AssetDatabase.GetAssetPath(s.HomeButtonImages[i])
                .Substring(17, AssetDatabase.GetAssetPath(s.HomeButtonImages[i]).Length - 21);
            #endif
            HomeButtTransColors[i] = s.HomeButtonTransitionColors[i].ToString().Substring(4);
        }

        InspWebLink = s.InspirationWebLink;

        for (i = 0; i < s.PopUpButtNames.Length; i++) {
            PopUpButtNames[i] = s.PopUpButtNames[i];
            PopUpButtColors[i] = s.PopUpButtonTextColors[i].ToString().Substring(4);
        }

        for (i = 0; i < s.UIIconImages.Length; i++) {
            #if UNITY_EDITOR
            UIIconImagePaths[i] = AssetDatabase.GetAssetPath(s.UIIconImages[i])
                .Substring(17, AssetDatabase.GetAssetPath(s.UIIconImages[i]).Length - 21);
            #endif
        }

        for (i = 0; i < s.SocialIconImages.Length; i++) {
        #if UNITY_EDITOR
            SocialIconImagePaths[i] = AssetDatabase.GetAssetPath(s.SocialIconImages[i])
                .Substring(17, AssetDatabase.GetAssetPath(s.SocialIconImages[i]).Length - 21);
        #endif
        }

        #if UNITY_EDITOR
        WaterMark = AssetDatabase.GetAssetPath(s.Watermark)
            .Substring(17, AssetDatabase.GetAssetPath(s.Watermark).Length - 21);
        #endif
        SaveSettingToAsset();
    }


    public static void SaveSettings(SettingAssets settAsset) {
        GetSettAsset(settAsset);
    }


    static void SaveSettingToAsset() {
        string[] TAStr = Resources.Load<TextAsset>("AllImageData")
            .text.Split(new string[] { "?" }, System.StringSplitOptions.RemoveEmptyEntries);

        string saveStr = "HOMEBUTTONS|";
        int i = 0;

        for (i = 0; i < HomeButtNames.Length; i++)
            saveStr += HomeButtNames[i] + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "HOMEBUTTONIMAGES|";

        for (i = 0; i < HomeButtImgPaths.Length; i++)
            saveStr += HomeButtImgPaths[i] + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "HOMEBUTTONCOLORS|";

        for (i = 0; i < HomeButtTransColors.Length; i++)
            saveStr += HomeButtTransColors[i] + ":";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "INSPIRATIONLINK|" + InspWebLink + "\n" + "POPUPBUTTONS|";

        for (i = 0; i < PopUpButtNames.Length; i++)
            saveStr += PopUpButtNames[i] + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "POPUPBUTTONCOLORS|";

        for (i = 0; i < PopUpButtColors.Length; i++)
            saveStr += PopUpButtColors[i] + ":";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "UIICONIMAGES|";

        for (i = 0; i < UIIconImagePaths.Length; i++)
            saveStr += UIIconImagePaths[i] + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "SOCIALICONIMAGES|";

        for (i = 0; i < SocialIconImagePaths.Length; i++)
            saveStr += SocialIconImagePaths[i] + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "WATERMARK|";
        saveStr += WaterMark;

        #if UNITY_EDITOR
        if (Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.WindowsEditor) {
            //Write to file
            string activeDir = Application.dataPath + @"/Resources/";
            string newPath = System.IO.Path.Combine(activeDir, "AllSettings.txt");
            FileStream fs;
            fs = File.Create(newPath);
            byte[] SaveByte = System.Text.Encoding.UTF8.GetBytes(saveStr);
            fs.Write(SaveByte, 0, SaveByte.Length);
            fs.Close();
            AssetDatabase.Refresh();
        }
        #endif
    }

    public static void SaveIAP(IAPSetting iap) {
        SKUs = new List<string>();
        Pricing = new List<string>();
        for (int i = 0; i < iap.Pricing.Length; i++) {
            SKUs.Add(iap.SKUs[i]);
            Pricing.Add(iap.Pricing[i]);
        }
        IAPTestMode = iap.TestMode;
        SaveIAPToAsset();
    }

    public static void SaveAds(AdsAsset ad) {
        gameIDAndroid = ad.gameIdAndroid;
        gameIDiOS = ad.gameIdiOS;
        string saveStr = "ANDROID|";
        saveStr += gameIDAndroid;
        saveStr += "\n";
        saveStr += "IOS|" + gameIDiOS + "\n";
        saveStr += "SHOWADS|" + ad.ShowAds.ToString();
        #if UNITY_EDITOR

        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
            //Write to file
            string activeDir = Application.dataPath + @"/Resources/";
            string newPath = System.IO.Path.Combine(activeDir, "AllAds.txt");
            FileStream fs;
            fs = File.Create(newPath);
            byte[] SaveByte = System.Text.Encoding.UTF8.GetBytes(saveStr);
            fs.Write(SaveByte, 0, SaveByte.Length);
            fs.Close();
            AssetDatabase.Refresh();
        }
        #endif
    }

    public static bool GetShowAds() {
        bool category = false;
        string[] TextInAsset = Resources.Load<TextAsset>("AllAds").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        category = System.Convert.ToBoolean(TextInAsset[2].Replace("SHOWADS|", string.Empty).Trim());
        return category;
    }

    public static string GetGameIDAndroid() {
        string category = "";
        string[] TextInAsset = Resources.Load<TextAsset>("AllAds").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        category = TextInAsset[0].Replace("ANDROID|", string.Empty).Trim();
        return category;
    }

    public static string GetGameIDIOS() {
        string category = "";
        string[] TextInAsset = Resources.Load<TextAsset>("AllAds").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        category = TextInAsset[1].Replace("IOS|", string.Empty).Trim();
        return category;
    }

    static void SaveIAPToAsset() {
        string saveStr = "SKUS|";
        int i = 0;
        for (i = 0; i < SKUs.Count; i++)
            saveStr += SKUs[i] + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "PRICING|";
        for (i = 0; i < Pricing.Count; i++)
            saveStr += Pricing[i] + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";
        saveStr += "TESTMODE|" + IAPTestMode.ToString();
        #if UNITY_EDITOR
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
            //Write to file
            string activeDir = Application.dataPath + @"/Resources/";
            string newPath = System.IO.Path.Combine(activeDir, "AllIAP.txt");
            FileStream fs;
            fs = File.Create(newPath);
            byte[] SaveByte = System.Text.Encoding.UTF8.GetBytes(saveStr);
            fs.Write(SaveByte, 0, SaveByte.Length);
            fs.Close();
            AssetDatabase.Refresh();
        }
        #endif
    }


    public static List<string> GetSKUs() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllIAP").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[0].Replace("SKUS|", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            category.Add(s);

        }

        return category;
    }


    public static List<string> GetPricing() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllIAP").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[1].Replace("PRICING|", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            category.Add(s);

        }
        return category;
    }


    public static bool GetTestMode() {
        bool category = false;
        string[] TextInAsset = Resources.Load<TextAsset>("AllIAP").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        category = System.Convert.ToBoolean(TextInAsset[2].Replace("TESTMODE|", string.Empty).Trim());
        return category;
    }

    public static void SetLockedColors() {
        PlayerPrefsX.SetBool("AllColors", true);
    }

    public static bool GetLockedColors() {
        if (PlayerPrefs.HasKey("AllColors"))
            return PlayerPrefsX.GetBool("AllColors");
        return false;
    }


    public static string LoadWaterMark() {
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[8].Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries);
        return CategoryInAsset[1];
    }


    public static List<string> LoadSocialIcons() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[7].Replace("SOCIALICONIMAGES|", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            category.Add(s);

        }

        return category;
    }

    public static List<string> LoadUIIcons() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[6].Replace("UIICONIMAGES|", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            category.Add(s);

        }

        return category;
    }


    public static string LoadInspirationWebLink() {
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[3].Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries);
        return CategoryInAsset[1];
    }

    public static List<string> LoadHomeButtImages() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[1].Replace("HOMEBUTTONIMAGES|", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            category.Add(s);

        }

        return category;
    }


    public static List<string> LoadPopUpButtonNames() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[4].Replace("POPUPBUTTONS|", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset)
            category.Add(s);
        return category;
    }

    public static List<string> LoadPopUpButtonColors() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[5].Replace("POPUPBUTTONCOLORS|", string.Empty).Trim().Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset)
            category.Add(s);
        return category;
    }


    public static List<string> LoadHomeButtColors() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[2].Replace("HOMEBUTTONCOLORS|", string.Empty).Trim().Split(new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset)
            category.Add(s);
        return category;
    }


    public static List<string> LoadHomeButtNames() {
        List<string> category = new List<string>();
        string[] TextInAsset = Resources.Load<TextAsset>("AllSettings").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] CategoryInAsset = TextInAsset[0].Replace("HOMEBUTTONS|", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset)
            category.Add(s);
        return category;
    }


    public static void SaveData(CategoryList list) {
        PlayerPrefs.DeleteAll();
        //		if (!PlayerPrefsX.GetBool ("ImagesStored")) {
        GetAllPathToImages(list);
        //						SetImagePaths ();
        //					} else
        //						GetImagePaths ();

    }
    //	void Start()
    //	{
    ////		PlayerPrefs.DeleteAll ();
    //		Debug.Log (Application.dataPath);
    //		Debug.Log("Comes Here");
    ////		foreach (Category c in list.ListOfCategories) 
    ////		{
    ////
    //////			Debug.Log(AssetDatabase.GetAssetPath (c.mainImage).Length);
    //////			Debug.Log(AssetDatabase.GetAssetPath (c.mainImage).Substring(17,AssetDatabase.GetAssetPath (c.mainImage).Length-17));
    ////		}	
    //		if (!PlayerPrefsX.GetBool ("ImagesStored")) {
    //			GetAllPathToImages ();
    //			SetImagePaths ();
    //		} else
    //			GetImagePaths ();
    //		Resources.UnloadUnusedAssets ();
    //		Application.LoadLevel("Gallery");
    //
    //	}



    public static void SetImagePaths() {
        PlayerPrefsX.SetStringArray("Categories", imageCategories.ToArray());
        PlayerPrefsX.SetStringArray("MainImages", mainCategoryImages.ToArray());
        List<string> tmpImagesInCategory = new List<string>();
        List<bool> tmpImgLockStatus = new List<bool>();

        foreach (ImagePath i in imagesInCategory) {
            tmpImagesInCategory.Add(i.imagePath);
            tmpImgLockStatus.Add(i.isLocked);
        }

        PlayerPrefsX.SetIntArray("CategoryImagesCount", imagesInCategoryCount.ToArray());
        PlayerPrefsX.SetStringArray("CategoryImages", tmpImagesInCategory.ToArray());
        PlayerPrefsX.SetBoolArray("CategoryLockStatus", tmpImgLockStatus.ToArray());
        tmpImgLockStatus.Clear();
        tmpImagesInCategory.Clear();

        foreach (ImagePath i in editedImagesInCategory) {
            tmpImagesInCategory.Add(i.imagePath);
            tmpImgLockStatus.Add(i.isLocked);
        }

        PlayerPrefsX.SetStringArray("EditedCategoryImages", tmpImagesInCategory.ToArray());
        PlayerPrefsX.SetBoolArray("EditedCategoryLockStatus", tmpImgLockStatus.ToArray());
        tmpImgLockStatus.Clear();
        tmpImagesInCategory.Clear();
    }

    public static void GetImagePaths() {
        imageCategories = PlayerPrefsX.GetStringArray("Categories").ToList();
        mainCategoryImages = PlayerPrefsX.GetStringArray("MainImages").ToList();
        for (int i = 0; i < PlayerPrefsX.GetStringArray("CategoryImages").Length; i++) {
            imagesInCategory.Add(new ImagePath(PlayerPrefsX.GetStringArray("CategoryImages")[i], PlayerPrefsX.GetBoolArray("CategoryLockStatus")[i]));
            editedImagesInCategory.Add(new ImagePath(PlayerPrefsX.GetStringArray("EditedCategoryImages")[i], PlayerPrefsX.GetBoolArray("EditedCategoryLockStatus")[i]));
        }
        imagesInCategoryCount = PlayerPrefsX.GetIntArray("CategoryImagesCount").ToList();
    }

    public static void GetAllPathToImages(CategoryList list) {
        imagesInSubCategory = new List<string>();
        mainCategoryImages = new List<string>();
        imageCategories = new List<string>();
        imagesInCategory = new List<ImagePath>();
        editedImagesInCategory = new List<ImagePath>();
        waterMarkedImages = new List<ImagePath>();
        imagesInCategoryCount = new List<int>();
        imagesInSubcategoryCount = new List<int>();
        Debug.Log(list.ListOfCategories.Count());
        int CurrentIndex = 0;

        foreach (Category c in list.ListOfCategories) {
            imageCategories.Add(c.CategoryName);
            imagesInCategoryCount.Add(c.images.ListOfImageAssets.Count);
            CurrentIndex = 1;
            foreach (ImageAsset tMain in c.images.ListOfImageAssets) {
                //				DataManager.Instance.FileCreatorBytes(Resources.Load<Texture2D>(AssetDatabase.GetAssetPath (tMain.image).
                //				                                                                Substring(17,AssetDatabase.GetAssetPath (tMain.image).Length-21)).EncodeToPNG(),c.CategoryName+CurrentIndex.ToString())
                //				imagesInCategory.Add(new ImagePath(
                //					AssetDatabase.GetAssetPath (tMain.image).Substring(17,AssetDatabase.GetAssetPath (tMain.image).Length-21),tMain.isLocked));
                #if UNITY_EDITOR
                imagesInSubCategory.Add(AssetDatabase.GetAssetPath(tMain.image).Substring(17, AssetDatabase.GetAssetPath(tMain.image).Length - 21));
                #endif
                FileCreatorBytes(tMain.image.EncodeToPNG(), c.CategoryName + CurrentIndex.ToString());
                //				FileCopier(c.CategoryName+CurrentIndex.ToString(),c.CategoryName+CurrentIndex.ToString()+"D");
                FileCreatorBytes(tMain.image.EncodeToPNG(), c.CategoryName + CurrentIndex.ToString() + "D");
                //				FileCopier(c.CategoryName+CurrentIndex.ToString(),c.CategoryName+CurrentIndex.ToString()+"W");
                imagesInCategory.Add(new ImagePath(c.CategoryName + CurrentIndex.ToString(), tMain.isLocked));
                editedImagesInCategory.Add(new ImagePath(c.CategoryName + CurrentIndex.ToString() + "D", tMain.isLocked));
                //				waterMarkedImages.Add(new ImagePath(c.CategoryName+CurrentIndex.ToString()+"W",tMain.isLocked));
                //				Resources.UnloadAsset(tMain);
                CurrentIndex++;
            }

            #if UNITY_EDITOR
            mainCategoryImages.Add(AssetDatabase.GetAssetPath(c.mainImage)
                .Substring(17, AssetDatabase.GetAssetPath(c.mainImage).Length - 21));
            #endif
            //			Resources.UnloadAsset(c.mainImage);
            //			if(c.hasSubCategory)
            //			{
            ////				imagesInSubcategoryCount.Add (c.subCategoryImages.Count);
            ////				foreach(Texture2D tSub in c.subCategoryImages)
            ////				{
            ////					imagesInSubCategory.Add (AssetDatabase.GetAssetPath (tSub).Substring(17,AssetDatabase.GetAssetPath (tSub).Length-17));
            ////
            //////					Resources.UnloadAsset(tSub);
            ////				}
            //					
            //			}
        }
        //		list.ListOfCategories.Clear ();

        foreach (ImagePath iSub in imagesInCategory) {

            Debug.Log(iSub.imagePath);
        }

        PlayerPrefsX.SetBool("ImagesStored", true);
        SaveDataToAsset();
    }

    #if UNITY_ANDROID || UNITY_IOS
    public static void ForMobileDeviceOnly(List<string> subImgResPath, List<string> subImgFilePath) {
        // TODO : COMMENTED FOR DEBUG
        /*
        foreach (string s in subImgResPath)
            FileCreatorBytes(Resources.Load<Texture2D>(s).EncodeToPNG(), subImgFilePath[subImgResPath.IndexOf(s)]);
        */

        PlayerPrefsX.SetBool("ImagesStoredMobile", true);
        FileCreatorLines((Resources.Load<TextAsset>("AllImageData").text).Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries), "AllImageData");
        FileCreatorLines((Resources.Load<TextAsset>("AllSettings").text).Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries), "AllSettings");

    }
    #endif


    #if UNITY_ANDROID || UNITY_IOS
    public static void SaveFileInResources_ForMobileDeviceOnly(string imageToSave, List<string> subImgFilePath) {
        FileCreatorBytes(Resources.Load<Texture2D>(imageToSave).EncodeToPNG(), subImgFilePath[subImgFilePath.IndexOf(imageToSave)]);
    }
    #endif


    public static void UnlockCategories() {
        imagesInCategory = new List<ImagePath>();
        imagesInCategory = LoadoriginalSubImagePath();
        foreach (ImagePath iOrig in imagesInCategory)
            iOrig.isLocked = false;
        editedImagesInCategory = new List<ImagePath>();
        editedImagesInCategory = LoadSubImagePathFromAsset();
        foreach (ImagePath iEdit in editedImagesInCategory)
            iEdit.isLocked = false;
        imageCategories = new List<string>();
        imageCategories = LoadCategoryFromAsset();
        mainCategoryImages = new List<string>();
        mainCategoryImages = LoadMainImagePathFromAsset();
        imagesInCategoryCount = new List<int>();
        imagesInCategoryCount = LoadSubImageCountFromAsset();
        imagesInSubCategory = new List<string>();
        imagesInSubCategory = LoadSubImageResourcePathFromAsset();
        SaveDataToAsset();
        //		Resources.UnloadUnusedAssets ();
    }




    static void SaveDataToAsset() {
        string saveStr = "CATEGORY ";
        foreach (string sCat in imageCategories)
            saveStr += sCat + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";

        saveStr += "MAINIMAGEPATH ";
        foreach (string sMain in mainCategoryImages)
            saveStr += sMain + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";

        saveStr += "SUBIMAGEPATH ";
        foreach (ImagePath iSub in imagesInCategory) {
            saveStr += iSub.imagePath + "|" + iSub.isLocked.ToString() + ",";
            //			Debug.Log(iSub.imagePath);
        }
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";

        saveStr += "EDITEDSUBIMAGEPATH ";
        foreach (ImagePath iSubEdit in editedImagesInCategory) {
            Debug.Log(iSubEdit.isLocked.ToString());
            saveStr += iSubEdit.imagePath + "|" + iSubEdit.isLocked.ToString() + ",";
        }
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";

        saveStr += "IMAGESINCATEGORYCOUNT ";
        foreach (int i in imagesInCategoryCount)
            saveStr += i.ToString() + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);
        saveStr += "\n";

        saveStr += "IMAGEPATHINRESOURCE ";
        foreach (string s in imagesInSubCategory)
            saveStr += s + ",";
        saveStr = saveStr.Substring(0, saveStr.Length - 1);

        //		saveStr += "\n";
        //		saveStr += "WATERMARKEDIMAGES ";
        //		foreach (ImagePath iSubEdit in waterMarkedImages) 
        //		{
        //			saveStr += iSubEdit.imagePath+" "+iSubEdit.isLocked.ToString()+",";
        //		}
        //		saveStr=saveStr.Substring(0, saveStr.Length - 1);

        #if UNITY_ANDROID || UNITY_IOS
        Debug.Log("Writing Saved Data into files");
        FileCreatorLines(saveStr.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries), "AllImageData");
        #endif

        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
            //Write to file
            Debug.Log("Writing Saved Data into files in Editor");
            string activeDir = Application.dataPath + @"/Resources/";
            string newPath = System.IO.Path.Combine(activeDir, "AllImageData.txt");
            FileStream fs;
            fs = File.Create(newPath);
            byte[] SaveByte = System.Text.Encoding.UTF8.GetBytes(saveStr);
            fs.Write(SaveByte, 0, SaveByte.Length);
            fs.Close();
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
        }
    }



    public static List<string> LoadCategoryFromAsset() {
        Debug.Log("====================== LoadCategoryFromAsset ======================");

        List<string> category = new List<string>();
        string[] TextInAsset = new string[0];

        #if UNITY_EDITOR
        TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" },
            System.StringSplitOptions.RemoveEmptyEntries);
        #endif

        #if UNITY_ANDROID || UNITY_IOS
        TextInAsset = FileReaderLines("AllImageData").Split(new string[] { "\n" },
            System.StringSplitOptions.RemoveEmptyEntries);
        #endif

        string[] CategoryInAsset = TextInAsset[0].Replace("CATEGORY ", string.Empty).Trim().Split(new string[] { "," },
            System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string s in CategoryInAsset) {
            category.Add(s);
            // Debug.Log("LoadCategoryFromAsset, CATEGORY : " + s);
        }
        return category;
    }



    // Load image for each category
    public static List<string> LoadMainImagePathFromAsset() {
        Debug.Log("====================== LoadMainImagePathFromAsset ======================");

        List<string> mainImgPath = new List<string>();
        string[] TextInAsset = new string[0];
        #if UNITY_EDITOR
        TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif
        #if UNITY_ANDROID || UNITY_IOS
        TextInAsset = FileReaderLines("AllImageData").Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif

        string[] CategoryInAsset = TextInAsset[1].Replace("MAINIMAGEPATH ", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            mainImgPath.Add(s);
            // Debug.Log("LoadMainImagePathFromAsset, MAINIMAGEPATH : " + s);
        }
        return mainImgPath;
    }



    public static List<ImagePath> LoadoriginalSubImagePath() {
        Debug.Log("====================== LoadoriginalSubImagePath ======================");
        List<ImagePath> subImgPath = new List<ImagePath>();
        string[] TextInAsset = new string[0];
        #if UNITY_EDITOR
        TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif
        #if UNITY_ANDROID || UNITY_IOS
        TextInAsset = FileReaderLines("AllImageData").Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif

        string[] CategoryInAsset = TextInAsset[2].Replace("SUBIMAGEPATH ", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            subImgPath.Add(new ImagePath(s.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[0], System.Convert.ToBoolean(s.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[1])));
            // Debug.Log("LoadoriginalSubImagePath, SUBIMAGEPATH : " + s);
        }
        return subImgPath;
    }


    public static List<ImagePath> LoadSubImagePathFromAsset() {
        Debug.Log("====================== LoadSubImagePathFromAsset ======================");
        List<ImagePath> subImgPath = new List<ImagePath>();
        string[] TextInAsset = new string[0];
        #if UNITY_EDITOR
        TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif
        #if UNITY_ANDROID || UNITY_IOS
        TextInAsset = FileReaderLines("AllImageData").Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif

        string[] CategoryInAsset = TextInAsset[3].Replace("EDITEDSUBIMAGEPATH ", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            subImgPath.Add(new ImagePath(s.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[0], System.Convert.ToBoolean(s.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[1])));
            // Debug.Log("LoadSubImagePathFromAsset, EDITEDSUBIMAGEPATH : " + s);
        }
        return subImgPath;
    }


    public static List<ImagePath> LoadSubImagePathFromUnityAsset() {
        Debug.Log("====================== LoadSubImagePathFromUnityAsset ======================");
        List<ImagePath> subImgPath = new List<ImagePath>();

        string[] TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        string[] CategoryInAsset = TextInAsset[3].Replace("EDITEDSUBIMAGEPATH ", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            subImgPath.Add(new ImagePath(s.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[0], System.Convert.ToBoolean(s.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries)[1])));
            // Debug.Log(s.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)[0]+System.Convert.ToBoolean(s.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)[1]).ToString());
            // Debug.Log("LoadSubImagePathFromUnityAsset, same name !! EDITEDSUBIMAGEPATH : " + s);
        }
        return subImgPath;
    }



    //	public static List<ImagePath> LoadWaterMarkedImages()
    //	{
    //		List<ImagePath> subImgPath=new List<ImagePath>();
    //		string[] TextInAsset= Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
    //		string[] CategoryInAsset=TextInAsset[3].Replace("WATERMARKEDIMAGES ", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
    //		foreach (string s in CategoryInAsset) {
    //			subImgPath.Add (new ImagePath(s.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)[0],System.Convert.ToBoolean(s.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)[1])));
    //		}
    //		return subImgPath;
    //	}


    public static List<string> LoadSubImageResourcePathFromAsset() {
        Debug.Log("====================== LoadSubImageResourcePathFromAsset ======================");

        List<string> subImgResPath = new List<string>();
        string[] TextInAsset = new string[0];
        #if UNITY_EDITOR
        TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" },
            System.StringSplitOptions.RemoveEmptyEntries);
        #endif
        #if UNITY_ANDROID || UNITY_IOS
        TextInAsset = FileReaderLines("AllImageData").Split(new string[] { "\n" },
            System.StringSplitOptions.RemoveEmptyEntries);
        #endif

        string[] CategoryInAsset = TextInAsset[5].Replace("IMAGEPATHINRESOURCE ", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            subImgResPath.Add(s);
            // Debug.Log("LoadSubImageResourcePathFromAsset, IMAGEPATHINRESOURCE : " + s);
        }
        return subImgResPath;
    }



    public static List<string> LoadSubImageResourcePathFromUnityAsset() {
        Debug.Log("====================== LoadSubImageResourcePathFromUnityAsset ======================");

        List<string> subImgResPath = new List<string>();

        string[] TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" },
            System.StringSplitOptions.RemoveEmptyEntries);


        string[] CategoryInAsset = TextInAsset[5].Replace("IMAGEPATHINRESOURCE ",
            string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset) {
            subImgResPath.Add(s);
            // Debug.Log("LoadSubImageResourcePathFromUnityAsset, same name!! IMAGEPATHINRESOURCE : " + s);
        }
        return subImgResPath;
    }

    public static List<int> LoadSubImageCountFromAsset() {
        Debug.Log("====================== LoadSubImageCountFromAsset ======================");

        List<int> subImgCount = new List<int>();
        string[] TextInAsset = new string[0];
        #if UNITY_EDITOR
        TextInAsset = Resources.Load<TextAsset>("AllImageData").text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif
        #if UNITY_ANDROID || UNITY_IOS
        TextInAsset = FileReaderLines("AllImageData").Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        #endif

        string[] CategoryInAsset = TextInAsset[4].Replace("IMAGESINCATEGORYCOUNT ", string.Empty).Trim().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string s in CategoryInAsset)
            subImgCount.Add(int.Parse(s));
        return subImgCount;
    }


    public static void FileCreatorLines(string[] fileData, string fileName) {
        string filePath;

        Debug.Log("================ FileCreatorLines : START ===============");
        #if UNITY_ANDROID
                Debug.Log(fileName);
                filePath = Application.persistentDataPath + "/" + fileName + ".txt";
                Debug.Log("Android : Create " + fileName + " at path : " + filePath);
        #endif

        #if UNITY_IPHONE
                // TODO : was commented before: check in ios version
        		string fileNameBase = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        		filePath = fileNameBase.Substring(0, fileNameBase.LastIndexOf('/')) + "/Documents/" + fileName;
        		filePath = Application.persistentDataPath + "/"+fileName + ".txt";
                Debug.Log("IOS : Create " + fileName + " at path : " + filePath);
        #endif

        #if UNITY_EDITOR
                filePath = Application.persistentDataPath + "/" + fileName + ".txt";
                Debug.Log("Editor : Create " + fileName + " at path : " + filePath);
        #endif


        //		BinaryWriter bw;
        //		bw=File.WriteAllBytes (filePath, fileData);
        //		bw.Close ();
        //		FileStream fs;
        //		fs=File.Create(filePath);
        System.IO.File.WriteAllLines(filePath, fileData);
        //		fs.Write (System.Text.Encoding.UTF8.GetBytes (fileData), 0, System.Text.Encoding.UTF8.GetBytes (fileData).Length);
        //		fs.Close ();

        Debug.Log("================ FileCreatorLines : END ===============");



    }


    public static void FileCreatorBytes(byte[] fileData, string fileName) {
        //Create a file of specificed file-name and save byte array to it.

        string filePath;

        Debug.Log("================ FileCreatorBytes : START ===============");
        #if UNITY_ANDROID
                filePath = Application.persistentDataPath + "/" + fileName + ".txt";
                Debug.Log("Android : Create " + fileName + " at path : " + filePath);


        #elif UNITY_IPHONE && !UNITY_EDITOR
                // TODO : was commented before: check in ios version
        		string fileNameBase = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        		filePath = fileNameBase.Substring(0, fileNameBase.LastIndexOf('/')) + "/Documents/" + fileName;
        		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
                Debug.Log("IOS : Create " + fileName + " at path : " + filePath);
        #endif

        #if UNITY_EDITOR
                filePath = Application.persistentDataPath + "/" + fileName + ".txt";
                Debug.Log("Editor : Create " + fileName + " at path : " + filePath);
        #endif
        //		BinaryWriter bw;
        //		bw=File.WriteAllBytes (filePath, fileData);
        //		bw.Close ();
        FileStream fs;
        fs = File.Create(filePath);
        fs.Write(fileData, 0, fileData.Length);
        fs.Close();

        Debug.Log("================ FileCreatorBytes : START ===============");

    }


    public static void FileCopier(string fileName, string copiedFileName) {
        string filePath, copiedFilePath;
        #if UNITY_ANDROID
        filePath = Application.persistentDataPath + "/" + fileName + ".txt";
        copiedFilePath = Application.persistentDataPath + "/" + copiedFileName + ".txt";
        #elif UNITY_IPHONE && !UNITY_EDITOR
		copiedFilePath=Application.persistentDataPath + "/"+copiedFileName + ".txt";
		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #endif
        #if UNITY_EDITOR
        copiedFilePath = Application.persistentDataPath + "/" + copiedFileName + ".txt";
        filePath = Application.persistentDataPath + "/" + fileName + ".txt";
        #endif
        System.IO.File.Copy(filePath, copiedFilePath, true);
    }



    public static string FileReaderLines(string fileName) {
        //read byte array of colors from specified file-path and return.
        string imageColors = "";
        string filePath;
        #if UNITY_ANDROID
        filePath = Application.persistentDataPath + "/" + fileName + ".txt";

        #elif UNITY_IPHONE && !UNITY_EDITOR
		//	string fileNameBase = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
		//		filePath = fileNameBase.Substring(0, fileNameBase.LastIndexOf('/')) + "/Documents/" + fileName;
		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #elif UNITY_EDITOR
		filePath=Application.persistentDataPath + "/"+fileName + ".txt";
        #endif
        if (System.IO.File.Exists(filePath))
            imageColors = System.IO.File.ReadAllText(filePath);
        return imageColors;

    }


}
