using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text textMeshPro;
    private string fullText;
    private int currentCharacterIndex;
    private float timePerCharacter;
    private float timer;
    [SerializeField] private GameObject Next;
    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        fullText = textMeshPro.text;
        textMeshPro.text = "";
        currentCharacterIndex = 0;
        timePerCharacter = 0.06f;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timePerCharacter)
        {
            timer -= timePerCharacter;
            textMeshPro.text += fullText[currentCharacterIndex];
            currentCharacterIndex++;

            if (currentCharacterIndex >= fullText.Length)
            {
                Next.SetActive(true);
                enabled = false;
            }
        }
    }
}