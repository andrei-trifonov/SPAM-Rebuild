using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LockAR : MonoBehaviour
{
    public Camera mainCamera; // ссылка на вашу главную камеру

    private int targetWidth = 1920; // желаемая ширина в пикселях (16:9)
    private int targetHeight = 1080; // желаемая высота в пикселях (16:9)

    void Start()
    {
        // Получаем текущее разрешение экрана
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        // Вычисляем соотношение сторон экрана
        float screenAspectRatio = (float)screenWidth / screenHeight;

        // Вычисляем новое разрешение камеры
        int newWidth = targetWidth;
        int newHeight = targetHeight;
        if (screenAspectRatio > 16f / 9f) // экран шире, чем 16:9
        {
            newWidth = (int)(screenHeight * (16f / 9f));
        }
        else // экран уже, чем 16:9
        {
            newHeight = (int)(screenWidth / (16f / 9f));
        }

        // Установка Rect для камеры
        mainCamera.rect = new Rect((screenWidth - newWidth) / 2f / screenWidth,
            (screenHeight - newHeight) / 2f / screenHeight,
            newWidth / (float)screenWidth,
            newHeight / (float)screenHeight);
    }
}

