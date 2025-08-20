using System.Collections.Generic;
using UnityEngine;

public class EnemyScaner : MonoBehaviour
{
    public PlayerController PC;
    public TargetFollow2D TF;

    private List<Collider> Enemies = new List<Collider>();
    private List<Collider> Corpses = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Corpse"))
        {
            Corpses.Add(other);
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemies.Add(other);
        }

        UpdateTarget();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Corpse"))
        {
            Corpses.Remove(other);
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemies.Remove(other);
        }

        UpdateTarget();
    }

    private void UpdateTarget()
    {
        float dE = Mathf.Infinity;
        Collider nearestEnemy = null;

        foreach (var enemy in Enemies)
        {
            if (enemy != null)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < dE)
                {
                    dE = dist;
                    nearestEnemy = enemy;
                }
            }
        }

        float dC = Mathf.Infinity;
        Collider nearestCorpse = null;

        foreach (var corpse in Corpses)
        {
            if (corpse != null)
            {
                float dist = Vector3.Distance(transform.position, corpse.transform.position);
                if (dist < dC)
                {
                    dC = dist;
                    nearestCorpse = corpse;
                }
            }
        }
    Debug.Log(dC + " " + dE);
        if (nearestCorpse != null && dC < dE)
        {
            PC.nearCorpse = true;
            TF.nearCorpse = true;
            TF.enemy = nearestCorpse.transform;
        }
        else if (nearestEnemy != null)
        {
            PC.nearCorpse = false;
            TF.nearCorpse = false;
            TF.enemy = nearestEnemy.transform;
        }
        else
        {
            // Никого рядом нет
            PC.nearCorpse = false;
            TF.nearCorpse = false;
            TF.enemy = null;
        }
    }
}
