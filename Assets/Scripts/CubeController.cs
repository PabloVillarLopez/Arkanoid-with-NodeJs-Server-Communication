using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    ClienteWebSocket gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindObjectOfType<ClienteWebSocket>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            gameObject.SetActive(false);
            int cubeIndex = System.Array.IndexOf(gameController.cubes, gameObject);
            gameController.HitCube(cubeIndex);
        }
    }
}
