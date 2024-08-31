using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BallScript : MonoBehaviour
{
    private Rigidbody2D ballRigy;
    [SerializeField]
    private Vector2 initialVelocity;
    private bool isMoving;
    public static bool canStart = false;
    private ClientWebSocket gameController;

    [SerializeField]
    private GameObject winPanel;
    public GameObject loosePanel;
    private Vector3 initialPosition;
    public PlayerController player;
    public Button playAgainButton;
    [HideInInspector]
    public bool canRestart = false;
    [HideInInspector]
    public bool canDeactivateWinPanel = false;
    [HideInInspector]
    public bool canDeactivateLoosePanel = false;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        ballRigy = GetComponent<Rigidbody2D>();
        gameController = GameObject.FindObjectOfType<ClientWebSocket>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving && canStart)
        {
            Launch();
        }

        if (canRestart)
        {
            Restart();
            canRestart = false;
        }

        if (canDeactivateWinPanel)
        {
            winPanel.SetActive(false);
            canDeactivateWinPanel = false;
        }

        if (canDeactivateLoosePanel)
        {
            loosePanel.SetActive(false);
            canDeactivateLoosePanel = false;
        }
    }

    private void Launch()
    {
        transform.parent = null;
        ballRigy.constraints = RigidbodyConstraints2D.None;
        ballRigy.velocity = initialVelocity;
        isMoving = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Message newMessage = new Message();
            newMessage.type = "BrockenBrick";
            newMessage.data = collision.gameObject.GetComponent<CubeController>().cubeNumber - 1;
            newMessage.origin = ClientWebSocket.playerId;

            string messageJson = JsonUtility.ToJson(newMessage);

            gameController.ws.Send(messageJson);

            collision.gameObject.SetActive(false);

            CheckIfWin();
            CheckIfLoose();
        }

        VelocityFix();
    }

    //Bug correction
    private void VelocityFix()
    {
        float velocityDelta = 0.5f;
        float minVelocity = 0.2f;

        if (Mathf.Abs(ballRigy.velocity.x) < minVelocity)
        {
            velocityDelta = Random.value < 0.5f ? velocityDelta : -velocityDelta;
            ballRigy.velocity += new Vector2(velocityDelta, 0f);
        }

        if (Mathf.Abs(ballRigy.velocity.y) < minVelocity)
        {
            velocityDelta = Random.value < 0.5f ? velocityDelta : -velocityDelta;
            ballRigy.velocity += new Vector2(0f, velocityDelta);
        }
    }

    public void CheckIfLoose()
    {
        if (gameController.cubes.All(p => p.activeInHierarchy))
        {
            Debug.Log("Comprobando si pierde");
            Message newMessage2 = new Message();
            newMessage2.type = "LostGame";
            newMessage2.data = 50;
            newMessage2.origin = ClientWebSocket.playerId;
            gameController.ws.Send(JsonUtility.ToJson(newMessage2));
        }
    }

    public void CheckIfWin()
    {
        if (gameController.cubes.All(p => !p.activeInHierarchy))
        {
            Debug.Log("Comprobando si gana");
            Message newMessage3 = new Message();
            newMessage3.type = "WinedGame";
            newMessage3.data = 100;
            newMessage3.origin = ClientWebSocket.playerId;
            gameController.ws.Send(JsonUtility.ToJson(newMessage3));
            winPanel.SetActive(true);
        }
    }

    private void Restart()
    {
        ballRigy.velocity = Vector2.zero;
        player.gameObject.transform.position = player.initialPosition;
        transform.position = initialPosition;
        transform.parent = player.gameObject.transform;
        isMoving = false;
        gameController.ActivateAlternatedBlocks();
    }

    public void TryAgain()
    {
        canRestart = true;
        canDeactivateLoosePanel = true;
        
        Message newMessage4 = new Message();
        newMessage4.type = "TryAgain";
        newMessage4.data = 120;
        newMessage4.origin = ClientWebSocket.playerId;
        gameController.ws.Send(JsonUtility.ToJson(newMessage4));
    }

    public void PlayAgain()
    {
        canRestart = true;
        canDeactivateWinPanel = true;
        
        Message newMessage5 = new Message();
        newMessage5.type = "PlayAgain";
        newMessage5.data = 130;
        newMessage5.origin = ClientWebSocket.playerId;
        gameController.ws.Send(JsonUtility.ToJson(newMessage5));
    }
}
