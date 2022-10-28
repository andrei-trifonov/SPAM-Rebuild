using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public struct WayPoint
{
    public bool isLandingPoint;
    public Transform pointTransform;
}


[System.Serializable]
public struct WayPoints
{
    public List<WayPoint> Way;
}
public class CrowFly : MonoBehaviour
{
    public List<WayPoints> wayPoints;
    
    [SerializeField] private float Speed;
    private Transform goalPoint;
    [SerializeField] private float landDistance;
    [SerializeField] private float landAnimDistance;
    private int currentPoint;
    private int currentWay;
    private bool Blocked;
    private Animator m_Anim;
    // Start is called before the first frame update
    void Start()
    {
        goalPoint = wayPoints[0].Way[0].pointTransform;
        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    IEnumerator WaitCoroutine()
    {
        Blocked = true;
        m_Anim.SetBool("flying", false);
        m_Anim.SetBool("landing", false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(3, 15));
        Blocked = false;
    }

    void FixedUpdate()
    {
        if (!Blocked)
        {

            if ((goalPoint.position - transform.position).magnitude > landDistance)
            {
                if ((goalPoint.position - transform.position).magnitude < landAnimDistance &&
                    wayPoints[currentWay].Way[currentPoint].isLandingPoint)
                {
                    m_Anim.SetBool("flying", false);
                    m_Anim.SetBool("landing", true);
                   
                }
                else
                {
                    m_Anim.SetBool("landing", false);
                    m_Anim.SetBool("flying", true);
                }

                transform.LookAt(goalPoint);
                transform.position += Speed * (goalPoint.position - transform.position).normalized;

            }
            else
            {
                if (wayPoints[currentWay].Way[currentPoint].isLandingPoint)
                {
                    StartCoroutine(WaitCoroutine());
                }
                if (currentPoint < wayPoints[currentWay].Way.Count - 1)
                {
                    currentPoint++;
                }
                else
                {
                    currentPoint = 0;
                    currentWay = UnityEngine.Random.Range(0, wayPoints.Count);
                }

                goalPoint = wayPoints[currentWay].Way[currentPoint].pointTransform;
            }
        }
    }
}
