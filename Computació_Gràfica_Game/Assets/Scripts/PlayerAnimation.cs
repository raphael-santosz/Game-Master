using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
   [SerializeField] 
    private Animator animator;
    
    [SerializeField] 
    private Rigidbody2D rigidbody;
    
    [SerializeField]
    private Character player;
    // Update is called once per frame
    void Update()
    {
        float velocidadeX = Mathf.Abs(this.rigidbody.velocity.x);
        if(velocidadeX > 0){
            this.animator.SetBool("run",true);
        } else{
            this.animator.SetBool("run",false);
        }
    }
}
