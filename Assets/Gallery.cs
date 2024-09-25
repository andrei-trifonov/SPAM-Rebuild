using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    private  List<string> cgName = new List<string>();
    [SerializeField] private string labelName; // Имя метки, которую вы хотите загрузить
    [SerializeField] private Sprite placeholderImage; // Картинка-заглушка
    [SerializeField] private string lockedImage;
    [SerializeField] private List<Button> buttons; // Список кнопок
  
    [SerializeField] private GameObject currentEnlargedImage; // Текущая увеличенная версия изображения
    private int currentPage = 0; // Текущая страница
    [SerializeField] private GameObject closeGalleryButton;
    [SerializeField] private GameObject closeImageButton;
    [SerializeField] private GameObject galleryObject;
    [SerializeField] private TMP_Text pageText;
    void Start()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("CG/");
        int spriteCount = sprites.Length - 4;
        for (int i = 0; i < sprites.Length; i++)
        {
            cgName.Add("cg" + (i + 1));
            if (PlayerPrefs.GetInt(cgName.Last()) == 0)
            {
                cgName[i] = lockedImage;
            }
        }


        UpdatePage(0);
        pageText.text = (currentPage + 1).ToString();
        
    }


    public void NextPage()
    {
        currentPage++;
        if (currentPage * buttons.Count >= cgName.Count)
        {
            currentPage = 0;
        }
        pageText.text = (currentPage + 1).ToString();
        Debug.Log("Page: " + currentPage);
        UpdatePage(currentPage);
    }

    public void PreviousPage()
    {
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = Mathf.CeilToInt((float)cgName.Count / buttons.Count ) - 1;
        }
        pageText.text = (currentPage + 1).ToString();
        Debug.Log("Page: " + currentPage);
        UpdatePage(currentPage);
    }

    private void UpdatePage(int page)
    {
       
        try
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                int spriteIndex = page * buttons.Count + i;
                //Debug.Log(spriteIndex);
                buttons[i].image.sprite = placeholderImage;
                StartCoroutine(LoadCG(cgName[spriteIndex], buttons[i]));
            }
        }
        catch
        {
        }
    }
    IEnumerator LoadCG(string name, Button button)
    {
        
        ResourceRequest request = Resources.LoadAsync<Sprite>("CG/"+ name);
                   
        while (!request.isDone)
        {
            yield return null;
        }
                   
        if (request.asset == null)
        {
            Debug.LogError("Failed to load CG at path: CG/" + name);
        }
        else
        {
            Sprite sprite = request.asset as Sprite;
            button.image.sprite = sprite;
        }

      


    }
   
      

    public void ShowEnlarged(int buttonIndex)
    {
        closeImageButton.SetActive(true);
        int spriteIndex = currentPage * buttons.Count+ buttonIndex;
        if (spriteIndex < cgName.Count)
        {
            
            currentEnlargedImage.GetComponent<Image>().sprite = buttons[buttonIndex].image.sprite;
            currentEnlargedImage.SetActive(true);
        }
    }

    public void CloseEnlarged()
    {
        closeImageButton.SetActive(false);
       currentEnlargedImage.SetActive(false);
    }


    public void SetGallery(bool state)
    {
        closeGalleryButton.SetActive(state);
        galleryObject.SetActive(state);
    }
    
}