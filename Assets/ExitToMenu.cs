using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ExitToMenu : MonoBehaviour
{
    public Button button; // Ссылка на кнопку


    void Start()
    {
        // Добавляем слушатель событий на кнопку
        button.onClick.AddListener(OpenLink);
    }

    // Функция, которая открывает URL-адрес
    void OpenLink()
    {
        // Проверяем платформу
        SceneManager.LoadScene(0);
        // Android: Используем Application.OpenURL для открытия URL



    }
}