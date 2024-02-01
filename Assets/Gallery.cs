using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class Gallery : MonoBehaviour
{
    private  List<string> cgName = new List<string>();
    [SerializeField] private string labelName; // Имя метки, которую вы хотите загрузить
    [SerializeField] private Sprite placeholderImage; // Картинка-заглушка
    [SerializeField] private List<Button> buttons; // Список кнопок
  
    [SerializeField] private GameObject currentEnlargedImage; // Текущая увеличенная версия изображения
    private int currentPage = 0; // Текущая страница
    [SerializeField] private GameObject closeGalleryButton;
    [SerializeField] private GameObject closeImageButton;
    [SerializeField] private GameObject galleryObject;
    void Start()
    {
        StartCoroutine(AsyncResourceLoad());
       
    }

    IEnumerator AsyncResourceLoad()
    {
       
        var handle =  Addressables.LoadResourceLocationsAsync(labelName);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var item in handle.Result)
            {
                if (!cgName.Contains(item.PrimaryKey))
                    cgName.Add(item.PrimaryKey);
            }
        }
        UpdatePage(0);
    }
    

    public void NextPage()
    {
        currentPage++;
        if (currentPage * buttons.Count >= cgName.Count)
        {
            currentPage = 0;
        }
        UpdatePage(currentPage);
    }

    public void PreviousPage()
    {
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = Mathf.CeilToInt((float)cgName.Count / buttons.Count ) - 1;
        }
        UpdatePage(currentPage);
    }

    private void UpdatePage(int page)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int spriteIndex = page * buttons.Count  + i;
            Debug.Log(spriteIndex);
            if (spriteIndex < cgName.Count)
            {
                StartCoroutine(LoadCG(cgName[spriteIndex], buttons[i]));
            }
            else
            {
                buttons[i].image.sprite = placeholderImage;
            }
        }
    }
    IEnumerator LoadCG(string name, Button button)
    {
        Debug.Log(name + " "  + button.name);
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(name);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            button.image.sprite = handle.Result;
        }
        Addressables.Release(handle);
      


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