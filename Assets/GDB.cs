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
        Толя,
        Мама
       
    }
    public enum Emoji
    {
        Heartpuff,
        Heartbreak,
        Stun,
        Stars,
        SleepZ,
        Cutescull,
        Evildeath

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
        Var,
        If,
        Jump,
        CamEffect,
        Investigation,
        Emoji
    }
    public enum BGName
    {
        None,
        Park,
        Construct,
        Garages,
        Mira_house,
        Field,
        Scene,
        Hospital,
        Train,
        Rooftop,
        School,
        Classroom,
        Nightclub,
        BusStop,
        River,
        MiraRoom,
        AlinaHouse,
        AlineRoom,
        KirRoom,
        SonyaHouse,
        SonyaRoom,
        MishaHouse,
        BGTemp
    }
    public enum Fonts
    {
        Regular,
        Wave,
        Scared,
        Rainbow
      
    }
    public enum Effects
    {
        BlackOut,
        VShake,
        HShake,
        Punch,
        PointTo,
        Zoom,
        PointToAndZoom

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
        Cry,
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
        Krypton2,
        Track2,
        Track3,
        None
    }
    public enum Variables
    {
        APoints,
        KPoints,
        MPoints
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
