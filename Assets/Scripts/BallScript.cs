using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody2D ballRigy;
    [SerializeField]
    private Vector2 initialVelocity;
    private bool isMoving;

    // Start is called before the first frame update
    void Start()
    {
        ballRigy = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            Launch();
        }
    }

    private void Launch()
    {
        transform.parent = null;
        ballRigy.velocity = initialVelocity;
        isMoving = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Destroy(collision.gameObject);
        }

        VelocityFix();
    }

    //Corrección de bug típico de quedarse la pelota atascada rebotando solo horizontalmente o verticalmente
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
}
