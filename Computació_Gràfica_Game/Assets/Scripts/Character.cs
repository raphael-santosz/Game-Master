using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Character : MonoBehaviour
{
 
    public Rigidbody2D rigidbody2D;
    public float velocidadMovimento;
    public SpriteRenderer spriteRenderer;

    public float forcaPulo;

    public Transform detectorChao;
    public float raioDeteccao;

    public LayerMask layerChao;

    public bool pulando;
    public bool estaNoChao;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
      Move();
      Jump();
    }

    private void Move(){
        float horizontal = Input.GetAxis("Horizontal");
        Vector2 velocidade = this.rigidbody2D.velocity;
        velocidade.x = horizontal * this.velocidadMovimento;
        this.rigidbody2D.velocity = velocidade;

        if (velocidade.x > 0){
            this.spriteRenderer.flipX = false;
        }else if (velocidade.x < 0){
            this.spriteRenderer.flipX = true;
        }
    }
    
    private void Jump(){
        Collider2D collider = Physics2D.OverlapCircle(this.detectorChao.position, this.raioDeteccao, this.layerChao);
        if (collider != null){
            this.estaNoChao = true;
            this.pulando = false;
            }else{
                this.estaNoChao = false;
            }
        
        if (this.estaNoChao){
            if (Input.GetKeyDown(KeyCode.W ) || Input.GetKeyDown(KeyCode.UpArrow)){
                if(!this.pulando){
                    AplicarForcaPulo();
                }    
            
            
            }
            
        }
           
    }

    private void AplicarForcaPulo(){
        Vector2 forca = new Vector2(0, this.forcaPulo);
        this.rigidbody2D.AddForce(forca, ForceMode2D.Impulse); 

    }
    public bool EstaNoChao{
        get {
            return this.estaNoChao;
        }
    }

    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(this.detectorChao.position, this.raioDeteccao);

    }
    
}
