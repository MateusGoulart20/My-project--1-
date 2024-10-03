using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Acerometro : MonoBehaviour {
    SerialPort serialPort;
    public GameObject alvo0, alvo1, alvo2;
    private float alvo;
    // selecao / angulos {x,y,z} / normal sensibilidade / normal {x,y,z} / memória {x,y,z} 'para variação' / operados {x,y,z} o
    //private const float o = 20;
    private int[] estado = new int[3];
    private float[] 
        referencia = new float[3],
        aceleracao = new float[3],
        norma = new float[3],
        memoria = new float[3];
    private const float speed = 0.10f, rotationSpeed = 0.46f, speed_correcao = 50.0f;
    private float module=0, horizontalInput, forwardInput, x,y,z;
    private Vector3 vet = new Vector3(0,0,0);
    private Quaternion deltaRotation, currentRotation;

    // Start is called before the first frame update
    public string portName = "COM3"; // Altere para a porta onde o ESP32 está conectado
    public int baudRate = 115200;
    void Start(){
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        estado[0] = 0;
        estado[1] = 0;
        estado[2] = 0;
    }

    // Update is called once per frame 
    void Update(){
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
        // Mover o veiculo pra frente
        alvo0.transform.Translate(Vector3.left * Time.deltaTime * speed * forwardInput);
        alvo0.transform.Translate(Vector3.forward * Time.deltaTime * speed * horizontalInput);   
        if (serialPort.IsOpen){
            try{
                string data = serialPort.ReadLine();
                string[] values = data.Split(',');
                    // Converter as strings para float
                    alvo = float.Parse(values[0]);
                if(alvo==0){
                Debug.Log("#:"+data);
                Debug.Log("@:"+values[1]+" / "+values[2]+" / "+values[3]);
                    }    aceleracao[0] = float.Parse(values[1])/1000000;
                    aceleracao[1] = float.Parse(values[2])/1000000;
                    aceleracao[2] = float.Parse(values[3])/1000000;
                    //normal = float.Parse(values[4])/100;
                    //normal *= 7;
                    norma[0] = float.Parse(values[4])/1000000;
                    norma[1] = float.Parse(values[5])/1000000;
                    norma[2] = float.Parse(values[6])/1000000;
                   

                    vet = new Vector3(aceleracao[0], aceleracao[1], aceleracao[2]);
                    x =aceleracao[0];
                    y =aceleracao[1];
                    z =aceleracao[2];

                    //Debug.Log("C-"+alvo+":Vet:"+vet);
                    switch(alvo){
                        case 0:
                            aceleracao[0]*=speed_correcao;
                            aceleracao[1]*=speed_correcao;
                            aceleracao[2]*=speed_correcao;
                            norma[0]*=10;
                            norma[1]*=10;
                            norma[2]*=10;
                            Debug.Log("A-"+alvo+" x:"+aceleracao[0]+" y:"+aceleracao[1]+" z:"+aceleracao[2]+" x:"+norma[0]+" y:"+norma[1]+" z:"+norma[2]);
                            Debug.Log("A-"+alvo+" x:"+x+" y:"+y+" z:"+z);
                            //module = Mathf.Sqrt(aceleracao[0] * aceleracao[0] + aceleracao[1] * aceleracao[1] + aceleracao[2] * aceleracao[2]) -1;
                            // Se o módulo for maior que 1, o objeto vai na direção do vetor
                            //memoria += module*4;
                            if(aceleracao[2]>0){
                                //alvo0.transform.position += aceleracao[2] * Vector3.up;
                                alvo0.transform.Translate(aceleracao[2] *speed* Vector3.up, Space.World );
                            }else{
                                //alvo0.transform.position += -aceleracao[2] * Vector3.down;
                                alvo0.transform.Translate(aceleracao[2] *(-1)*speed* Vector3.down, Space.World );
                            }
                            if(aceleracao[0]>0){
                                //alvo0.transform.position += aceleracao[0] * Vector3.forward;
                                alvo0.transform.Translate(aceleracao[0] *speed* Vector3.back, Space.World );
                            }else{
                                //alvo0.transform.position += -aceleracao[0] * Vector3.back;
                                alvo0.transform.Translate(aceleracao[0] *(-1)*speed* Vector3.forward, Space.World );
                            }
                            if(aceleracao[1]>0){
                                //alvo0.transform.position += aceleracao[1] * Vector3.right;
                                alvo0.transform.Translate(aceleracao[1] *speed* Vector3.right, Space.World );
                            }else{
                                alvo0.transform.Translate(aceleracao[1] *(-1)*speed* Vector3.left, Space.World );
                                //alvo0.transform.position += aceleracao[1] * Vector3.back;
                            }
                               

/*
                            module*=5;
                            vet = new Vector3(aceleracao[0]-memoria[0],aceleracao[1]-memoria[1],aceleracao[2]-memoria[2]);
                            memoria[0]=aceleracao[0];memoria[1]=aceleracao[1];memoria[2]=aceleracao[2];

                             Debug.Log("n:"+module);
                            if( (module>0.1)||(module<-0.1) ) {
                                if ( module > 0.0f){
                                    vet = /*memoria* */// speed * Time.deltaTime * vet;
                                //}else{ // Se o módulo for menor ou igual a 1, inverte a direção
                                  //  vet = /*memoria* */ speed * Time.deltaTime * -vet;
                               // }
                                //alvo0.transform.position += vet;
                                //memoria*=0.8f;
                                //Debug.Log("m:"+memoria);
                            //}*/
                            vet = rotationSpeed * Time.deltaTime * new Vector3(-norma[1], -norma[2], norma[0]);

                            // Multiplicando os valores do giroscópio pela velocidade e deltaTime para suavizar
                            //Vector3 gyroRotation = new Vector3(gy, -gx, gz) * rotationSpeed * Time.deltaTime;

                            // Converte a rotação em um Quaternion
                            deltaRotation = Quaternion.Euler(vet);

                            // Aplica a rotação acumulada ao objeto
                            currentRotation *= deltaRotation;

                            // Atualiza a rotação do objeto
                            transform.rotation = currentRotation;
                            // Aplicando a rotação no objeto
                            alvo0.transform.Rotate(vet);
                        break;
                        case 1:
                            // SUCESS
                            alvo1.transform.eulerAngles = new Vector3(alvo1.transform.parent.eulerAngles.x, alvo1.transform.parent.eulerAngles.y, aceleracao[2]);
                            //alvo1.transform.rotation = alvo1.transform.parent.rotation*Quaternion.Euler(vet);
                            //alvo1.transform.eulerAngles = vet;
                            //Debug.Log("M: / x:"+memoria[0]+" / y:"+memoria[1]+" /z:"+memoria[2]);
                            //ox = (norma[0]-memoria[0])*o;
                            //oy = (norma[1]-memoria[1])*o;
                            //oz = (norma[2]-memoria[2])*o; 
                            //Debug.Log("N-M / x:"+ox+" / y:"+oy+" /z:"+oz);
                            //vet = new Vector3(ox, oz, oy);
                            //alvo1.transform.Translate(vet);
                            //memoria[0]=norma[0];
                            //memoria[2]=norma[2];
                            //memoria[1]=norma[1];
                            /*
                            if(norma[0]<0){
                                alvo1.transform.Translate(Vector3.forward * Time.deltaTime *  normal * norma[0] * 100);
                            }else{
                                alvo1.transform.Translate(Vector3.back * Time.deltaTime *  normal * norma[0] * 100);
                            }
                            alvo1.transform.Translate(Vector3.right * Time.deltaTime *  normal * norma[1] * 100);
                            alvo1.transform.Translate(Vector3.up * Time.deltaTime *  normal * norma[2] * 100);*/
                        break;
                        case 2:
                            alvo2.transform.eulerAngles = new Vector3(alvo2.transform.parent.eulerAngles.x, alvo2.transform.parent.eulerAngles.y, aceleracao[2]);
                            //alvo2.transform.rotation = alvo2.transform.parent.rotation*Quaternion.Euler(vet);
                            //alvo2.transform.eulerAngles = vet;
                            //Debug.Log("D-"+alvo+":Vet:"+alvo2.transform.eulerAngles);
                        break;
                    }
                
            }
            catch (System.Exception ex){
                Debug.LogWarning(ex.Message);
            }
        }
    }

    void OnApplicationQuit(){
        if (serialPort != null && serialPort.IsOpen){
            serialPort.Close();
        }
    }
}
