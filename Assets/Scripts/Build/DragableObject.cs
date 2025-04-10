using EPOOutline;
using NaughtyAttributes;
using System;
using UnityEngine;

public abstract class DragableObject : MonoBehaviour
{
    [SerializeField] private Outlinable _outlinable;
    [SerializeField] private Collider _collider;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material _greenTransparentMaterial;
    [SerializeField] private Material _redTransparentMaterial;
    [SerializeField] private Animator _animatorController;
    [SerializeField] protected LayerMask _floorLayer;
    [SerializeField] protected LayerMask _dragableObjectLayer;
    [SerializeField] protected LayerMask _wallLayer;
    [SerializeField] private Color _outlineAvailableColor;
    [SerializeField] private Color _outlineBlockedColor;
    [SerializeField] protected float _snapDistance;
    [SerializeField] protected float _snapDiffrence;
    [Layer]
    [SerializeField] private int _dragAbleObjectLayer;
    [Layer]
    [SerializeField] private int _defaultLayer;
    [SerializeField] protected bool _canBeStackedOn;
    [SerializeField] protected bool _canBeStacked;

    private Quaternion _goalObjectRotation;
    protected Vector3 _calculatedRaycastStartPosition = new Vector3();
    protected Vector3 _calculatedPlacedPosition = new Vector3();
    protected Vector3 _startPosition;
    protected DragableObject _collidedObject;
    protected DragableObject _bottomObject;
    private Material _basicMaterial;
    private int _collidedObjectsAmount;
    protected bool _hasBottomObject;
    protected bool _hasPlacedPosition;
    private bool _isBlocked;
    protected bool _isPlayerIn;
    private bool _needToRotate;
    private bool _canBePlaced;
    private string _popVerticalTrigger = "PopVertical";

    public bool IsPickedUp { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsBlocked => _isBlocked;
    public bool CanStackOn => _canBeStackedOn;
    public bool CanStack => _canBeStacked;
    public bool CanBePlaced => _canBePlaced;
    public bool NeedToRotate => _needToRotate;

    protected event Action onTriggerExitPlayer;


    private void Start()
    {
        SelectObject(false);
        _basicMaterial = _renderer.sharedMaterial;
        onTriggerExitPlayer += SetCorrectColliderSettings;
    }
    public virtual void DragObject(Transform cameraTransform, float distanceToPickUp)
    {

    }
    public virtual void PlaceObject()
    {
        transform.position = _calculatedPlacedPosition;
        if (_hasBottomObject)
        {
            ReleaseObject(_bottomObject);
        }
        else
        {
            ReleaseObject();
        }
    }
    public void RotateObject(float rotationSpeed)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _goalObjectRotation, rotationSpeed);
        if (transform.rotation == _goalObjectRotation)
        {
            _needToRotate = false;
        }
    }
    public void StartRotateObject(Vector2 scrollInput)
    {
        _needToRotate = true;
        _goalObjectRotation = Quaternion.Euler(_goalObjectRotation.eulerAngles + (Vector3.up * (45f * scrollInput.y)));
    }
    protected void MoveDragableItem(Transform cameraTransform,float distanceToPickUp)
    {
        if (!_hasPlacedPosition)
        {
            _calculatedPlacedPosition = cameraTransform.position + cameraTransform.forward * distanceToPickUp;
        }
        transform.position = _calculatedPlacedPosition;
    }
    protected void CheckIfCanPlace()
    {
        if (_collidedObjectsAmount <= 0 && _hasPlacedPosition)
        {
            _canBePlaced = true;
        }
        else
        {
            _canBePlaced = false;
        }
        SetPlaceAbleState(_canBePlaced);
    }
    public void SelectObject(bool state)
    {
        IsSelected = state;
        _outlinable.enabled = state;
    }
    public void PickUpObject()
    {
        if (_hasBottomObject)
        {
            _hasBottomObject = false;
            _bottomObject.SetBlockedState(false);
        }
        _startPosition = transform.position;
        _goalObjectRotation = transform.rotation;

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
    public void ReleaseObject(DragableObject bottomObject)
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
            _collidedObjectsAmount++;
        }
        if (other.CompareTag(TagManager.GetTag(TagType.Player)))
        {
            _isPlayerIn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagManager.GetTag(TagType.DragableObject)) || other.CompareTag(TagManager.GetTag(TagType.Wall)))
        {
            _collidedObjectsAmount--;
        }
        if (other.CompareTag(TagManager.GetTag(TagType.Player)))
        {
            _isPlayerIn = false;
            onTriggerExitPlayer?.Invoke();
        }
    }
    public RaycastCheckResult CheckForObjectByLayer(Transform startTransform, LayerMask layer, float pickupDistance)
    { 
        RaycastCheckResult result = new RaycastCheckResult();
        RaycastHit hit;
        Ray ray = new Ray(startTransform.position, startTransform.forward);

        if (Physics.Raycast(ray, out hit, pickupDistance, layer))
        {
            result.DistanceToObject = hit.distance;
            result.HasObject = true;
        }
        else
        {
            result.HasObject = false;
        }
        return result;
    }
}
