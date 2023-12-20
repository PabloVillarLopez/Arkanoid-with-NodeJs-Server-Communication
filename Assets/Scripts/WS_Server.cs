using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WS_Server : MonoBehaviour
{
    WebSocketServer server;
    public string url;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createServer()
    {
        WebSocketServer ws_server = new WebSocketServer(url);
    }
}
