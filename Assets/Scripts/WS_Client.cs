using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WS_Client : MonoBehaviour
{
    WebSocket ws;

    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Datos: " + e.Data);
        };
        ws.Connect();
        
        //Los datos habrá que pasarlos a JSON para enviarlos al servidor y pasarlos de JSON cuando vengan del servidor
        //JsonUtility.ToJson

        //Funcionamiento:
        //Array de GameObjects que contiene los ladrillos
        //Jugabilidad como el pong, cuando el jugador A le da a uno de sus ladrillos le manda un mensaje al servidor con qué número de ladrillo en el array ha destruido
        //Cuando el jugador B recibe qué número de ladrillo ha destruido el jugador A, se activa el gameobject para él
        //Cuando el jugador A o el jugador B gana o pierde también se manda en el mensaje al servidor
        //Si se recibe que alguno de los jugadores gana o pierde se para el juego y se le comunica a los dos quien ha ganado
        //El servidor de nodejs tiene que capar la conexión haciendo que si ya hay el máximo de jugadores permitidos les cierre la conexion a las nuevas conexiones con un Close
    }

    // Update is called once per frame
    void Update()
    {
        if (ws == null)
        {
            return;
        }

        ws.Send("Hola");
    }
}
