using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class TargetFollow2D : MonoBehaviour
{
    public bool nearCorpse;
   
    [Header("References")]
    public Transform target;               // IK Target для руки (ладони)
    public Transform defaultTargetPosition; // Пустышка впереди — куда смотрит рука по умолчанию
    public Transform enemy;                // Цель (враг)
    public Rig armsRig;
    [Header("Settings")]
    public float maxLookAngle = 70f;       // Зона "зрения" (в градусах)

    public float moveSpeed = 8f;           // Насколько быстро target перемещается
    public float rotateSpeed = 10f;        // Насколько быстро target поворачивается
    public float aimOffsetDistance = 0.1f; // Насколько target тянется к врагу
    public PlayerController controller;
    
    [Header("Vision Zone")]
    public float nearViewWidth = 1f;     // ширина зоны у игрока (на старте)
    public float farViewWidth = 3f;      // ширина зоны на максимальной дистанции
    public float viewDistance = 5f;      // глубина зоны видимости

    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        if (controller.currentMode == PlayerController.PlayerMode.Combat && !controller.fireBlock)
        {
            if (enemy == null || defaultTargetPosition == null )
                return;

            Vector3 localPos = transform.InverseTransformPoint(enemy.position);
            bool canSeeEnemy;
// Игнорируем если враг сзади или слишком далеко
            if (localPos.z <= 0 || localPos.z > viewDistance)
            {
                canSeeEnemy = false;
            }
            else
            {
                // Интерполяция ширины от ближней к дальней
                float widthAtZ = Mathf.Lerp(nearViewWidth, farViewWidth, localPos.z / viewDistance) * 0.5f;

                // Проверка по X
                canSeeEnemy = Mathf.Abs(localPos.x) <= widthAtZ;
            }
            Vector3 targetPos;
            Quaternion targetRot;

            if (canSeeEnemy)
            {

                if (armsRig.weight < 1)
                {
                    armsRig.weight += Time.deltaTime * 4;
                }

                // Смещение от оружия в сторону врага
             
                targetPos = enemy.position;


            }
            else
            {
                if (armsRig.weight > 0)
                {
                    armsRig.weight -= Time.deltaTime * 4;
                }

                // Вернуться в дефолтное положение
                targetPos = defaultTargetPosition.position;
                targetRot = defaultTargetPosition.rotation;
            }

            // Плавное перемещение и поворот Target
            target.position = Vector3.Lerp(target.position, targetPos, Time.deltaTime * moveSpeed);
           // target.rotation = Quaternion.Slerp(target.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
      

        Gizmos.color = Color.red;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float halfNear = nearViewWidth * 0.5f;
        float halfFar = farViewWidth * 0.5f;

        // Углы ближнего основания трапеции
        Vector3 nearLeft = origin - right * halfNear;
        Vector3 nearRight = origin + right * halfNear;

        // Углы дальнего основания трапеции
        Vector3 farLeft = origin + forward * viewDistance - right * halfFar;
        Vector3 farRight = origin + forward * viewDistance + right * halfFar;

        // Рисуем трапецию
        Gizmos.DrawLine(nearLeft, nearRight);
        Gizmos.DrawLine(farLeft, farRight);
        Gizmos.DrawLine(nearLeft, farLeft);
        Gizmos.DrawLine(nearRight, farRight);
    }
}
