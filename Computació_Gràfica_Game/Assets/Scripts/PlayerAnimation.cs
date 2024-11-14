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

    private float minVelocityYForFall = -0.1f; // Margem para detectar queda

    // Update is called once per frame
    void Update()
    {
        float velocidadeX = Mathf.Abs(this.rigidbody.velocity.x);
        
        if(this.player.EstaNoChao) // Verifica se está no chão
        {
            if(velocidadeX > 0)
            {
                this.animator.SetBool("run", true);
            }
            else
            {
                this.animator.SetBool("run", false);
            }
            this.animator.SetBool("fall", false);
            this.animator.SetBool("jump", false);
        }
        else // Não está no chão
        {
            float velocidadeY = this.rigidbody.velocity.y;
            
            if (velocidadeY > 0.1f) // Pulando para cima
            {
                this.animator.SetBool("jump", true);
                this.animator.SetBool("fall", false);
            }
            else if (velocidadeY < minVelocityYForFall) // Começando a cair
            {
                this.animator.SetBool("jump", false);
                this.animator.SetBool("fall", true);
            }
        }
    }
}
