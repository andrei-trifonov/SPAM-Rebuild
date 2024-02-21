using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[SerializeField]
public class GCLabel
{
    public int used;
    public string name;
    public string reason;
    public GameObject entity;
    public GDB.LineType lt;
}
[SerializeField]
public class GCFullLabel
{
    public GCLabel label;
    public List<GCLabel> linked = new List<GCLabel>();
}
[ExecuteInEditMode]
public class GraphComposerNew : MonoBehaviour {
    public GameObject chapterPrefab;
    public GameObject linePrefab;
    public Dialogue dialogueData;
    private List<GCFullLabel> Nodes = new List<GCFullLabel>();
    public float nodeSpacing = 2f; // Расстояние между узлами
    GameObject linkedNode;


    private void Start()
    {
        ClearDialogueGraph();
        
    }

    int FindLabelIByName(string name)
    {
        for (int i = 0; i< Nodes.Count; i ++)
        {
            GCFullLabel l = Nodes[i];
            if (l.label.name == name)
            {
                return i;
            }
        }

        return -1;
    }

    public void ClearDialogueGraph()
    {
        foreach (Transform node in gameObject.transform.GetComponentsInChildren<Transform>())
        {
            try
            {
                if (node.gameObject != gameObject)
                    DestroyImmediate(node.gameObject);
            }
            catch{}
        }
        Nodes.Clear();

    }
    public void BuildDialogueGraph() {
     

        foreach (var chapter in dialogueData.Labels) {
            GameObject chapterObject =  Instantiate(chapterPrefab,  CalculateNodePosition(chapter.name), Quaternion.identity, gameObject.transform);
            chapterObject.name = chapter.name;

            GCLabel l = new GCLabel();
            l.name = chapter.name;
            l.entity = chapterObject;
            
            GCFullLabel l2 = new GCFullLabel();
            l2.label = l;
            l2.linked = new List<GCLabel>();
            Nodes.Add(l2);
        }

        foreach (var chapter in dialogueData.Labels) {
            GCFullLabel currentNode = Nodes[FindLabelIByName(chapter.name)];

            foreach (var line in chapter.lines) {
                if (line.type == GDB.LineType.Jump || line.type == GDB.LineType.If) {
                    LabelSample linkedChapter = dialogueData.Labels.Find(ch => ch.name == line.additionalPose);
                    if (line.additionalPose != "")
                    {
                        Nodes[FindLabelIByName(linkedChapter.name)].label.lt = line.type;
                        if (line.type == GDB.LineType.Jump )
                           Nodes[FindLabelIByName(linkedChapter.name)].label.reason = currentNode.label.name;
                        if (line.type == GDB.LineType.If) 
                            Nodes[FindLabelIByName(linkedChapter.name)].label.reason = line.var.ToString() + " " +  line.signsIf.ToString() + " " + line.value.ToString();
                        currentNode.linked.Add(Nodes[FindLabelIByName(linkedChapter.name)].label);
                    }
                }
              
                if (line.type == GDB.LineType.Menu)
                {
                    int i = 0;
                    foreach (var jl in line.menu_jump) {
                        
                        LabelSample linkedChapter = dialogueData.Labels.Find(ch => ch.name == jl);
                        if (line.menu_jump[i] != "")
                        {
                            Nodes[FindLabelIByName(linkedChapter.name)].label.lt = line.type;
                            Nodes[FindLabelIByName(linkedChapter.name)].label.reason = line.menu_label[i];
                            currentNode.linked.Add(Nodes[FindLabelIByName(linkedChapter.name)].label);
                            i++;
                        }
                    }
                  
                }
            }
            
        }
        Draw();
    }

    Vector3 CalculateNodePosition(string nodeName) {

        return new Vector3(dialogueData.Labels.FindIndex(ch => ch.name == nodeName) * nodeSpacing, 0f, 0f);
    }

    void DrawLine(Vector3 start, Vector3 end) {
        GameObject line = Instantiate(linePrefab, gameObject.transform);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
    void DrawCircleLine(Vector3 start, Vector3 end)
    {
        // Вычисляем вектор от начальной точки к конечной
        Vector3 direction = end - start;
        
        // Вычисляем радиус окружности
        float radius = direction.magnitude / 2f;
        
        // Вычисляем центр окружности
        Vector3 center = start + direction / 2f;
        
        // Создаем объект линии
        GameObject line = Instantiate(linePrefab, gameObject.transform);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        
        // Задаем параметры линии
        lineRenderer.positionCount = 6;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        
        // Генерируем случайный цвет для материала линии
        Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = randomColor;
        float startAngle = Mathf.Atan2(start.y - center.y, start.x - center.x) * Mathf.Rad2Deg;
        Debug.Log(startAngle);
        lineRenderer.SetPosition(0, start);
        // Строим линию по окружности
        for (int i = 1; i < 5; i++)
        {
            float angle = (180  / 10 ) * i + startAngle;
            Debug.Log(angle);
            float x = Mathf.Cos(angle) * radius + center.x;
            float y = Mathf.Sin(angle) * radius + center.y;
            lineRenderer.SetPosition(i, new Vector3(x, y, center.z));
        }
        lineRenderer.SetPosition(5, end);
    }
    void Draw()
    {
        foreach (var node in Nodes)
        {
            SimpleLabel sl = node.label.entity.GetComponent<SimpleLabel>(); 
            sl.SetName(node.label.name);
            sl.SetMarker(node.label.lt, node.label.reason);
            
            int s = 1;
            int i = 0;
            int r = 0;

            foreach (var linkedNode in node.linked )
            {
                SimpleLabel ln = linkedNode.entity.GetComponent<SimpleLabel>();
                linkedNode.used++;
              
                if (linkedNode.used<2)
                {
                    linkedNode.entity.transform.position = new Vector3(node.label.entity.transform.position.x+nodeSpacing,
                    node.label.entity.transform.position.y + nodeSpacing*r, node.label.entity.transform.position.z);
                    DrawLine(node.label.entity.transform.position, linkedNode.entity.transform.position);
                        if (s>0)
                            i++;
                        s *= -1;
                        r = i * s;
                    
                }else
                {
                    
                    DrawCircleLine(node.label.entity.transform.position, linkedNode.entity.transform.position);
                }
                            
                            
          


            }
        }
        
                           
    }
}