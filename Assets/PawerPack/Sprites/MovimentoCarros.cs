using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoCarros : MonoBehaviour {

    private bool m_FacingRight = true;
    public Transform target;
    public AudioSource audioSrcCarro;
    // Use this for initialization
    void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (tag == "VeiculoAzul")
        {
            transform.position = new Vector3(target.position.x + 300, transform.position.y, 0);
        }
        else
        {
            transform.position = new Vector3(target.position.x - 300, transform.position.y, 0f);
        }
	}

    // Update is called once per frame
    private bool block = false;
	void FixedUpdate () {
        if (Ambiente.instance.jogoFinalizado)
        {
            enabled = false;
        }
        if (tag == "VeiculoAzul")
        {
            Vector3 move = new Vector3(-1, 0, 0);
            transform.position += move;
            if (Math.Abs(target.position.x + 80) >= Math.Abs(transform.position.x) && !block) { audioSrcCarro.Play(); block = true; }
            if(transform.position.x + 200 < target.position.x)
            {
                Destroy(gameObject);
                block = false;
            }
        }
        else 
        {
            Vector3 move = new Vector3(1, 0, 0);
            transform.position += move;
            if (Math.Abs(target.position.x - 80) >= Math.Abs(transform.position.x) && !block) { audioSrcCarro.Play(); block = true; }

            if (transform.position.x - 200 > target.position.x)
            {
                Destroy(gameObject);
                block = false;
            }
        }
	}

    
    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
