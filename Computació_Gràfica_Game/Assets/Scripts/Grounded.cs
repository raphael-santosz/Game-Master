using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Grounded : MonoBehaviour
{
    Character Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = gameObject.transform.parent.gameObject.GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collisor)
    {
        if(collisor.gameObject.layer == 3)
        {
            Player.isJumping = false;
        }
    }
    void OnCollisionExit2D(Collision2D collisor)
    {
        if(collisor.gameObject.layer == 3)
        {
            Player.isJumping = true;
        }
    }
}
