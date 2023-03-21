using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor
{
    
    private Dialogue sample;
    private void OnEnable()
    {
     
        sample = (Dialogue) target;
        
    }
    

    public override void OnInspectorGUI()
    {

        

        foreach (var label in sample.Labels)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Space(50);
                EditorGUILayout.EndVertical();   
                EditorGUILayout.BeginHorizontal();
                
                    GUI.backgroundColor = Color.yellow;
                    label.name = EditorGUILayout.TextField("Название главы", label.name, GUILayout.Height(20));
                  

                    if (GUILayout.Button("Add", GUILayout.Height(20),GUILayout.Width(100)))
                    {
                        LabelSample nl = new LabelSample();
                        List<Item> items = new List<Item>();
                        items.Add(new Item());
                        nl.lines = items;
                        sample.Labels.Insert(sample.Labels.IndexOf(label)+1,  nl);
                        
                        
                        Repaint();
                    }
                    if (GUILayout.Button("Remove", GUILayout.Height(20),GUILayout.Width(100)))
                    {
                        if (sample.Labels.Count > 1)
                            sample.Labels.Remove(label);
                        Repaint();
                    }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.gray;
  
   
               

                EditorGUILayout.BeginVertical();
                    GUILayout.Space(20);
                EditorGUILayout.EndVertical();
          
                    bool color= false;
                    try
                    {
                        foreach (var item in label.lines)
                        {
                            color = !color;

                            if (color)
                                GUI.backgroundColor = Color.white;
                            else
                                GUI.backgroundColor = Color.gray;
                            EditorGUILayout.BeginVertical();
                            GUILayout.Space(10);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(50);
                            item.type = (GDB.LineType) EditorGUILayout.EnumPopup("", item.type, GUILayout.Width(80));
                            switch (item.type)
                            {

                                case (GDB.LineType.Line):
                                {

                                    item.line = EditorGUILayout.TextArea(item.line, GUILayout.Width(600),
                                        GUILayout.ExpandHeight(true));
                                    item.name = (GDB.Name) EditorGUILayout.EnumPopup("", item.name,
                                        GUILayout.Height(20), GUILayout.Width(100));
                                    EditorGUILayout.LabelField("Шрифт", GUILayout.Height(20), GUILayout.Width(40));
                                    item.font = (GDB.Fonts) EditorGUILayout.EnumPopup("", item.font,
                                        GUILayout.Height(20), GUILayout.Width(100));

                                }
                                    break;
                                case (GDB.LineType.BG):
                                {
                                    GUI.backgroundColor = new Color(1,0.5f,1);
                                    
                                    EditorGUILayout.LabelField("Фон", GUILayout.Height(20), GUILayout.Width(50));
                                    item.BGname = (GDB.BGName) EditorGUILayout.EnumPopup("", item.BGname,
                                        GUILayout.Height(20), GUILayout.Width(100));
                                    EditorGUILayout.LabelField("Эффект", GUILayout.Height(20), GUILayout.Width(50));
                                    item.effects = (GDB.Effects) EditorGUILayout.EnumPopup("", item.effects,
                                        GUILayout.Height(20), GUILayout.Width(100));
                                    GUILayout.Space(540);
                                }
                                    break;
                                case (GDB.LineType.CG):
                                {
                                    GUI.backgroundColor = new Color(1,0.5f,1);
                                    
                                    EditorGUILayout.LabelField("Фон", GUILayout.Height(20), GUILayout.Width(50));
                                    item.CGname = EditorGUILayout.TextField("", item.CGname,
                                        GUILayout.Height(20), GUILayout.Width(100));
                                    EditorGUILayout.LabelField("Эффект", GUILayout.Height(20), GUILayout.Width(50));
                                    item.effects = (GDB.Effects) EditorGUILayout.EnumPopup("", item.effects,
                                        GUILayout.Height(20), GUILayout.Width(100));
                                    EditorGUILayout.LabelField("Вкл", GUILayout.Height(20), GUILayout.Width(50));
                                    item.show = EditorGUILayout.Toggle(item.show, GUILayout.Height(20), GUILayout.Width(50));
                                  GUILayout.Space(440);
                                }
                                    break;
                                case (GDB.LineType.Actor):
                                {
                                    GUI.backgroundColor = new Color(1,0.5f,0.4f);
                                    
                                    item.name = (GDB.Name) EditorGUILayout.EnumPopup("", item.name,  GUILayout.Height(20), GUILayout.Width(50));
                                    EditorGUILayout.LabelField("Поза", GUILayout.Height(20), GUILayout.Width(50));
                                    item.pose = (GDB.Pose) EditorGUILayout.EnumPopup("", item.pose,
                                        GUILayout.Height(20), GUILayout.Width(50));
                                    item.V3position = EditorGUILayout.Vector3Field("", item.V3position,
                                           GUILayout.Height(20), GUILayout.Width(150));
                                if (item.pose == GDB.Pose.Custom)
                                    {
                                        
                                        item.additionalPose = EditorGUILayout.TextField("", item.additionalPose, GUILayout.Height(20), GUILayout.Width(50));
                                        GUILayout.Space(485);
                                    }
                                    else
                                    
                                       GUILayout.Space(540);
                                }
                                    break;
                                case (GDB.LineType.Music):
                                {
                                    GUI.backgroundColor = Color.green;
                                    
                                    EditorGUILayout.LabelField("Трек", GUILayout.Height(20), GUILayout.Width(50));
                                    item.music = (GDB.Music) EditorGUILayout.EnumPopup("", item.music,
                                        GUILayout.Height(20), GUILayout.Width(100));
                                    EditorGUILayout.LabelField("Вкл", GUILayout.Height(20), GUILayout.Width(50));
                                    item.show = EditorGUILayout.Toggle(item.show,GUILayout.Height(20), GUILayout.Width(50));
                                    GUILayout.Space(590);
                                }
                                    break;
                                case (GDB.LineType.Sound):
                                {
                                    GUI.backgroundColor = Color.green;
                                    
                                    EditorGUILayout.LabelField("Трек", GUILayout.Height(20), GUILayout.Width(50));
                                    item.additionalPose = EditorGUILayout.TextField("", item.additionalPose,
                                        GUILayout.Height(20), GUILayout.Width(100));
                                    EditorGUILayout.LabelField("Вкл", GUILayout.Height(20), GUILayout.Width(50));
                                    item.show = EditorGUILayout.Toggle(item.show,GUILayout.Height(20), GUILayout.Width(50));
                                    GUILayout.Space(590);
                                }
                                    break;
                                case (GDB.LineType.Pause):
                                {
                                    GUI.backgroundColor = Color.red;
                                    
                                    EditorGUILayout.LabelField("Задержка", GUILayout.Height(20), GUILayout.Width(50));
                                    item.time = EditorGUILayout.FloatField(item.time,GUILayout.Height(20), GUILayout.Width(50));
                                    GUILayout.Space(750);
                                }
                                    break;
                                case (GDB.LineType.Var):
                                {
                                    GUI.backgroundColor = Color.blue;
                                    
                                    EditorGUILayout.LabelField("Переменная", GUILayout.Height(20), GUILayout.Width(50));
                                    item.var = (GDB.Variables) EditorGUILayout.EnumPopup("", item.var,  GUILayout.Height(20), GUILayout.Width(100));
                                    item.signs = (GDB.Signs) EditorGUILayout.EnumPopup("", item.signs,  GUILayout.Height(20), GUILayout.Width(50));

                                    item.value = EditorGUILayout.IntField(item.value,GUILayout.Height(20), GUILayout.Width(50));
                                    GUILayout.Space(590);
                                }
                                    break;
                                case (GDB.LineType.If):
                                {
                                    GUI.backgroundColor = Color.blue;
                                    EditorGUILayout.LabelField("Переменная", GUILayout.Height(20), GUILayout.Width(50));
                                    item.var = (GDB.Variables) EditorGUILayout.EnumPopup("", item.var,  GUILayout.Height(20), GUILayout.Width(100));
                                    item.signsIf = (GDB.SignsIf) EditorGUILayout.EnumPopup("", item.signsIf,  GUILayout.Height(20), GUILayout.Width(50));
                                    item.value = EditorGUILayout.IntField(item.value,GUILayout.Height(20), GUILayout.Width(50));
                                    EditorGUILayout.LabelField("Прыжок", GUILayout.Height(20), GUILayout.Width(50));
                                    item.additionalPose = EditorGUILayout.TextField("", item.additionalPose, GUILayout.Height(20), GUILayout.Width(50));
                                   
                                    GUILayout.Space(490);
                                }
                                    break;
                                case (GDB.LineType.Jump):
                                {   
                                    GUI.backgroundColor = Color.red;
                                    EditorGUILayout.LabelField("Прыжок", GUILayout.Height(20), GUILayout.Width(50));
                                    item.additionalPose = EditorGUILayout.TextField("", item.additionalPose, GUILayout.Height(20), GUILayout.Width(50));
                                    GUILayout.Space(750);
                                }
                                    break;
                                case (GDB.LineType.Menu):
                                {
                                    GUI.backgroundColor = Color.cyan;
                                    EditorGUILayout.BeginVertical();
                                    for (int i=0; i< item.menu_label.Count; i++)
                                    {
                                       
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Выбор", GUILayout.Height(20), GUILayout.Width(50));
                                        item.menu_label[i] = EditorGUILayout.TextArea(item.menu_label[i], GUILayout.Width(300));
                                        EditorGUILayout.LabelField("Прыжок", GUILayout.Height(20), GUILayout.Width(50));
                                        item.menu_jump[i] = EditorGUILayout.TextArea(item.menu_jump[i], GUILayout.Width(300));
                                        EditorGUILayout.EndHorizontal();
                                      
                                     
                                    }
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("+", GUILayout.Height(20), GUILayout.Width(50)))
                                    {
                                        item.menu_label.Add("");
                                        item.menu_jump.Add("");

                                    }
                                    
                                    if (GUILayout.Button("-", GUILayout.Height(20), GUILayout.Width(50)))
                                    {
                                        item.menu_label.Remove(item.menu_label.Last());
                                        item.menu_jump.Remove(item.menu_jump.Last());
                                        Repaint();
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndVertical();
                                } break;

                            }

                            if (GUILayout.Button("Add", GUILayout.Height(20), GUILayout.Width(50)))
                            {
                                label.lines.Insert(label.lines.IndexOf(item) + 1, new Item());

                            }

                            if (GUILayout.Button("Remove", GUILayout.Height(20), GUILayout.Width(100)))
                            {
                                if (label.lines.Count > 1)
                                    label.lines.Remove(item);
                                Repaint();
                            }

                            EditorGUILayout.EndHorizontal();

                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                    }


            }
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button("Add", GUILayout.Height(20),GUILayout.Width(100)))
        {
            LabelSample nl = new LabelSample();
            List<Item> items = new List<Item>();
            items.Add(new Item());
            nl.lines = items;
            sample.Labels.Add(  nl);
                        
                        
            Repaint();
        }

    }
}
