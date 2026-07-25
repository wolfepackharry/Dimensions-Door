using System.Collections.Generic;
using UnityEngine;

public class EnvioronmentController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enablesShift;
    [SerializeField] private List<GameObject> _disablesShift;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerMovment.Singleton.OnShift += ShiftDim;
        PlayerMovment.Singleton.OnReturn += ReturnDim;
        PlayerMovment.Singleton.OnDie += ReturnDim;
    }

    private void ReturnDim()
    {
        foreach (GameObject enable in _enablesShift){enable.SetActive(false);}
        foreach (GameObject enable in _disablesShift){enable.SetActive(true);}
    }

    private void ShiftDim()
    {
        foreach (GameObject enable in _enablesShift){enable.SetActive(true);}
        foreach (GameObject enable in _disablesShift){enable.SetActive(false);}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
