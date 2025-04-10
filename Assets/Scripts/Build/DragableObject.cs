using EPOOutline;
using NaughtyAttributes;
using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(RenderDragableObject))]
public abstract class DragableObject : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] protected LayerMask _floorLayer;
    [SerializeField] protected LayerMask _dragableObjectLayer;
    [SerializeField] protected LayerMask _wallLayer;
    [SerializeField] protected float _snapDistance;
    [SerializeField] protected float _snapDiffrence;
    [Layer]
    [SerializeField] private int _dragAbleObjectLayer;
    [Layer]
    [SerializeField] private int _defaultLayer;
    [SerializeField] protected bool _canBeStackedOn;
    [SerializeField] protected bool _canBeStacked;

    private RenderDragableObject _objectRender;
    private TagManager _tagManager;
    private Quaternion _goalObjectRotation;
    protected Vector3 _calculatedRaycastStartPosition = new Vector3();
    protected Vector3 _calculatedPlacedPosition = new Vector3();
    protected Vector3 _startPosition;
    protected DragableObject _collidedObject;
    protected DragableObject _bottomObject;
    protected bool _hasBottomObject;
    protected bool _hasPlacedPosition;
    protected bool _isPlayerIn;
    private int _collidedObjectsAmount;
    private bool _isBlocked;
    private bool _needToRotate;
    private bool _canBePlaced;

    public bool IsPickedUp { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsBlocked => _isBlocked;
    public bool CanStackOn => _canBeStackedOn;
    public bool CanStack => _canBeStacked;
    public bool CanBePlaced => _canBePlaced;
    public bool NeedToRotate => _needToRotate;

    protected event Action onTriggerExitPlayer;

    private void Awake()
    {
        _objectRender= GetComponent<RenderDragableObject>();
    }
    private void Start()
    {
        SelectObject(false);
        onTriggerExitPlayer += SetCorrectColliderSettings;
    }
    [Inject]
    private void Construct(TagManager tagManager, ParticlesManager particlesManager)
    {
        _tagManager = tagManager;
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
    protected void MoveDragableItem(Transform cameraTransform, float distanceToPickUp)
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
        _objectRender.SelectObject(state);
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
        gameObject.layer = _dragAbleObjectLayer;
        _objectRender.ReleaseObject();
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
        _objectRender.SetAvailabilityMaterial(state);
    }
    public void SetBlockedState(bool state)
    {
        _isBlocked = state;
        _objectRender.SetAvailabilityOutline(state);
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
        if (other.CompareTag(_tagManager.GetTag(TagType.DragableObject)) || other.CompareTag(_tagManager.GetTag(TagType.Wall)))
        {
            _collidedObjectsAmount++;
        }
        if (other.CompareTag(_tagManager.GetTag(TagType.Player)))
        {
            _isPlayerIn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_tagManager.GetTag(TagType.DragableObject)) || other.CompareTag(_tagManager.GetTag(TagType.Wall)))
        {
            _collidedObjectsAmount--;
        }
        if (other.CompareTag(_tagManager.GetTag(TagType.Player)))
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
