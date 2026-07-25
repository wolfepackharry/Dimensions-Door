using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private GameObject _winPopUp;
    [SerializeField] private GameObject _tipsPopUp;
    [SerializeField] private GameObject _settingsPopUp;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _exitTipsButton;
    [SerializeField] private Button _exitSettingsButton;
    [SerializeField] private Button _WButton;
    [SerializeField] private Button _AButton;
    [SerializeField] private Button _DButton;
    [SerializeField] private Button _DashButton;
    [SerializeField] private Button _ShiftButton;
    [SerializeField] private Button _SettingsButton;
    [SerializeField] private Button _tipsButton;
    
    private bool _searching = false;
    public enum  Key { W, A, D, Dash, Shift }
    private Key _currentKey;
    private void Awake()
    {
        FindAnyObjectByType<Goal>().OnWin += Win;
        _playAgainButton.onClick.AddListener(PlayAgain);
        _SettingsButton.onClick.AddListener(Settings);
        _exitSettingsButton.onClick.AddListener(ExitSettings);
        _WButton.onClick.AddListener(ChangeWKey);
        _AButton.onClick.AddListener(ChangeAKey);
        _DButton.onClick.AddListener(ChangeDKey);
        _ShiftButton.onClick.AddListener(ChangeShiftKey);
        _DashButton.onClick.AddListener(ChangeDashKey);
        _tipsButton.onClick.AddListener(Tips);
        _exitTipsButton.onClick.AddListener(ExitTips);
    }

    private void ExitTips()
    {
        _tipsPopUp.SetActive(false);
        _settingsPopUp.SetActive(true);
    }

    private void Tips()
    {
        _settingsPopUp.SetActive(false);
        _tipsPopUp.SetActive(true);
    }

    private void ChangeWKey()
    {
        _currentKey = Key.W;
        _WButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        _searching  = true;
    }
    private void ChangeAKey()
    {
        _currentKey = Key.A;
        _AButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        _searching  = true;
    }
    private void ChangeDKey()
    {
        _currentKey = Key.D;
        _DButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        _searching  = true;
    }
    private void ChangeDashKey()
    {
        _currentKey = Key.Dash;
        _DashButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        _searching  = true;
    }
    private void ChangeShiftKey()
    {
        _currentKey = Key.Shift;
        _ShiftButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        _searching  = true;
    }

    private void Update()
    {
        if (_searching)
        {
            if (Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                _searching = false;
                if (Keyboard.current.backspaceKey.wasPressedThisFrame)
                {
                    _searching = false;
                    switch (_currentKey)
                    {
                        case Key.W:
                            _WButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerMovment.Singleton.GetKey(_currentKey).displayName;
                            break;
                        case Key.A:
                            _AButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerMovment.Singleton.GetKey(_currentKey).displayName;
                            break;
                        case Key.D:
                            _DButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerMovment.Singleton.GetKey(_currentKey).displayName;
                            break;
                        case Key.Shift:
                            _ShiftButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerMovment.Singleton.GetKey(_currentKey).displayName;
                            break;
                        case Key.Dash:
                            _DashButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerMovment.Singleton.GetKey(_currentKey).displayName;
                            break;
                    }
                }
            }
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                foreach (var key in Keyboard.current.allKeys)
                {
                    if (key.wasPressedThisFrame && CheckKey(key))
                    {
                        PlayerMovment.Singleton.ChangeKey(_currentKey, key);
                        _searching = false;
                        switch (_currentKey)
                        {
                            case Key.W:
                                _WButton.GetComponentInChildren<TextMeshProUGUI>().text = key.displayName;
                                break;
                            case Key.A:
                                _AButton.GetComponentInChildren<TextMeshProUGUI>().text = key.displayName;
                                break;
                            case Key.D:
                                _DButton.GetComponentInChildren<TextMeshProUGUI>().text = key.displayName;
                                break;
                            case Key.Shift:
                                _ShiftButton.GetComponentInChildren<TextMeshProUGUI>().text = key.displayName;
                                break;
                            case Key.Dash:
                                _DashButton.GetComponentInChildren<TextMeshProUGUI>().text = key.displayName;
                                break;
                        }
                        break;
                    }
                }
            }
        }
    }
    private bool CheckKey(KeyControl key)
    {
        foreach (Key k in Enum.GetValues(typeof(Key)))
        {
            if (PlayerMovment.Singleton.GetKey(k) == key)
            {
                return false;
            }
        }

        return true;
    }

    private void ExitSettings()
    {
        _settingsPopUp.SetActive(false);
        Time.timeScale = 1;
        _SettingsButton.interactable = true;
    }

    private void Settings()
    {
        _settingsPopUp.SetActive(true);
        Time.timeScale = 0;
        _SettingsButton.interactable = false;
    }

    private void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Win()
    {
        Time.timeScale = 0;
        _winPopUp.SetActive(true);
    }
}
