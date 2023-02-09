using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

[System.Serializable]

public class LabelGraph
{
 
    public bool used;
    public string name;
    public List<LabelGraph> children;
    public GameObject obj;
    public string if_cond;
    public string choose_cond;
   
    public LabelGraph(string name, List<LabelGraph> l)
    {
        this.children = l;
        this.name = name;
        
    }

};
[ExecuteInEditMode]

public class GraphComposer : MonoBehaviour
{
    [HideInInspector]public List<LabelGraph> AllLabels;
    [HideInInspector]public float spaceSizeX;
    [HideInInspector] public float spaceSizeY;
    public bool update;
   
    public GameObject labelObj;
    public Dialogue diaClass;
    private List<GameObject> Instanced;
    private float maxY =0 ;

    IEnumerator UpdateCoroutine()
    { 
        RenewGraph();
        yield return new WaitForSeconds(1);
        Print();
    }
  
    void Update()
    {
        

        if (update)
        {
            update = !update;
            StartCoroutine(UpdateCoroutine());

        }


        foreach (var label in AllLabels)
        {
            recDrawLine(label);
        }  


     
    }

    void recDrawLine(LabelGraph label)
    {
        try
        {
            foreach (var item in label.children)
            {
     
               
                Debug.DrawLine(label.obj.transform.position, item.obj.transform.position, Color.black);
                Debug.DrawLine(label.obj.transform.position+Vector3.up/50 , item.obj.transform.position+Vector3.up/50, Color.black);
                Debug.DrawLine(label.obj.transform.position+Vector3.up/45 , item.obj.transform.position+Vector3.up/45, Color.black);
                Debug.DrawLine(item.obj.transform.position, item.obj.transform.position + Vector3.up/7 + Vector3.left/10, Color.black);
                Debug.DrawLine(item.obj.transform.position, item.obj.transform.position + Vector3.down/7 - Vector3.right/10, Color.black);
                recDrawLine(item);
            }
        }
        catch (Exception e)
        {
        
        }

    }
    
    // Update is called once per frame
    LabelGraph FindLabel(string name)
    {
        foreach (var label in AllLabels)
        {
            if (label.name == name)
                return (label);
        }

        return new LabelGraph("None", new List<LabelGraph>());
    }

    public void RenewGraph()
    {
     
        AllLabels.Clear();
           maxY = 0;
           foreach (var label in diaClass.Labels)
           {
               LabelGraph lg = new LabelGraph(label.name, new List<LabelGraph>());
               bool used = false; 
               foreach (var label1 in AllLabels)
               {
                   if (label1.name == lg.name)
                   {
                       used = true;
                   } 
               }
               if(used == false)
                   AllLabels.Add(lg); 
           }

        
           foreach (var label in diaClass.Labels)
            {

                foreach (var item in label.lines)
                {
                    if (item.type == GDB.LineType.Jump)
                    {

                        FindLabel(label.name).children.Add(FindLabel(item.additionalPose));
                        FindLabel(item.additionalPose).used = true;

                    }
                    if (item.type == GDB.LineType.Menu)
                    {
                        for (int i = 0; i < item.menu_jump.Count; i++)
                        {
                            
                           
                           
                        
                            if (FindLabel(item.menu_jump[i]).choose_cond!=  item.menu_label[i])
                               FindLabel(item.menu_jump[i]).choose_cond += " " +item.menu_label[i];
                            FindLabel(item.menu_jump[i]).used = true;
                            FindLabel(label.name).children.Add(FindLabel(item.menu_jump[i]));
                          
                        }
                    }
                    if (item.type == GDB.LineType.If)
                    { 
                       
                         if (FindLabel(item.additionalPose).if_cond != item.var+"")
                             FindLabel(item.additionalPose).if_cond +=  " " + item.var + " " +item.signsIf + " " + item.value;

                         FindLabel(item.additionalPose).used = true;
                         FindLabel(label.name).children.Add(FindLabel(item.additionalPose));
                    }
                }
                
            }

     

    }
    
    void Print()
    {
        try
        {
            foreach (Transform child in gameObject.transform)
            {
                if (child.gameObject != gameObject)
                    DestroyImmediate(child.gameObject);
            }

            foreach (var instance in Instanced)
            {

                DestroyImmediate(instance);


            }

            Instanced.Clear();
        }
        catch
        {
            
        }
       
        float incr = 0;
        foreach (var label in AllLabels)
        {
            if (!label.used)
            {
                printRec(label, new Vector3(gameObject.transform.position.x, maxY + incr, gameObject.transform.position.z));
                incr += spaceSizeY;
            }
        }

       

    }

    GameObject FindInstanced(LabelGraph label, Vector3 position)
    {
        try
        {
            foreach (var obj in Instanced)
            {
                if (obj.GetComponent<SimpleLabel>().name.text == label.name)
                    return (obj);
            }

        }
        catch (Exception e)
        {
          
        }
        
        return Instantiate(labelObj, position, gameObject.transform.rotation, gameObject.transform);

    }


    void printRec(LabelGraph label, Vector3 position)
    {
        
            float incr = 0;
            GameObject parent = FindInstanced(label, position);
            label.obj = parent;
            label.obj.GetComponent<SimpleLabel>().name.text = label.name;
            label.obj.GetComponent<SimpleLabel>().choosename.text = label.choose_cond;
            label.obj.GetComponent<SimpleLabel>().ifname.text = label.if_cond;
            try
            {
                if (label.obj.GetComponent<SimpleLabel>().ifname.text.Length > 0)
                {
                    label.obj.GetComponent<SimpleLabel>().ifobj.SetActive(true);
                }
                if (label.obj.GetComponent<SimpleLabel>().choosename.text.Length > 0)
                {
                    label.obj.GetComponent<SimpleLabel>().chooseobj.SetActive(true);
                }
            }
            catch 
            {
               
            }
           
            Instanced.Add(parent);
            foreach (var item in label.children)
            {
                float ypos = parent.transform.position.y - (label.children.Count * spaceSizeY / 2) + spaceSizeY / 2 +
                             incr;
                if (maxY < ypos)
                    maxY = ypos;
                printRec(item, new Vector3(parent.transform.position.x + spaceSizeX, ypos, 0) );
                incr += spaceSizeY;
            }
        
        
    }
    
}
