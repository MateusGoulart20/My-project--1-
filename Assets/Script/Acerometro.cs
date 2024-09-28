using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Acerometro : MonoBehaviour {
    SerialPort serialPort;
    public GameObject alvo0, alvo1, alvo2;
    private float alvo, ax, ay, az, nx, ny, nz, mx=0, my=0, mz=0, ox, oy, oz;
    // selecao / angulos {x,y,z} / normal sensibilidade / normal {x,y,z} / memória {x,y,z} 'para variação' / operados {x,y,z} o
    //private const float o = 20;
    private float speed = 20.0f, horizontalInput, forwardInput;
    private Vector3 vet = new Vector3(0,0,0);
    public float rotationSpeed = 3.00f; // Velocidade de rotação
    private Quaternion deltaRotation, currentRotation;

    // Start is called before the first frame update
    public string portName = "COM3"; // Altere para a porta onde o ESP32 está conectado
    public int baudRate = 115200;
    void Start(){
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
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

                if (values.Length == 7){
                    // Converter as strings para float
                    alvo = float.Parse(values[0]);
                    ax = float.Parse(values[1])/100;
                    ay = float.Parse(values[2])/100;
                    az = float.Parse(values[3])/100;
                    //normal = float.Parse(values[4])/100;
                    //normal *= 7;
                    nx = float.Parse(values[4])/100;
                    ny = float.Parse(values[5])/100;
                    nz = float.Parse(values[6])/100;
                    Debug.Log("A-"+alvo+" x:"+ax+" y:"+ay+" z:"+az+" x:"+nx+" y:"+ny+" z:"+nz);
                   

                    vet = new Vector3(ax, ay, az);
                    //vet.x =ax;
                    //vet.y =ay;
                    //vet.z =az;

                    Debug.Log("C-"+alvo+":Vet:"+vet);
                    switch(alvo){
                        case 0:
                            vet = new Vector3(-ny, -nz, nx) * rotationSpeed * Time.deltaTime;

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
                            alvo1.transform.eulerAngles = new Vector3(alvo1.transform.parent.eulerAngles.x, alvo1.transform.parent.eulerAngles.y, az);
                            //alvo1.transform.rotation = alvo1.transform.parent.rotation*Quaternion.Euler(vet);
                            //alvo1.transform.eulerAngles = vet;
                            //Debug.Log("M: / x:"+mx+" / y:"+my+" /z:"+mz);
                            //ox = (nx-mx)*o;
                            //oy = (ny-my)*o;
                            //oz = (nz-mz)*o; 
                            //Debug.Log("N-M / x:"+ox+" / y:"+oy+" /z:"+oz);
                            //vet = new Vector3(ox, oz, oy);
                            //alvo1.transform.Translate(vet);
                            //mx=nx;
                            //mz=nz;
                            //my=ny;
                            /*
                            if(nx<0){
                                alvo1.transform.Translate(Vector3.forward * Time.deltaTime *  normal * nx * 100);
                            }else{
                                alvo1.transform.Translate(Vector3.back * Time.deltaTime *  normal * nx * 100);
                            }
                            alvo1.transform.Translate(Vector3.right * Time.deltaTime *  normal * ny * 100);
                            alvo1.transform.Translate(Vector3.up * Time.deltaTime *  normal * nz * 100);*/
                        break;
                        case 2:
                            alvo2.transform.eulerAngles = new Vector3(alvo2.transform.parent.eulerAngles.x, alvo2.transform.parent.eulerAngles.y, az);
                            //alvo2.transform.rotation = alvo2.transform.parent.rotation*Quaternion.Euler(vet);
                            //alvo2.transform.eulerAngles = vet;
                            //Debug.Log("D-"+alvo+":Vet:"+alvo2.transform.eulerAngles);
                        break;
                    }
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
