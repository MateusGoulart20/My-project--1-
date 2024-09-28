using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacionaRotaciona : MonoBehaviour
{
    public GameObject manipulante;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = manipulante.transform.eulerAngles;
    }
}
