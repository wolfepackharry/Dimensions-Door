using System;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Transform _target;
    public Transform Target { get { return _target; } set { _target = value; } }
    private float _startY;
    private const float TARGET_ASPECT = 16f / 9f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _target = PlayerMovment.Singleton.transform;
        _startY = transform.position.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_target == null){return;}

        CheckStart();
        Vector3 targetPos = new Vector3(_target.position.x, Mathf.Clamp(_target.position.y, _startY, Mathf.Infinity), transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
    }
    

    private Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        UpdateAspect();
    }

    void UpdateAspect()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / TARGET_ASPECT;

        if (scaleHeight < 1.0f)
        {
            // Add black bars top/bottom
            Rect rect = _camera.rect;

            rect.width = 1;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1 - scaleHeight) / 2;

            _camera.rect = rect;
        }
        else
        {
            // Add black bars left/right
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = _camera.rect;

            rect.width = scaleWidth;
            rect.height = 1;
            rect.x = (1 - scaleWidth) / 2;
            rect.y = 0;

            _camera.rect = rect;
        }
    }
    private void CheckStart()
    {
        Vector3 min = RespawnManager.Singleton.GetCameraStartPoint(_target.position);
        if (_startY != min.y && min.y != Mathf.Infinity)
        {
            _startY = min.y;
        }
    }
}
