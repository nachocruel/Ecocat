using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private bool m_FacingRight = true;
    private Animator m_Anim;
    public float forca = 10f;
    public AudioSource audioSrcPlayer;
    public GameObject[] Mensagens;
    private static PlayerController playerController;
    private bool ctrl = false;

    public static PlayerController getInstance()
    {
        if (!playerController)
        {
            playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;        
        }
        return playerController;
    }

    private void Awake()
    {
        m_Anim = GetComponent<Animator>();
    }

    bool enviandoMensagem = false;
    private void FixedUpdate()
    {
        ctrl = Input.GetKey(KeyCode.LeftControl);
        if (enviandoMensagem)
        {
            return;
        }
        if (Ambiente.instance.jogoFinalizado)
        {
            enabled = false;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if ((h > 0 || h < 0) || (v < 0 || v > 0))
        {
            m_Anim.SetBool("walking", true);
            Vector3 move = new Vector3(h, v, 0);
            if (transform.position.y > 9.5f)
            {
                move.y--;
            }

            if (transform.position.y < -43.71)
            {
                move.y++;
            }
            transform.position += move * forca * Time.deltaTime;

            if (h > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if (h < 0 && m_FacingRight)
            {
                Flip();
            }
        }
        else
        {
            m_Anim.SetBool("walking", false);
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Lixo")
        {
            m_Anim.SetTrigger("PegarLixo");
            if (Ambiente.instance.getLixoJogado().Contains(other.gameObject))
            {
                Ambiente.instance.getLixoJogado().Remove(other.gameObject);
            }

            if (Ambiente.instance.getListSelecionado().Contains(other.gameObject))
            {
                Ambiente.instance.getListSelecionado().Remove(other.gameObject);
            }
            audioSrcPlayer.Play();
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
        
        if(other.tag == "EspalhaLixo" && ctrl)
        {
            if (!enviandoMensagem)
            {
                enviandoMensagem = true;
                Ambiente.instance.iniciando = true;
                int indexMensage = Random.Range(0, Mensagens.Length - 1);
                GameObject msgObj = Mensagens[indexMensage];
                Vector3 msgPosition = new Vector3(transform.position.x + 23, transform.position.y + 11, 0f);
                msgObj.transform.position = msgPosition;
                msg = Instantiate(msgObj);
                other.gameObject.SendMessage("ouvirMensagem", 0.5f);
                (gameObject.GetComponent(typeof(Collider2D)) as Collider2D).isTrigger = false;
                Invoke("mudarEnviando", 5f);
            }         
        }
    }

    GameObject msg;
    void mudarEnviando()
    {     
        Destroy(msg);
        enviandoMensagem = false;
        Ambiente.instance.iniciando = false;
        (gameObject.GetComponent(typeof(Collider2D)) as Collider2D).isTrigger = true;
    }

    public string enviarMensagem()
    {
        return "Ok!";
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
