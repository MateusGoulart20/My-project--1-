using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform objetoAnterior; // Referência ao objeto anterior
    public Vector3 offset = new Vector3(0, 1, 0); // Deslocamento para manter o objeto da frente

    void Update()
    {
        // Atualiza a posição do objeto da frente
        transform.position = objetoAnterior.position +
                             objetoAnterior.forward * offset.z +
                             objetoAnterior.right * offset.x +
                             objetoAnterior.up * offset.y;
    }// Start is called before the first frame update
    void Start()
    {
        
    }
}
