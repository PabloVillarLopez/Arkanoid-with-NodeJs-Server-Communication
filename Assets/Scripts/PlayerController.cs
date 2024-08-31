using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    [HideInInspector]
    public Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (BallScript.canStart)
        {
            Move();
        }   
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        Vector2 playerPosition = transform.position;
        playerPosition.x = Mathf.Clamp(playerPosition.x + moveInput * moveSpeed * Time.deltaTime, -1.5f, 7.5f);
        transform.position = playerPosition;
    }
}
