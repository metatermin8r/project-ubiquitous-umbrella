using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public GameObject[] trackedObjects;
    List<GameObject> radarObjects;
    public GameObject radarPrefab;
    List<GameObject> borderObjects;
    public float switchDistance;
    public Transform helpTransform;
    public float renderRange;

    // Start is called before the first frame update
    void Start()
    {
        createRadarObjects();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < radarObjects.Count; i++) {

            if (radarObjects[i] != null && borderObjects[i] != null) 
            { //this makes the enemy marker invisible when you are not with in renderRange
                if (Vector3.Distance(radarObjects[i].transform.position, transform.position) > renderRange && borderObjects[i] != null && radarObjects[i] != null)
                {
                    borderObjects[i].layer = LayerMask.NameToLayer("Invisible");
                    //Debug.Log("not visible1");
                }
                else if (Vector3.Distance(radarObjects[i].transform.position, transform.position) < renderRange && borderObjects[i] != null && radarObjects[i] != null)
                {
                    if (Vector3.Distance(radarObjects[i].transform.position, transform.position) > switchDistance && Vector3.Distance(borderObjects[i].transform.position, transform.position) < renderRange)
                    {
                        //switch to border objects
                        helpTransform.LookAt(radarObjects[i].transform);
                        borderObjects[i].transform.position = transform.position + switchDistance*helpTransform.forward;
                        borderObjects[i].layer = LayerMask.NameToLayer("Radar");
                        radarObjects[i].layer = LayerMask.NameToLayer("Invisible");
                        //Debug.Log("visible");
                    }
                    else
                    {
                        //switch baCK TO REG OBJECTS
                        borderObjects[i].layer = LayerMask.NameToLayer("Invisible");
                        radarObjects[i].layer = LayerMask.NameToLayer("Radar");
                       // Debug.Log("not visible2");
                    }
                }
                else
                {
                    borderObjects[i].layer = LayerMask.NameToLayer("Invisible");
                    //Debug.Log("not visible3");
                }
            
            }
        }

    }
    //this function generates the enemy markers at the start of play... when spawning is added it will need to be called in update differently
    void createRadarObjects()
    {
        radarObjects = new List<GameObject>();
        borderObjects = new List<GameObject>();
        

        foreach (GameObject o in trackedObjects)
        {
            GameObject k = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
            radarObjects.Add(k);
            k.transform.parent = o.transform;
            GameObject j = Instantiate(radarPrefab, o.transform.position, Quaternion.identity) as GameObject;
            borderObjects.Add(j);
            j.transform.parent = o.transform;
            Debug.Log("generated radar objects");
        }
    }
}
