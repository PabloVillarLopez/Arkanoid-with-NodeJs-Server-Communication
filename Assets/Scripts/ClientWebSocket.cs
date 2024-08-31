using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ClientWebSocket : MonoBehaviour
{
    public WebSocket ws;

    public GameObject[] cubes;
    public static int playerId;

    private string url;
    public TMP_InputField urlInput;

    private bool canActivateCubes = false;
    private bool canActivateEnemyCubes = false;
    private int enemyCubeToActivate;
    private bool canActiveLoosePanel = false;

    [Header("Restart Settings")]
    public BallScript ball;
    public Button playAgainButton;
    public Button tryAgainButton;

    [Header("Error AND Feedback handler")]
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI feedbackText;
    private bool canShowErrorText = false;
    private bool waitingForPlayer = false;
    private bool feedbackTextCanRestart = false;
    private bool canShowConnectingPanel = false;
    [SerializeField]
    private GameObject connectingPanel;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var cube in cubes)
        {
            cube.SetActive(false);
        }
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        try
        {
            Message receivedMessage = JsonUtility.FromJson<Message>(e.Data);
            switch (receivedMessage.type)
            {
                case "Welcome":
                    playerId = receivedMessage.data;
                    Debug.Log("ID del jugador establecido: " + playerId);

                    Debug.Log("Llamando a ActivateAlternatedBlocks() con playerId: " + playerId);

                    canActivateCubes = true;
                    break;
                case "BrockenBrick":
                    Debug.Log("BrockenBrick");
                    // alguien ha roto un ladrillo

                    if (receivedMessage.origin != playerId)
                    {   // descartar si he sido yo mismo quien envió el mensaje
                        Debug.Log("El otro jugador ha roto el ladrillo número: " + receivedMessage.data);
                        // ...hacer lo que sea necesario en el juego (aparecerme a mí ese ladrillo que el otro a eliminado)
                        enemyCubeToActivate = receivedMessage.data;
                        canActivateEnemyCubes = true;
                    }

                    break;
                case "WinedGame":
                    canActiveLoosePanel = true;
                    
                    Debug.Log("Partida Perdida");
                    break;

                case "LostGame":
                    
                    Debug.Log("Partida Ganada");
                    break;

                case "TryAgain":
                    ball.canRestart = true;
                    ball.canDeactivateWinPanel = true;

                    break;

                case "PlayAgain":
                    ball.canRestart = true;
                    ball.canDeactivateLoosePanel = true;

                    break;

                case "Error":
                    canShowErrorText = true;
                    
                    break;

                case "WaitingForPlayer":
                    waitingForPlayer = true;
                    break;

                case "StartGame":
                    BallScript.canStart = true;
                    feedbackTextCanRestart = true;
                    break;

                case "ClosedConnection":
                    BallScript.canStart = false;
                    StatusUIManager.canRefresh = true;
                    StatusUIManager.canRefreshInit = true;
                    canShowConnectingPanel = true;
                    ball.canRestart = true;
                    waitingForPlayer = true;
                    break;

                default: // tipo de mensaje no conocido
                    Debug.Log("Algo va mal");
                    break;
            }

            Debug.Log(e.Data);
            Debug.Log(receivedMessage.data);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Excepción: " + ex);
            throw;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canActivateCubes)
        {
            ActivateAlternatedBlocks();
        }

        if (canActivateEnemyCubes)
        {
            ActivateEnemyCube();
        }

        if (canActiveLoosePanel)
        {
            ball.loosePanel.SetActive(true);
            canActiveLoosePanel = false;
        }

        if (canShowErrorText)
        {
            errorText.text = "Error. The game is already full. Try again later.";
            canShowErrorText = false;
        }

        if (waitingForPlayer)
        {
            feedbackText.text = "Esperando al segundo jugador...";
            waitingForPlayer = false;
        }

        if (feedbackTextCanRestart)
        {
            feedbackText.text = "";
            feedbackTextCanRestart = false;
        }

        if (canShowConnectingPanel)
        {
            connectingPanel.SetActive(true);
            canShowConnectingPanel = false;
        }
    }

    

    public void ActivateAlternatedBlocks()
    {
        Debug.Log("Desactivando cubos");

        try
        {
            //Debug.Log("Intentando");
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i].SetActive(i % 2 == playerId); //Activa solo los bloques asignados de forma alternada según el ID del jugador
            }

            canActivateCubes = false;
        }
        catch (System.Exception e)
        {
            Debug.Log("Error. Algo va mal. Excepción: " + e);
            throw;
        }
    }

    public void Connect()
    {
        url = urlInput.text;

        try
        {
            ws = new WebSocket(url);
        }
        catch (System.Exception ex)
        {
            Debug.Log("URL No válida");
            errorText.text = "Error. Url no válida. Prueba de nuevo.";
            throw;
        }

        

        ws.OnOpen += (sender, e) =>
        {
            ClearError();
            Debug.Log("Conexión establecida correctamente.");
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("Error al intentar conectar: " + e.Message);
            if (!canShowErrorText)
            {
                errorText.text = "Error. La URL del servidor no es correcta o el servidor se encuentra apagado. Por favor escribe una URL válida o asegúrate de que el servidor se encuentra encendido.";
            }
        };

        ws.OnMessage += Ws_OnMessage;

        ws.OnClose += (sender, e) =>
        {
            if (sender == ws)
            {
                Debug.Log("Conexión cerrada con el código: " + e.Code + " y la razón: " + e.Reason);
                BallScript.canStart = false;
                ShowError("Error. La URL del servidor no es correcta o el servidor se encuentra apagado. Por favor escribe una URL válida o asegúrate de que el servidor se encuentra encendido.");
            }
        };

        try
        {
            ws.Connect();
            Debug.Log("Intentando conectar a: " + url);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Excepción durante la conexión: " + ex.Message);
            errorText.text = "Error. La URL del servidor no es correcta o el servidor se encuentra apagado. Por favor escribe una URL válida o asegúrate de que el servidor se encuentra encendido.";
            throw;
        }
    }

    private void ShowError(string message)
    {
        errorText.text = message;
    }

    private void ClearError()
    {
        errorText.text = "";
    }

    private void ClearFeedback()
    {
        feedbackText.text = "";
    }

    public void ActivateEnemyCube()
    {
        cubes[enemyCubeToActivate].SetActive(true);
        canActivateEnemyCubes = false;
    }

    private void OnDisable()
    {
        if (ws != null)
        {
            ws.Close();
        }
        
        ClearFeedback();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
