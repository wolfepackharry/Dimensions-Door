using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class RespawnManager : MonoBehaviour
{
    private List<(Transform, Transform, Transform, Transform)> _boxes = new List<(Transform, Transform, Transform, Transform)>();
    [SerializeField] GameObject _point;
    [SerializeField] bool _saftey = true;
    public static RespawnManager Singleton;

    private void Awake()
    {
        if (Singleton == null){Singleton = this;}
        else if (Singleton != this){Destroy(gameObject);}

        List<Transform> point1s = new List<Transform>();
        List<Transform> point2s = new List<Transform>();
        List<Transform> respawnPoints = new List<Transform>();
        List<Transform> _cameraPoints = new List<Transform>();
        SortChildren(transform);
        foreach (Transform child in transform)
        {
            string name = child.name;
            if (name.Contains("Point1"))
            {
                point1s.Add(child);
            }

            if (name.Contains("Point2"))
            {
                point2s.Add(child);
            }

            if (name.Contains("RespawnPoint"))
            {
                respawnPoints.Add(child);
            }

            if (name.Contains("CamPos"))
            {
                _cameraPoints.Add(child);
            }
        }

        if (point1s.Count == 1)
        {
            _boxes.Add((point1s[0],point2s[0], respawnPoints[0], _cameraPoints[0]));
        }
        else
        {
            for (var index = 0; index < point1s.Count; index++)
            {
                _boxes.Add((point1s[index],point2s[index], respawnPoints[index], _cameraPoints[index]));
            }
        }
    }

    private void SortChildren(Transform transform1)
    {
        var children = new List<Transform>();
        foreach (Transform child in transform) children.Add(child);
        children.Sort((a, b) => string.Compare(a.name, b.name));
        for (int i = 0; i < children.Count; i++)
            children[i].SetSiblingIndex(i);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Button]
    private void SpawnPoints()
    {
        Transform point1 = Instantiate(_point, transform.position, Quaternion.identity, transform).transform;
        Transform point2 = Instantiate(_point, transform.position, Quaternion.identity, transform).transform;
        Transform respawnPoint = Instantiate(_point, transform.position, Quaternion.identity, transform).transform;
        Transform camMinPos = Instantiate(_point, transform.position, Quaternion.identity, transform).transform;
        point1.name = "Point1 Box" + _boxes.Count;
        point2.name = "Point2 Box" + _boxes.Count;
        respawnPoint.name = "RespawnPoint Box" + _boxes.Count;
        camMinPos.name = "CamPos Box" + _boxes.Count;
        _boxes.Add((point1, point2, respawnPoint, camMinPos));
        
    }

    [Button]
    private void DespawnPointPair(Transform spawnPoint1, Transform spawnPoint2, Transform spawnPoint3, Transform spawnPoint4)
    {
        print(_boxes.Count);
        if (_boxes.Contains((spawnPoint1, spawnPoint2, spawnPoint3, spawnPoint4)))
        {
            _boxes.Remove((spawnPoint1, spawnPoint2, spawnPoint3, spawnPoint4));
            Destroy(spawnPoint1.gameObject);
            Destroy(spawnPoint2.gameObject);
            Destroy(spawnPoint3.gameObject);
            Destroy(spawnPoint4.gameObject);
        }
    }
    

    public Vector3 GetCameraStartPoint(Vector3 cameraPosition)
    {
        foreach ((Transform, Transform, Transform, Transform) box in _boxes)
        {
            if (box.Item1.position.x == box.Item2.position.x || box.Item1.position.y == box.Item2.position.y){continue;}
            Transform top = box.Item1.position.y > box.Item2.position.y ? box.Item1 : box.Item2;
            Transform bottom = box.Item1.position.y < box.Item2.position.y ? box.Item1 : box.Item2;
            Transform right = box.Item1.position.x > box.Item2.position.x ? box.Item1 : box.Item2;
            Transform left = box.Item1.position.x < box.Item2.position.x ? box.Item1 : box.Item2;
            if (cameraPosition.y > bottom.position.y && cameraPosition.y < top.position.y && cameraPosition.x < right.position.x && cameraPosition.x > left.position.x)
            {
                return box.Item4.position;
            }
        }
        return Vector3.positiveInfinity;
    }
    public Vector3 GetCurrentRespawnPoint(Vector3 postion)
    {
        foreach ((Transform, Transform, Transform, Transform) box in _boxes)
        {
            if (box.Item1.position.x == box.Item2.position.x || box.Item1.position.y == box.Item2.position.y){continue;}
            Transform top = box.Item1.position.y > box.Item2.position.y ? box.Item1 : box.Item2;
            Transform bottom = box.Item1.position.y < box.Item2.position.y ? box.Item1 : box.Item2;
            Transform right = box.Item1.position.x > box.Item2.position.x ? box.Item1 : box.Item2;
            Transform left = box.Item1.position.x < box.Item2.position.x ? box.Item1 : box.Item2;
            if (postion.y > bottom.position.y && postion.y < top.position.y && postion.x < right.position.x && postion.x > left.position.x)
            {
                return box.Item3.position;
            }
        }
        return Vector3.positiveInfinity;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach ((Transform, Transform, Transform, Transform) box in _boxes)
        {
            Gizmos.DrawLine(new Vector3(box.Item1.position.x, box.Item2.position.y, box.Item1.position.z), box.Item1.position);
            Gizmos.DrawLine(new Vector3(box.Item2.position.x, box.Item1.position.y, box.Item1.position.z), box.Item1.position);
            Gizmos.DrawLine(new Vector3(box.Item2.position.x, box.Item1.position.y, box.Item1.position.z), box.Item2.position);
            Gizmos.DrawLine(new Vector3(box.Item1.position.x, box.Item2.position.y, box.Item1.position.z), box.Item2.position);
        }
    }
}
