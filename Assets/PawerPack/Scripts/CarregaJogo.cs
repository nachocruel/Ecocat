using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarregaJogo : MonoBehaviour {

    public GameObject ambiente;
    public GameObject soundManarger;
    void Awake()
    {
        if (Ambiente.instance == null)
            Instantiate(ambiente);

        if (SoundManarger.instance == null)
            Instantiate(soundManarger);
    }
}
