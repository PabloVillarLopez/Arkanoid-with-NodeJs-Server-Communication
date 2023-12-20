using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using TMPro;

public class ClienteWebSocket : MonoBehaviour
{

    WebSocketSharp.WebSocket ws;
    //public Vector3 posicionRecibida;
    public bool nuevoLadrilloEnviado = false;
    //public GameManager gameManager;

    [SerializeField]
    public Mensaje misDatos = new Mensaje();

    public GameObject[] cubes;
    public int playerId;

    public Button boton;
    private string url;
    public TMP_InputField urlInput;

    // Start is called before the first frame update
    void Start()
    {
        misDatos = new Mensaje();
        misDatos.id = 1;
        //misDatos.vidas = 5;
        


        ws = new WebSocket(url);
        ws.OnMessage += Ws_OnMessage;

        StartCoroutine("sincronizar");
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        misDatos = JsonUtility.FromJson<Mensaje>(e.Data);
        nuevoLadrilloEnviado = true;
    }

    // Update is called once per frame
    void Update()
    {
        //misDatos.posicion = transform.position;

        if (nuevoLadrilloEnviado)
        {
            nuevoLadrilloEnviado = false;
            //transform.position = misDatos.posicion;
        }
    }

    IEnumerator sincronizar() { 
        //if (Input.GetKeyDown(KeyCode.Space))
        while(true)
        { 
            if (ws.IsAlive)
            {
                ws.Send(JsonUtility.ToJson(misDatos));            
            }
            yield return new WaitForSeconds(0.5f);
        }

    }

    private void HandleMessage(string message)
    {
        Mensaje receivedMessage = JsonUtility.FromJson<Mensaje>(message);

        if (receivedMessage != null)
        {
            if (receivedMessage.id > 0)
            {
                playerId = receivedMessage.id;
                Debug.Log("ID del jugador establecido: " + playerId);

                ActivateAlternatedBlocks();
            }

            if (receivedMessage.ladrillo > 0 && receivedMessage.ladrillo < cubes.Length)
            {
                /*if ((receivedMessage.ladrillo + receivedMessage.id) % 2 == 0) //jugador id que ha matado el bloque
                {
                    cubes[receivedMessage.ladrillo].SetActive(false);
                }
                else if((receivedMessage.ladrillo + receivedMessage.id) % 2 != 0) //jugador id que le tiene que aparecer el bloque
                {
                    cubes[receivedMessage.ladrillo].SetActive(true);
                }*/

                //O
                if (receivedMessage.id == playerId)
                {
                    cubes[receivedMessage.ladrillo].SetActive(false);
                }
                
            }

            if (receivedMessage.ganado)
            {
                //Jugador ha ganado
            }

            if (receivedMessage.perdido)
            {
                //Jugador ha perdido
            }
        }
        else
        {
            Debug.Log("Error al parsear");
        }
    }

    private void SendMessage(Mensaje message)
    {


        string messageJSON = JsonUtility.ToJson(message);
        ws.Send(messageJSON);
    }

    public void HitCube(int cubeIndex)
    {
        SendMessage(new Mensaje {ladrillo = cubeIndex });
    }

    private void ActivateAlternatedBlocks()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].SetActive((i + playerId) % 2 == 0); //Activa solo los bloques asignados de forma alternada según el ID del jugador
        }
    }

    public void Connect()
    {
        url = urlInput.text;
        ws.Connect();
        misDatos.conectado = true;
    }
}
