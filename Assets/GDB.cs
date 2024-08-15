using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GDB
{
    
    public static Color CharColor(int i)
    {
        switch (i)
        {  
            case (int)Name.Никто: 
                return Color.clear;
           case (int)Name.Алина:
               return new Color(0.16f, 0.97f, 0.93f);
           case (int)Name.Мира:
               return new Color(0, 0.86f, 0.42f);
           case (int)Name.Миша:
               return new Color(0.84f, 0.71f, 0.36f);
           case (int)Name.Соня:
               return new Color(1, 0.86f, 0.35f);
           case (int)Name.Кир:
               return new Color(0.77f, 0.24f, 1);
            case (int)Name.Наташа:
                return new Color(1f, 0.3f, 1f);
      

        }
        return new Color(0.82f, 0.41f, 0.12f);
    }

    public enum Name
    {
        Никто,
        Алина, 
        Мира,
        Миша,
        Соня,
        Кир,
        Кот,
        Толя,
        Мама,
        Наташа,
        Женя,
        Левая,
        Правая,
        Инспектор,
        Кира, 
	Училка

    }
    public enum Emoji
    {
        Heartpuff,
        Heartbreak,
        Stun,
        Stars,
        SleepZ,
        Cutescull,
        Evildeath,
        AuraBlack,
        Lines,
        Ball

    }
    public enum LineType
    {
        Line, 
        Menu,
        BG,
        CG,
        Actor,
        Sound,
        Music,
        Pause,
        FScreen,
        Var,
        If,
        Jump,
        CamEffect,
        Investigation,
        Emoji,
        Chat,
        Event
    }
    public enum TextDisplay
    {
        Dialogue,
        Fullscreen,
        Chat
    }
    public enum BGName
    {
        None,
        Park,
        Construct,
        Garages,
        Mira_house,
        Scene,
        Tram,
        School,
        Classroom,
        Nightclub,
        Tram_stop,
        River,
        Mira_room,
        Alya_house,
        Alya_room,
        Kir_room,
        Sonya_room,
        Tower,
        BGTemp,
        Bunker,
        Hallucinacion1,
	Hallucinacion2
    }
    public enum Fonts
    {
        Regular,
        Wave,
        Scared,
        Rainbow,
        Computer
      
    }
    public enum Effects
    {
        BlackOut,
        VShake,
        HShake,
        Punch,
        PointTo,
        Zoom,
        PointToAndZoom,
        HSlide,
        VSlide

    }
    public enum Investigation
    {
        Open,
        AddThought,
        AddDrugs
    }

    public enum Pose
    {
        Hide,
        Sad,
        Normal,
        Happy,
        Angry,
        Custom

    }

    public enum SpriteEffect
    {
        Dissolve,
        Classic,
        DissolveOut

    }
    public enum Music
    {
        None,
        DvadcatDesyat,
        Bodhi,
        Bolezn21Veka,
        DiscoOkrain,
        DolgayaDorogaDomoy,
        KogdaNachinayetsaUtro,
        Krypton,
        Krypton2,
        Odinok,
        Otrajenia,
        Otsyuda,
        Paranoia,
        PechalBudetDlitsyaVechno,
        Raspadayas,
        RozdenieSverhnovoy,
        AbstynentnySindrom,
        Toshnota,
        TvoeRozdenie,
        UmiratOtTogoChotTiNeUmiraesh,
        ZavtraBudeshUbivatMenya
}
    public enum Variables
    {
        APoints,
        KPoints,
        MPoints,
        Sonya_support,
        Day1_choose_person,
        inv1,
        inv2,
        inv3,
        inv4,
        inv5,
        Cookie,
        Meds
    }

    public enum Signs
    {
        incr,
        decr,
        equal
    }
    public enum SignsIf
    {
        greater,
        less,
        equal
    }
    
   public struct chose
   {
       public string variant;
       public string jump;
   }
}
