using UnityEngine;

public class DragController : MonoBehaviour
{
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private LayerMask _floorLayer;
    [SerializeField] private LayerMask _stackAbleObjectLayer;
    [SerializeField] private float _objectMovementSpeed;
    [SerializeField] private float _objectSeeDistance;
    [SerializeField] private float _pickUpDistance;
    [SerializeField] private float _stackMaxHieght;

    private RaycastHit _hit;
    private DragAbleObject _raycastedDragAbleObject;
    private DragAbleObject _currentDragableObject;
    private Vector3 _startPosition;
    private Vector3 _calculatedPosition = new Vector3();
    private Vector3 _calculatedRaycastStartPosition = new Vector3();
    [SerializeField] private Vector3 _calculatedPlacedPosition = new Vector3();
    private float _distanceToFloor;
    [SerializeField] private bool _itemPickedUp;
    [SerializeField] private bool _isCameraSeeFloor;
    [SerializeField] private bool _itemSelected;
    [SerializeField] private bool _hasPlacedPosition;
    [SerializeField] private bool _isPositionStackObject;
    private void Update()
    {
        if (_itemPickedUp)
        {
            CalculateCameraDistanceToFloor();

            CheckForFloor();

            if (_currentDragableObject.IsStackAble)
            {
                CheckForStackAbleObject();
            }

            MoveDragableItem();
        }
        else
        {
            CheckForItems();
        }
    }
    private void CalculateCameraDistanceToFloor()
    {
        Ray ray = new Ray(_playerCamera.position, _playerCamera.forward);

        if (Physics.Raycast(ray, out _hit, _pickUpDistance, _floorLayer))
        {
            _distanceToFloor = _hit.distance;
            _isCameraSeeFloor = true;
        }
        else
        {
            _isCameraSeeFloor = false;

        }
    }
    private void CheckForStackAbleObject()
    {
            _calculatedRaycastStartPosition = _playerCamera.position + _playerCamera.forward * _pickUpDistance;
        _calculatedRaycastStartPosition.y = _stackMaxHieght;

        RaycastHit hit;
        Ray ray = new Ray(_calculatedRaycastStartPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, _stackMaxHieght + 1f, _stackAbleObjectLayer))
        {
            _hasPlacedPosition = true;
            _isPositionStackObject = true;
            _calculatedPlacedPosition = hit.point;
        }
    }
    private void CheckForFloor()
    {
        _calculatedRaycastStartPosition = _playerCamera.position + _playerCamera.forward * _pickUpDistance; 
        
        _calculatedRaycastStartPosition.y = _stackMaxHieght;

        RaycastHit hit;
        Ray ray = new Ray(_calculatedRaycastStartPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, _stackMaxHieght + 1f, _floorLayer))
        {
            _hasPlacedPosition = true;
            _calculatedPlacedPosition = hit.point;
        }
    }
    private void MoveDragableItem()
    {
        //if (!_isCameraSeeFloor)
        //{
        //    _calculatedPosition = _playerCamera.position + _playerCamera.forward * _pickUpDistance;
        //}
        _currentDragableObject.transform.position = _calculatedPlacedPosition;
    }
    public void PickUpItem(DragAbleObject dragableObject)
    {
        if (!_itemPickedUp)
        {
            _currentDragableObject = dragableObject;
            _currentDragableObject.PickUpObject();
            _startPosition = _currentDragableObject.transform.position;
            _itemPickedUp = true;
        }
    }
    public void ReleaseObject()
    {
        if (_itemPickedUp)
        {
            _itemPickedUp = false;
            _currentDragableObject.transform.position = _startPosition;
            _currentDragableObject.ReleaseObject();
        }
    }
    public void TryLMBInput()
    {
        if (_itemPickedUp)
        {
            if (_hasPlacedPosition)
            {
                PlaceItem();
            }
        }
        else
        {
            if (_itemSelected)
            {
                PickUpItem(_currentDragableObject);
            }
        }
    }
    private void PlaceItem()
    {
        _itemPickedUp = false;
        _currentDragableObject.transform.position = _calculatedPlacedPosition;
        _currentDragableObject.ReleaseObject();
    }
    private void CheckForItems()
    {
        RaycastHit hit;
        Ray ray = new Ray(_playerCamera.position, _playerCamera.forward);
        if (Physics.Raycast(ray, out hit, _objectSeeDistance))
        {
            if (hit.collider.TryGetComponent(out _raycastedDragAbleObject))
            {
                if (_itemSelected)
                {
                    if (_currentDragableObject != _raycastedDragAbleObject)
                    {
                        _currentDragableObject.SelectObject(false);
                        _currentDragableObject = _raycastedDragAbleObject;
                    }
                }
                else
                {
                    _currentDragableObject = _raycastedDragAbleObject;
                }
                if (!_currentDragableObject.IsSelected)
                {
                    _itemSelected = true;
                    _currentDragableObject.SelectObject(true);
                }
            }
        }
        else
        {
            if (_itemSelected)
            {
                if (_currentDragableObject.IsSelected)
                {
                    _itemSelected = false;
                    _currentDragableObject.SelectObject(false);
                }
            }
        }
    }
}