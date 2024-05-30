using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextEffects : MonoBehaviour
{

    public GDB.Fonts effect;
    public TMP_Text textComponent;
    private bool redraw;
    private Animator m_Animator;
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Rainbow());
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (effect == GDB.Fonts.Computer){
        textComponent.enableAutoSizing=(true);
        }
        else{
        textComponent.fontSize = 36;
         textComponent.enableAutoSizing =(false);
        }
        if (effect == GDB.Fonts.Rainbow && redraw || effect != GDB.Fonts.Rainbow)
            textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }
            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
           
            switch (effect)
            {

                case GDB.Fonts.Wave:
                    {
                        for (int j = 0; j < 4; ++j)
                        {
                            var orig = verts[charInfo.vertexIndex + j];
                            verts[charInfo.vertexIndex + j] = orig + new Vector3(0, 3 * Mathf.Sin(Time.time * 2f + orig.x * 0.01f) + 10f, 0);
                        }
                    }
                    break;
                case GDB.Fonts.Scared:
                    {
                        float rand = Random.Range(-3.0f, 3.0f);
                        for (int j = 0; j < 4; ++j)
                        {
                            var orig = verts[charInfo.vertexIndex + j];
                            verts[charInfo.vertexIndex + j] = orig + new Vector3(0, rand, 0);
                        }
                    }
                    break;
                case GDB.Fonts.Rainbow:
                    {

                            var meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                            
                            Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
                            for (int j = 0; j < 4; ++j)
                            {
                           
                            var index = charInfo.vertexIndex + j;
                                meshInfo.colors32[index] = c;
                            }
                        

                    }
                    break;
            }



        }
        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            meshInfo.mesh.colors32 = meshInfo.colors32;
            if (effect == GDB.Fonts.Rainbow && redraw || effect != GDB.Fonts.Rainbow)
            {
                redraw = false;
                textComponent.UpdateGeometry(meshInfo.mesh, i);
            }
        }

    }
    IEnumerator Rainbow() {

        yield return new WaitForSeconds(0.1f);
        redraw = !redraw;
        StartCoroutine(Rainbow());
    }
}
   
