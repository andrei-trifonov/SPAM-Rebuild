
using System;
using UnityEngine;

public class ClueObject : MonoBehaviour
{
    public Shader BlackMat;
    public float Height;
    public float Speed;
    public ThoughtSt Thought;
    private InvestigationController IC;
    private Vector3 startPos;
    public Transform finishPos;
    private float startTime;
    [SerializeField] private GameObject Effect;
    private bool startMove;
    private Material mat;

    [SerializeField] AudioClip clueSound;
    
    private void Start()
    {
        try
        {
            mat = gameObject.GetComponent<MeshRenderer>().material;
            gameObject.GetComponent<MeshRenderer>().material = Instantiate(mat);
        }
        catch
        {
            
        }

        IC = GameObject.FindObjectOfType<InvestigationController>();
    }

    private void OnMouseDown()
    {
        if (!startMove && !IC.inAction)
        {
            try
            {

                GetComponent<AudioSource>().PlayOneShot(clueSound);
            }
            catch
            {
            }

            startPos = transform.position;
            startTime = Time.time;
            IC.AddThought(Thought);
            IC.inAction = true;
            Effect.transform.position = startPos;
            startMove = true;
            Effect.SetActive(true);
           if (GetComponentInChildren<MeshRenderer>())
            for(int i =0; i < GetComponentInChildren<MeshRenderer>().materials.Length; i++)
            {
                GetComponentInChildren<MeshRenderer>().materials[i].shader = BlackMat;
                GetComponentInChildren<MeshRenderer>().materials[i].SetColor("_Color", Color.black);

            }
           else if (GetComponent<SpriteRenderer>())
           {
               Material _mat_s = GetComponent<SpriteRenderer>().material;
               _mat_s.SetColor("_OutlineColor", UnityEngine.Color.black);
               _mat_s.SetColor("_Color", UnityEngine.Color.black);
           }
        


        }
    }

    private void Update()
    {
        if (startMove)
        {
            float distance = Vector3.Distance(startPos, finishPos.position);
            float duration = distance / Speed; // distance / speed
            float t = (Time.time - startTime) / duration;
            if (t <= 1)
            {
                Vector3 newPos = CalculateParabolicPosition(startPos, finishPos.position, Height, t); //3f is height
                Effect.transform.position = newPos;
            }

            else
            {
                startMove = false;
                IC.inAction = false;
                Effect.SetActive(false);
                Destroy(this);
            }
        }
    }

    Vector3 CalculateParabolicPosition(Vector3 s, Vector3 f, float h, float t)
    {
        float pT = Mathf.Sin(t * Mathf.PI);
        return Vector3.Lerp(s, f, t) + Vector3.up * pT * h;
    }
}