using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMessageListener : MonoBehaviour {
 // Use this for initialization
 public GameObject alvo1, alvo2;
 private float alvo,ax,ay,az, bx,by,bz;
 private Vector3 vet;

 void Start () {
 }
 // Update is called once per frame
 void Update () {
 }
 // Invoked when a line of data is received from the serial device.
 void OnMessageArrived(string msg)
 {
 Debug.Log("Arrived: " + msg);


        // Separar os valores de x, y, z
        string[] values = msg.Split(',');

        if (values.Length == 4)
        {
            // Converter as strings para float
            alvo = float.Parse(values[0]);
            ax = float.Parse(values[1])/100;
            ay = float.Parse(values[2])/100;
            az = float.Parse(values[3])/100;
            Debug.Log("A-"+alvo+" / x:"+ax+" / y:"+ay+" /z:"+az);
            bx=calc(ax);by=calc(ay);bz=calc(az);
            Debug.Log("B-"+alvo+":Vet:"+vet+" / x:"+bx+" / y:"+by+" /z:"+bz);
            vet = new Vector3(bx, by, bz);
            Debug.Log("C-"+alvo+":Vet:"+vet);
            if(alvo == 1){
                alvo1.transform.eulerAngles = vet;
            }
            if (alvo == 2 ){
                alvo2.transform.eulerAngles = vet;
            }
        }
 }
//float calc(float v){return Mathf.Lerp(0.0f, 180.0f, (v + 1) / 2);}
float calc(float v){return (v + 1)*180;}

 // Invoked when a connect/disconnect event occurs. The parameter 'success'
 // will be 'true' upon connection, and 'false' upon disconnection or
 // failure to connect.
 void OnConnectionEvent(bool success)
 {
 Debug.Log(success ? "Device connected" : "Device disconnected");
 }
}