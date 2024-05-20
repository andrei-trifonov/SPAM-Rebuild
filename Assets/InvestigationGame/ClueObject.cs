
using UnityEngine;

public class ClueObject : MonoBehaviour
{
    public ThoughtSt Thought;
    private InvestigationController IC;
    private Vector3 startPos;
    public Transform finishPos;
    private float startTime;
    [SerializeField] private GameObject Effect;
    private bool startMove;
    private Material mat;
    
    private void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
        gameObject.GetComponent<MeshRenderer>().material = Instantiate(mat);
        IC = GameObject.FindObjectOfType<InvestigationController>();
    }

    private void OnMouseDown()
    {
        if (!startMove)
        {
            startPos = transform.position;
            startTime = Time.time;
            IC.AddThought(Thought);
            startMove = true;
            Effect.SetActive(true);
            Material            _mat = gameObject.GetComponent<MeshRenderer>().material;
            _mat.SetColor("_AlbedoColor", UnityEngine.Color.gray);
            _mat.SetColor("_OutlineColor", UnityEngine.Color.black);
            
        }
    }

    private void Update()
    {
        if (startMove)
        {
            float distance = Vector3.Distance(startPos, finishPos.position);
            float duration = distance / 4f; // distance / speed
            float t = (Time.time - startTime) / duration;
            if (t <= 1)
            {
                Vector3 newPos = CalculateParabolicPosition(startPos, finishPos.position, 3f, t); //3f is height
                Effect.transform.position = newPos;
            }

            else
            {
                startMove = false;
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