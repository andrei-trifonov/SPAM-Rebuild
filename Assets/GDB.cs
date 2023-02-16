using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GDB
{
    
    public static Color CharColor(int i)
    {
        switch (i)
        {
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
        Алина, 
        Мира,
        Миша,
        Соня,
        Кир,
        Толя,
        Мама
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
        Jump
    }
    public enum BGName
    {
        Park,
        Construct,
        Garages,
        MiraHouse,
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
        MishaHouse
    }
    public enum Fonts
    {
        Regular,
        SMASHED,
        Scared
    }
    public enum Effects
    {
        BlackOut,
        VShake,
        HShake,
        Derealise

    }
    public enum Pose
    {
        Sad,
        Normal,
        Happy,
        Cry,
        Custom

    }
    public enum Music
    {
        Crypton2,
        Track2,
        Track3
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
