using UnityEngine;
using System.Collections.Generic;

public class GraphComposerNew : MonoBehaviour {
    public GameObject chapterPrefab;
    public GameObject linePrefab;
    public Dialogue dialogueData;

    public float nodeSpacing = 2f; // Расстояние между узлами

    void Start() {
        BuildDialogueGraph();
    }

    void BuildDialogueGraph() {
        Dictionary<string, GameObject> createdNodes = new Dictionary<string, GameObject>();

        foreach (var chapter in dialogueData.Labels) {
            GameObject chapterObject = Instantiate(chapterPrefab, CalculateNodePosition(chapter.name), Quaternion.identity);
            chapterObject.name = chapter.name;

            createdNodes.Add(chapter.name, chapterObject);
        }

        foreach (var chapter in dialogueData.Labels) {
            GameObject currentNode = createdNodes[chapter.name];

            foreach (var line in chapter.lines) {
                if (line.type == GDB.LineType.Jump) {
                    LabelSample linkedChapter = dialogueData.Labels.Find(ch => ch.name == line.line);

                    if (linkedChapter != null) {
                        GameObject linkedNode = createdNodes[linkedChapter.name];

                        if (linkedNode != null) {
                            DrawLine(currentNode.transform.position, linkedNode.transform.position);
                        }
                    }
                }
            }
        }
    }

    Vector3 CalculateNodePosition(string nodeName) {
        // Пример логики расположения узлов по имени главы
        // Можно основываться на порядке глав или другой логике
        return new Vector3(dialogueData.Labels.FindIndex(ch => ch.name == nodeName) * nodeSpacing, 0f, 0f);
    }

    void DrawLine(Vector3 start, Vector3 end) {
        GameObject line = Instantiate(linePrefab);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}