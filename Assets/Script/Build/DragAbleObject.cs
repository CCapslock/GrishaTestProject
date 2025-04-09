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
    [SerializeField] private Animator _animatorController;
    [SerializeField] private Color _outlineAvailableColor;
    [SerializeField] private Color _outlineBlockedColor;
    [SerializeField] private DragableObjetType _dragableObjectType;
    [Layer]
    [SerializeField] private int _dragAbleObjectLayer;
    [Layer]
    [SerializeField] private int _defaultLayer;
    [SerializeField] private bool _canBeStackedOn;
    [SerializeField] private bool _canBeStacked;

    private DragAbleObject _collidedObject;
    private DragAbleObject _bottomObject;
    private Material _basicMaterial;
    private bool _hasBottomObject;
    private bool _isBlocked;
    private bool _isPlayerIn;
    private string _popVerticalTrigger = "PopVertical";

    public DragableObjetType ObjectType => _dragableObjectType;
    public bool IsPickedUp { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsBlocked => _isBlocked;
    public bool CanStackOn => _canBeStackedOn;
    public bool CanStack => _canBeStacked;

    public event Action onTriggerEnter;
    public event Action onTriggerExit;
    public event Action onTriggerExitPlayer;

    private void Start()
    {
        SelectObject(false);
        _basicMaterial = _renderer.sharedMaterial;
        onTriggerExitPlayer += SetCorrectColliderSettings;
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
        if (_hasBottomObject)
        {
            _hasBottomObject = false;
            _bottomObject.SetBlockedState(false);
        }
    }
    public void ReleaseObject()
    {
        _animatorController.SetTrigger(_popVerticalTrigger);
        ParticlesManager.Current.MakeParticles(ParticleType.PuffParticle, transform.position);
        gameObject.layer = _dragAbleObjectLayer;
        _renderer.sharedMaterial = _basicMaterial;
        if (!_isPlayerIn)
        {
            _collider.isTrigger = false;
        }
        IsPickedUp = false;
    }
    public void ReleaseObject(DragAbleObject bottomObject)
    {
        ReleaseObject();
        _hasBottomObject = true;
        _bottomObject = bottomObject;
        _bottomObject.SetBlockedState(true);
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
    public void SetBlockedState(bool state)
    {
        _isBlocked = state;
        if (state)
        {
            _outlinable.FrontParameters.Color = _outlineBlockedColor;
            _outlinable.BackParameters.Color = _outlineBlockedColor;
        }
        else
        {
            _outlinable.FrontParameters.Color = _outlineAvailableColor;
            _outlinable.BackParameters.Color = _outlineAvailableColor;
        }
    }
    private void SetCorrectColliderSettings()
    {
        if (!_isPlayerIn)
        {
            _collider.isTrigger = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagManager.GetTag(TagType.DragableObject)) || other.CompareTag(TagManager.GetTag(TagType.Wall)))
        {
            onTriggerEnter?.Invoke();
        }
        if (other.CompareTag(TagManager.GetTag(TagType.Player)))
        {
            _isPlayerIn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagManager.GetTag(TagType.DragableObject)) || other.CompareTag(TagManager.GetTag(TagType.Wall)) )
        {
            onTriggerExit?.Invoke();
        }
        if(other.CompareTag(TagManager.GetTag(TagType.Player)))
        {
            _isPlayerIn = false;
            onTriggerExitPlayer?.Invoke(); 
        }
    }
}
