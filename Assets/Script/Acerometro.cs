using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Acerometro : MonoBehaviour
{
  // objeto controlador do serial
  SerialPort serialPort;

  // controle de objetos
  public GameObject alvo0, alvo1, alvo2;
  private float alvo;
  // vetores [3] para = {x,y,z}
  // estado de controle para cada eixo
  private int[] estado = new int[3];
  private float[]
      // grupo 1 de aceleracao
      aceleracao = new float[3],
      //   grupo dois para normalizacao
      // ou rotacao
      norma = new float[3];
  // constantes utilizadas
  private const float
      speed = 0.10f,
      rotationSpeed = 0.46f,
      speed_correcao = 50.0f;
  // valores internos
  private float horizontalInput, forwardInput, x, y, z;
  // Vetor para uso geral
  private Vector3 vet = new Vector3(0, 0, 0);
  // Quaternion para manipulações
  private Quaternion deltaRotation, currentRotation;
  // Altere para a porta onde o ESP32 está conectado
  // Essa porta pode variar
  public string portName = "COM3";
  public const int
      // taxa de transmissao da banda
      baudRate = 115200,
      // valor corretivo do acelerometro
      corretivo = 1000000;
  void Start()  {
    serialPort = new SerialPort(portName, baudRate);
    serialPort.Open();
    estado[0] = 0;
    estado[1] = 0;
    estado[2] = 0;
  }

  // Update is called once per frame 
  void Update()  {
    // Controle corretivos 
    horizontalInput = Input.GetAxis("Horizontal");
    forwardInput = Input.GetAxis("Vertical");
    // Movimentação corretiva
    alvo0.transform.Translate(
      forwardInput * speed 
      * Time.deltaTime * Vector3.left);
    alvo0.transform.Translate(
      horizontalInput * speed 
      * Time.deltaTime * Vector3.forward);
    if (serialPort.IsOpen)  {
      try  {
        string data = serialPort.ReadLine();
        string[] values = data.Split(',');
        // Converter as strings para float
        alvo = float.Parse(values[0]);
        if (alvo == 0)  {
          Debug.Log("#:" + data);
          Debug.Log("@:" + values[1] + " / " + values[2] + " / " + values[3]);
        }
        aceleracao[0] = float.Parse(values[1]) / corretivo;
        aceleracao[1] = float.Parse(values[2]) / corretivo;
        aceleracao[2] = float.Parse(values[3]) / corretivo;

        norma[0] = float.Parse(values[4]) / corretivo;
        norma[1] = float.Parse(values[5]) / corretivo;
        norma[2] = float.Parse(values[6]) / corretivo;


        vet = new Vector3(aceleracao[0], aceleracao[1], aceleracao[2]);
        x = aceleracao[0];
        y = aceleracao[1];
        z = aceleracao[2];

        switch (alvo)
        {
          case 0:
            aceleracao[0] *= speed_correcao;
            aceleracao[1] *= speed_correcao;
            aceleracao[2] *= speed_correcao;
            norma[0] *= 10;
            norma[1] *= 10;
            norma[2] *= 10;
            Debug.Log("A-" + alvo + " x:"
              + aceleracao[0] + " y:" + aceleracao[1]
              + " z:" + aceleracao[2] + " x:" + norma[0]
              + " y:" + norma[1] + " z:" + norma[2]);
            Debug.Log("A-" + alvo + " x:" + x + " y:" + y + " z:" + z);
            if (aceleracao[2] > 0)  {
              alvo0.transform.Translate(
                aceleracao[2] * speed 
                * Vector3.up, Space.World);
            } else  {
              alvo0.transform.Translate(
                aceleracao[2] * (-1) * speed 
                * Vector3.down, Space.World);
            }
            if (aceleracao[0] > 0)  {
              alvo0.transform.Translate(
                aceleracao[0] * speed 
                * Vector3.back, Space.World);
            } else  {
              alvo0.transform.Translate(
                aceleracao[0] * (-1) * speed 
                * Vector3.forward, Space.World);
            }
            if (aceleracao[1] > 0)  {
              alvo0.transform.Translate(
                aceleracao[1] * speed 
                * Vector3.right, Space.World);
            } else  {
              alvo0.transform.Translate(
                aceleracao[1] * (-1) * speed 
                * Vector3.left, Space.World);
            }

            // criar um vetor rotacional
            vet = rotationSpeed * Time.deltaTime 
              * new Vector3(-norma[1], -norma[2], norma[0]);

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
            alvo1.transform.eulerAngles = new Vector3(
              alvo1.transform.parent.eulerAngles.x,
              alvo1.transform.parent.eulerAngles.y,
              aceleracao[2]
            );
            break;
          case 2:
            alvo2.transform.eulerAngles = new Vector3(
              alvo2.transform.parent.eulerAngles.x,
              alvo2.transform.parent.eulerAngles.y, 
              aceleracao[2]
            );
            break;
        }

      }
      catch (System.Exception ex)
      {
        Debug.LogWarning(ex.Message);
      }
    }
  }

  void OnApplicationQuit()
  {
    if (serialPort != null && serialPort.IsOpen)
    {
      serialPort.Close();
    }
  }
}
