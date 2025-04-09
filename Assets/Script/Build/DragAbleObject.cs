using EPOOutline;
using NaughtyAttributes;
using System;
using UnityEngine;

public class DragAbleObject : MonoBehaviour
{
    [SerializeField] private Outlinable _outlinable;
    [SerializeField] private Collider _collider;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material _greenTransparentMaterial;
    [SerializeField] private Material _redTransparentMaterial;
    [Layer]
    [SerializeField] private int _dragAbleObjectLayer;
    [Layer]
    [SerializeField] private int _defaultLayer;
    [SerializeField] private bool _stackAble;

    private DragAbleObject _collidedObject;
    private Material _basicMaterial;
    public bool IsPickedUp { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsStackAble => _stackAble;

    public event Action onTriggerEnter;
    public event Action onTriggerExit;

    private void Start()
    {
        SelectObject(false);
        _basicMaterial = _renderer.sharedMaterial;
    }
    public void SelectObject(bool state)
    {
        IsSelected = state;
        _outlinable.enabled = state;
    }
    public void PickUpObject()
    {
        gameObject.layer = _defaultLayer;
        _collider.isTrigger = true;
        IsPickedUp = true;
        if (IsSelected)
        {
            SelectObject(false);
        }
    }
    public void ReleaseObject()
    {
        gameObject.layer = _dragAbleObjectLayer;
        _renderer.sharedMaterial = _basicMaterial;
        _collider.isTrigger = false;
        IsPickedUp = false;
    }
    public void SetPlaceAbleState(bool state)
    {
        if (state) 
        {
            _renderer.sharedMaterial = _greenTransparentMaterial;
        }
        else
        {
            _renderer.sharedMaterial = _redTransparentMaterial;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _collidedObject))
        {
            onTriggerEnter?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out _collidedObject))
        {
            onTriggerExit?.Invoke();
        }
    }
}
