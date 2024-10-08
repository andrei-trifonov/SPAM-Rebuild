using UnityEngine;
using UnityEngine.UI;

public class OpenURL : MonoBehaviour
{
  public Button button; // Ссылка на кнопку
  public string url; // URL-адрес для открытия

  void Start()
  {
    // Добавляем слушатель событий на кнопку
    button.onClick.AddListener(OpenLink);
  }

  // Функция, которая открывает URL-адрес
  void OpenLink()
  {
    // Проверяем платформу
    if (Application.platform == RuntimePlatform.Android)
    {
      // Android: Используем Application.OpenURL для открытия URL
      Application.OpenURL(url);
    }
    else if (Application.platform == RuntimePlatform.IPhonePlayer)
    {
      // iOS: Используем Application.OpenURL для открытия URL
      Application.OpenURL(url);
    }
    else
    {
      // Другие платформы: Используем Process.Start для открытия URL
      Application.OpenURL(url);
    }
  }
}
