using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineDraw : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Draw Attributes")] private GameObject _line;
    private bool _isDrawing;
    private Vector3 _mousePosition;
    private Camera _mainCamera;
    [SerializeField] private GameObject _drawObjPrefab;
    private GameObject _drawInstantiated;

    [SerializeField] private Material _lineMaterial;

    [Header("Line Attributes")] private LineRenderer _lr;
    private int _currentIndex;

    private void Awake()
    {
        CreateLine();
    }

    void Update()
    {
        if (_isDrawing)
        {
            var distance = _mousePosition - Input.mousePosition;
            var distanceSqrMagnitude = distance.sqrMagnitude;
            if (distance.sqrMagnitude > 1000f)
            {
                _lr.SetPosition(_currentIndex,
                    _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        Input.mousePosition.z + 10f)));
                if (_drawInstantiated)
                {
                    var currentLinePosition = _lr.GetPosition(_currentIndex);
                    _drawInstantiated.SetActive(true);
                    _drawInstantiated.transform.LookAt(currentLinePosition);
                    if (_drawInstantiated.transform.rotation.y == 0)
                    {
                        _drawInstantiated.transform.eulerAngles =
                            new Vector3(_drawInstantiated.transform.rotation.eulerAngles.x, 90,
                                _drawInstantiated.transform.rotation.eulerAngles.z);
                    }

                    _drawInstantiated.transform.localScale = new Vector3(_drawInstantiated.transform.localScale.x,
                        Vector3.Distance(_drawInstantiated.transform.position, currentLinePosition) * 0.5f,
                        35f);
                }

                _drawInstantiated = Instantiate(_drawObjPrefab, _lr.GetPosition(_currentIndex), Quaternion.identity, _line.transform);
                _drawInstantiated.SetActive(false);
                _mousePosition = Input.mousePosition;
                _currentIndex++;
                _lr.positionCount = _currentIndex + 1;
                _lr.SetPosition(_currentIndex,
                    _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        Input.mousePosition.z + 10f)));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDrawing = true;
        _mousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrawing = false;
        _line.AddComponent<Rigidbody>();
        _lr.useWorldSpace = false;
        _currentIndex = 0;
        Destroy(_drawInstantiated);
        CreateLine();
    }

    public void CreateLine()
    {
        _line = new GameObject();
        _line.transform.name = "Line";
        _lr = _line.AddComponent<LineRenderer>();
        _lr.startWidth = 0.2f;
        _lr.material = _lineMaterial;
        _lr.enabled = false;
        if (Camera.main) _mainCamera = Camera.main;
    }
}