using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private Action _onWin;
    public Action OnWin { get { return _onWin; } set { _onWin = value; } }
    // Update is called once per frame

    private void Awake()
    {
        PlayerMovment.Singleton.OnShift += Vanish;
        PlayerMovment.Singleton.OnReturn += Return;
    }

    private void Return()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    private void Vanish()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnWin?.Invoke();
        }
    }
}
