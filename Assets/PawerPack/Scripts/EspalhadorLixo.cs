
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EspalhadorLixo : MonoBehaviour
{

    public GameObject[] lixos;
    public GameObject[] mensagens;
    private bool m_FacingRight = true;
    private int[] direction = { 1, 0, -1, 1, 0, -1, 1, 0, -1, 1, 0, -1, 1, 0, -1 };
    public float forca = 0.2f;
    public Animator m_Anim;
    private int OptX = 0;
    private int OptY = 0;
    public bool ajudarPegar;
    public bool pararJogarLixo;
    private bool identificarAlvo = true;
    GameObject Player;
    private PlayerController playerContoller;
    private static EspalhadorLixo espalhaLixoScript;

    public static EspalhadorLixo getInstance()
    {
        if (!espalhaLixoScript)
        {
            espalhaLixoScript = FindObjectOfType(typeof(EspalhadorLixo)) as EspalhadorLixo;
        }
        return espalhaLixoScript;
    }

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        InvokeRepeating("JogaLixo", 3f, 10f);
        InvokeRepeating("move", 0f, 3f);
        ajudarPegar = false;
        pararJogarLixo = false;
        Player = GameObject.FindGameObjectWithTag("Player");
        playerContoller = PlayerController.getInstance();
    }


    void JogaLixo()
    {

        int pLixo = Random.Range(0, lixos.Length);
        GameObject lixo = lixos[pLixo];
        if (transform.position.y < 6.8f)
        {
            Vector3 positionVector = new Vector3(transform.position.x, transform.position.y, 0f);
            lixo.transform.position = positionVector;
        }
        else
        {
            Vector3 positionVector = new Vector3(transform.position.x, transform.position.y - 9, 0f);
            lixo.transform.position = positionVector;
        }

        GameObject novoLixo = Instantiate(lixo);
        //novoLixo.transform.position = positionVector;
        Ambiente.instance.addLixo(novoLixo);

        m_Anim.SetTrigger("JogarLixo");

    }

    Transform bestTarget;
    bool pensando = false;
    void FixedUpdate()
    {
        if (pensando)
        {
            return;
        }

        if (Ambiente.instance.jogoFinalizado)
        {
            enabled = false;
            stopInvoke();
        }

        if (!pararJogarLixo && !ajudarPegar)
        {
            if (OptX < 10 && OptY < 10)
            {

                int x = direction[OptX];
                int y = direction[OptY];
                m_Anim.SetBool("walking", true);

                Vector3 move = new Vector3(x, y, 0);
                if (transform.position.y > 7.8f)
                {
                    move.y--;
                }

                if (transform.position.y < -38.71)
                {
                    move.y++;
                }
                transform.position += move * forca * Time.deltaTime;

                if (x > 0 && !m_FacingRight)
                {
                    Flip();
                }
                else if (x < 0 && m_FacingRight)
                {
                    Flip();
                }
            }
            else
            {
                m_Anim.SetBool("walking", false);
            }
        }
        else if (ajudarPegar)
        {
            if (identificarAlvo)
            {
                bestTarget = getBestLixoTarget();
                if (bestTarget != null)
                    identificarAlvo = false;
            }

            if (!identificarAlvo)
            {
                int xDir = 0;
                int yDir = 0;

                if (bestTarget != null)
                {
                    /*
                    if (Mathf.Abs(bestTarget.position.x - transform.position.x) > float.Epsilon)
                    {
                        yDir =  bestTarget.position.y >  transform.position.y ? 1 : -1;
                        print(bestTarget.gameObject.transform.position.y - transform.position.y > 0);
                    }
                    else
                    {
                        xDir =  bestTarget.position.x > transform.position.x ? 1 : -1;
                      
                    }
                    */

                    xDir = bestTarget.position.x - transform.position.x > 0 ? 1 : -1;
                    yDir = bestTarget.position.y - transform.position.y > 0 ? 1 : -1;
                }
                else
                {
                    identificarAlvo = true;
                }

                m_Anim.SetBool("walking", true);
                Vector3 move = new Vector3(xDir, yDir, 0);
                if (transform.position.y > 7.8f)
                {
                    move.y--;
                }

                if (transform.position.y < -38.71)
                {
                    move.y++;
                }
                transform.position += move * forca * Time.deltaTime;

                if (xDir > 0 && !m_FacingRight)
                {
                    Flip();
                }
                else if (xDir < 0 && m_FacingRight)
                {
                    Flip();
                }
            }
            else
            {
                m_Anim.SetBool("walking", false);
            }

        }
        else if (pararJogarLixo)
        {
            Vector3 move = new Vector3(-1, transform.position.y, 0);
            transform.position += move;
            if (transform.position.x + 200 < Player.transform.position.x)
            {
                Destroy(gameObject);
            }
        }
    }

    void move()
    {
        OptX = Random.Range(0, direction.Length + 3);
        OptY = Random.Range(0, direction.Length + 3);
    }

    void stopInvoke()
    {
        CancelInvoke("move");
        CancelInvoke("JogaLixo");
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ajudarPegar)
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
                if (other.gameObject != null)
                    Destroy(other.gameObject);
                identificarAlvo = true;
            }

        }
    }

    GameObject msg;
    void tomarDecisao()
    {
        int decide = Random.Range(0, 5);
        print(decide);
        ajudarPegar = decide > 2 ? true : false;
        if (pararJogarLixo || ajudarPegar)
        {
            CancelInvoke("JogaLixo");           
            (gameObject.GetComponent(typeof(Collider2D)) as Collider2D).isTrigger = false;
            Invoke("construirMensagemPositiva", 4f);          
        }
        else
        {         
            Invoke("construirMensagemNegativa", 4f);
        }
        
    }

    void construirMensagemPositiva()
    {
        GameObject objBalao = mensagens[1];
        Vector3 msgPosition = new Vector3(transform.position.x + 27, transform.position.y + 33, 0f);
        objBalao.transform.position = msgPosition;
        msg = Instantiate(objBalao);
        Invoke("destrouMessage", 4f);
    }

    void construirMensagemNegativa()
    {
        GameObject objBalao = mensagens[0];
        Vector3 msgPosition = new Vector3(transform.position.x + 23, transform.position.y + 11, 0f);
        objBalao.transform.position = msgPosition;
        msg = Instantiate(objBalao);
        Invoke("destrouMessage", 4f);
    }

    void destrouMessage()
    {
        Destroy(msg);
        pensando = false;
    }

    //Verifica qual lixo esta mais próximo para ser coletado
    Transform getBestLixoTarget()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        List<GameObject> listaDeLixo = Ambiente.instance.getLixoJogado();
        GameObject ant = null;

        foreach (GameObject lixo in listaDeLixo)
        {
            if (lixo != null)
            {
                Vector3 directionToTarget = lixo.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr && !Ambiente.instance.getListSelecionado().Contains(lixo))
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = lixo.transform.transform;
                    ant = lixo;
                }
            }
        }

        if (ant != null)
            Ambiente.instance.getListSelecionado().Add(ant);
        return bestTarget;
    }

    public void ouvirMensagem()
    {
        pensando = true;
        if (ajudarPegar == false)
        { 
            Invoke("tomarDecisao", 0.5f);
        }
    }
}
