using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Ambiente : MonoBehaviour
{

    public GameObject[] ObjetosAleatorios;
    public GameObject[] carros;
    public GameObject onda;
    private float[] Yposition = { -14.9f, -10.2f, -6.2f, -1.9f };
    private Transform target;
    public Camera camerae;
    private Timer timerT;
    public int tempoParaFim = 2;
    public static Ambiente instance = null;
    public List<GameObject> lixoJogado;
    public List<GameObject> lixoSelecionado;
    public AudioSource somMar;
    public AudioSource somChuva;
    public GameObject Cidade;
    public GameObject Cidade2;
    private bool movel = false;
    public Text textLixoJogado;
    public Text textTempo;
    private int minuto = 0;
    private int segundo = 0;
    private int mileseg = 0;
    private int countTemp = 0;
    public GameObject Objchuva;
    public float forca = 10f;
    public int minutoMaximo = 5;
    public int milSegMaximo = 5;
    public int segundoMaximo = 5;
    public AudioClip clipSomTrovao;
    public AudioClip clipSomGaivotas;
    public float levelStartDelay = 2f;
    public float restartLevelDelay = 1f;
    GameObject painel;
    public Text painelText;
    private int level = 1;
    public bool jogoFinalizado = false;

    private void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        lixoJogado = new List<GameObject>();
        lixoSelecionado = new List<GameObject>();
        iniciando = true;
        initGame();
    }

    //This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        level++;
        initGame();
    }

    void initGame()
    {
        minuto = 0;
        segundo = 0;
        mileseg = 0;
        countTemp = 0;
        block = false;
        camerae = GetComponent<Camera>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Cidade = GameObject.FindGameObjectWithTag("cidade");
        Cidade2 = GameObject.FindGameObjectWithTag("Cidade1");
        onda = GameObject.FindGameObjectWithTag("Onda");
        Objchuva = GameObject.FindGameObjectWithTag("Chuva");
        Objchuva.SetActive(false);
        InvokeRepeating("CriarEspalhaLixo", 2, 5.5f);
        InvokeRepeating("criarCarros", 2, 15f);
        InvokeRepeating("acionaTrovao", 2, 25f);
        quant = 10;
        quant = quant * level;
        timerT = new Timer(10);
        timerT.Elapsed += initTimer;
        timerT.Start();
        textTempo = GameObject.Find("TempoLimite").GetComponent<Text>();
        textLixoJogado = GameObject.Find("LixoEspalhadoText").GetComponent<Text>();
        painel = GameObject.Find("Painel");
        painelText = GameObject.Find("PainelText").GetComponent<Text>();
        painelText.text = "Nível: " + level;
        Invoke("esconderPainel", levelStartDelay);
    }

    void esconderPainel()
    {
        painel.SetActive(false);
        iniciando = false;
    }

    // Update is called once per frame
    bool recuo = false;
    public bool iniciando = true;
    void FixedUpdate()
    {
        if (iniciando) { return; }
          
        textTempo.text = tmrString;
        float h = Input.GetAxis("Horizontal");
        Objchuva.transform.position += new Vector3(h, 0f, 0f) * forca * Time.deltaTime;
        if (movel == false && h < 0)
        {
            Vector3 vCidade = new Vector3(Cidade.transform.position.x - 247.4f, 0f, 0f);
            Cidade2.transform.position = vCidade;
        }

        if ((int)h > 0)
        {
            movel = true;
            if ((int)Cidade2.transform.position.x == (int)target.position.x)
            {
                Vector3 vCidade = new Vector3(Cidade2.transform.position.x + 247.4f, 0f, 0f);
                Cidade.transform.position = vCidade;
            }


            if ((int)Cidade.transform.position.x == (int)target.position.x)
            {
                Vector3 vCidade = new Vector3(Cidade.transform.position.x + 247.4f, 0f, 0f);
                Cidade2.transform.position = vCidade;

            }
        }
        else if ((int)h < 0)
        {
            if ((int)Cidade.transform.position.x == (int)target.position.x)
            {
                Vector3 vCidade = new Vector3(Cidade.transform.position.x - 247.4f, 0f, 0f);
                Cidade2.transform.position = vCidade;
            }


            if ((int)Cidade2.transform.position.x == (int)target.position.x)
            {
                Vector3 vCidade = new Vector3(Cidade2.transform.position.x - 247.4f, 0f, 0f);
                Cidade.transform.position = vCidade;
            }
        }


        if (!recuo)
        {
            Vector3 vectorOnda = new Vector3(h, 1f, 0f);
            onda.transform.position += vectorOnda * 10f * Time.deltaTime;
            if (onda.transform.position.y >= -170)
            {
                recuo = true;
            }

        }
        else
        {
            Vector3 vectorOnda = new Vector3(h, -1f, 0f);
            onda.transform.position += vectorOnda * 10f * Time.deltaTime;
            if (onda.transform.position.y <= -200)
            {
                recuo = false;
            }

        }

        checarJogo();
    }

   void checarJogo()
   {
        if (minuto >= minutoMaximo  && lixoJogado.Count > 100)
        {
            painelText.text = "Fim de jogo!";
            painel.SetActive(true);
            Invoke("FinalizarJogo", 2f);
        }
        else if(minuto >= minutoMaximo && lixoJogado.Count < 100)
        {
            iniciando = true;
            timerT.Stop();
            timerT.Close();
            timerT = null;
            CancelInvoke("CriarEspalhaLixo");
            CancelInvoke("acionaTrovao");
            CancelInvoke("criarCarros");
            lixoJogado.Clear();
            lixoSelecionado.Clear();
            Invoke("Restart", restartLevelDelay);
        }
    }


    void criarCarros()
    {

        SoundManarger.instance.RandomizeSfxAmbiente(clipSomGaivotas);
        int index = Random.Range(0, carros.Length);
        GameObject carro = carros[index];
        GameObject novoCarro = Instantiate(carro);
    }

    public int quant = 10;
    private int[] dist = { 100, -100, 200, -200, 300, -300, 500, -500, 600, -600, 700, -700 };
    void CriarEspalhaLixo()
    {

        if (quant > 0)
        {
            float positionX = target.position.x + dist[Random.Range(0, dist.Length - 1)];
            float positionY = target.position.y;

            int index = Random.Range(0, ObjetosAleatorios.Length);
            GameObject obj = ObjetosAleatorios[index];
            GameObject novo = Instantiate(obj);
            novo.transform.position = new Vector3(positionX, positionY, 0);
            quant--;
        }

    }

    private bool block = false;
    void acionaTrovao()
    {
        if ((minuto >= (minutoMaximo - 1)) && (segundo >= (segundoMaximo / 2)) && (mileseg >= (milSegMaximo / 2)))
        {

            SoundManarger.instance.RandomizeSfx(clipSomTrovao);

            if (!block)
            {
                block = true;
                Objchuva.SetActive(true);
                somChuva.Play();
            }
        }

    }

    public void addLixo(GameObject lixo)
    {
        lixoJogado.Add(lixo);
        textLixoJogado.text = "Lixo Espalhado: " + lixoJogado.Count;
    }

    public List<GameObject> getListSelecionado()
    {
        return lixoSelecionado;
    }

    public List<GameObject> getLixoJogado()
    {
        return lixoJogado;
    }

    void FinalizarJogo()
    {
        CancelInvoke("CriarEspalhaLixo");
        CancelInvoke("acionaTrovao");
        CancelInvoke("criarCarros");
        timerT.Stop();
        SoundManarger.instance.musicSource.Stop();
        somChuva.Stop();
        somMar.Stop();
        jogoFinalizado = true;
        enabled = false;
        Destroy(this);
    }


    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }


    private string tmrString;
    
    private void initTimer(object sender, EventArgs e)
    {
        countTemp++;
        minuto = (countTemp / 3600);
        segundo = (countTemp % 3600) / 60;
        mileseg = (countTemp % 3600) % 60;
        tmrString = String.Format("{0:#,0#}:{1:#,0#}:{2:#,0#}", minuto, segundo, mileseg);
    }
}
