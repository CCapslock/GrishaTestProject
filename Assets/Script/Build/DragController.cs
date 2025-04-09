using UnityEngine;

public class DragController : MonoBehaviour
{
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private LayerMask _floorLayer;
    [SerializeField] private LayerMask _dragableObjectLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _objectMovementSpeed;
    [SerializeField] private float _objectSeeDistance;
    [SerializeField] private float _pickUpDistance;
    [SerializeField] private float _stackMaxHieght;
    [SerializeField] private float _stackHieghtDiffrence;
    [SerializeField] private float _objectRotationSpeed;

    private RaycastHit _hit;
    private DragAbleObject _raycastedDragAbleObject;
    private DragAbleObject _currentDragableObject;
    private DragAbleObject _bottomObject;
    private Quaternion _goalObjectRotation;
    private Vector3 _startPosition;
    private Vector3 _calculatedPosition = new Vector3();
    private Vector3 _calculatedRaycastStartPosition = new Vector3();
    private Vector3 _calculatedPlacedPosition = new Vector3();
    private float _distanceToFloor;
    private float _distanceToObject;
    private float _distanceToWall;
    private int _collidedObjectsAmount;
    private bool _itemPickedUp;
    private bool _isCameraSeeFloor;
    private bool _isCameraSeeWall;
    private bool _isCameraSeeDragableObject;
    private bool _itemSelected;
    private bool _hasPlacedPosition;
    private bool _canBePlaced;
    private bool _hasBottomObject;
    private bool _needToRotate;

    private void Update()
    {
        if (_needToRotate)
        {
            RotateObject();
        }
        if (_itemPickedUp)
        {
            if (_currentDragableObject.ObjectType == DragableObjetType.Vertical)
            {
                VerticalObjectMovement();
            }
            else
            {
                HorizontalObjectMovement();
            }

        }
        else
        {
            CheckForItems();
        }
    }

    #region VerticalObject

    public void VerticalObjectMovement()
    {
        CalculateCameraDistanceToFloor();
        _hasPlacedPosition = false;
        if (_isCameraSeeFloor)
        {
            CheckForFloor();
        }
        if (_currentDragableObject.CanStack)
        {
            CalculateCameraDistanceToDragableObject();
            if (_isCameraSeeDragableObject)
            {
                CheckForStackAbleObject();
            }
        }
        MoveDragableItem();
        CheckIfCanPlace();
    }
    private void CalculateCameraDistanceToFloor()
    {
        RaycastCheckResult temp = CheckForObjectByLayer(_floorLayer);

        _isCameraSeeFloor = temp.HasObject;
        _distanceToFloor = temp.DistanceToObject;
    }
    private void CalculateCameraDistanceToDragableObject()
    {
        RaycastCheckResult temp = CheckForObjectByLayer(_dragableObjectLayer);

        _distanceToObject = temp.DistanceToObject;
        _isCameraSeeDragableObject = temp.HasObject;
    }
    private void CheckForFloor()
    {
        _calculatedRaycastStartPosition = _playerCamera.position + _playerCamera.forward * _distanceToFloor;

        _calculatedRaycastStartPosition.y = _stackMaxHieght;

        RaycastHit hit;
        Ray ray = new Ray(_calculatedRaycastStartPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, _stackMaxHieght + 1f, _floorLayer))
        {
            _hasPlacedPosition = true;
            _calculatedPlacedPosition = hit.point;
        }
    }
    private void CheckForStackAbleObject()
    {
        _hasBottomObject = false;
        if (_isCameraSeeDragableObject)
        {
            _calculatedRaycastStartPosition = _playerCamera.position + _playerCamera.forward * _distanceToObject;
        }
        else
        {
            if (_isCameraSeeFloor)
            {
                _calculatedRaycastStartPosition = _playerCamera.position + _playerCamera.forward * _distanceToFloor;
            }
            else
            {
                _calculatedRaycastStartPosition = _playerCamera.position + _playerCamera.forward * _pickUpDistance;

            }
        }
        _calculatedRaycastStartPosition.y = _stackMaxHieght;

        Ray ray = new Ray(_calculatedRaycastStartPosition, Vector3.down);

        if (Physics.Raycast(ray, out _hit, _stackMaxHieght + 1f, _dragableObjectLayer))
        {
            if (_hit.collider.TryGetComponent(out _bottomObject))
            {
                if (_bottomObject.CanStackOn)
                {
                    _hasPlacedPosition = true;
                    _hasBottomObject = true;
                    _calculatedPlacedPosition = _hit.point + Vector3.up * _stackHieghtDiffrence;
                }
            }
        }
    }

    #endregion

    #region HorizontalObject
    public void HorizontalObjectMovement()
    {
        CalculateCameraDistanceToWall();
        _hasPlacedPosition = false;
        if (_isCameraSeeWall)
        {
            CheckForWall();
        }
        //if (_currentDragableObject.CanStack)
        //{
        //    CalculateCameraDistanceToDragableObject();
        //    if (_isCameraSeeDragableObject)
        //    {
        //        CheckForStackAbleObject();
        //    }
        //}
        MoveDragableItem();
        CheckIfCanPlace();
    }

    private void CalculateCameraDistanceToWall()
    {
        RaycastCheckResult temp = CheckForObjectByLayer(_wallLayer);

        _isCameraSeeWall = temp.HasObject;
        _distanceToWall = temp.DistanceToObject - 0.5f;
    }
    private void CheckForWall()
    {
        _calculatedRaycastStartPosition = _playerCamera.position + _playerCamera.forward * _distanceToWall;



        RaycastHit hit;
        Ray ray = new Ray(_calculatedRaycastStartPosition, _currentDragableObject.transform.forward * -1f);

        if (Physics.Raycast(ray, out hit, _stackMaxHieght + 1f, _wallLayer))
        {
            _hasPlacedPosition = true;
            _calculatedPlacedPosition = hit.point + _currentDragableObject.transform.forward * _stackHieghtDiffrence;
        }
    }
    #endregion

    private void MoveDragableItem()
    {
        if (!_hasPlacedPosition)
        {
            _calculatedPlacedPosition = _playerCamera.position + _playerCamera.forward * _pickUpDistance;
        }
        _currentDragableObject.transform.position = _calculatedPlacedPosition;
    }
    private void RotateObject()
    {
        _currentDragableObject.transform.rotation = Quaternion.RotateTowards(_currentDragableObject.transform.rotation, _goalObjectRotation, _objectRotationSpeed);
        if (_currentDragableObject.transform.rotation == _goalObjectRotation)
        {
            _needToRotate = false;
        }
    }
    private void CheckIfCanPlace()
    {
        if (_collidedObjectsAmount <= 0 && _hasPlacedPosition)
        {
            _canBePlaced = true;
        }
        else
        {
            _canBePlaced = false;
        }
        _currentDragableObject.SetPlaceAbleState(_canBePlaced);
    }
    public void PickUpItem(DragAbleObject dragableObject)
    {
        if (!_itemPickedUp)
        {
            _itemSelected = false;
            _currentDragableObject = dragableObject;
            _currentDragableObject.PickUpObject();
            _startPosition = _currentDragableObject.transform.position;
            _goalObjectRotation = _currentDragableObject.transform.rotation;
            SubscribeToObjectActions();
            _itemPickedUp = true;
        }
    }
    private void PlaceItem()
    {
        _itemPickedUp = false;
        _currentDragableObject.transform.position = _calculatedPlacedPosition;
        if (_hasBottomObject)
        {
            _currentDragableObject.ReleaseObject(_bottomObject);
        }
        else
        {
            _currentDragableObject.ReleaseObject();
        }
    }
    public void ReleaseItem()
    {
        if (_itemPickedUp)
        {
            _itemPickedUp = false;
            _currentDragableObject.transform.position = _startPosition;
            ClearSubscriptions();
            _currentDragableObject.ReleaseObject();
        }
    }
    public void TryRotateObject(Vector2 scrollInput)
    {
        if (_itemPickedUp)
        {
            _needToRotate = true;
            _goalObjectRotation = Quaternion.Euler(_goalObjectRotation.eulerAngles + (Vector3.up * (45f * scrollInput.y)));
        }
    }
    public void TryLMBInput()
    {
        if (_itemPickedUp)
        {
            if (_canBePlaced)
            {
                PlaceItem();
            }
        }
        else
        {
            if (_itemSelected && !_currentDragableObject.IsBlocked)
            {
                PickUpItem(_currentDragableObject);
            }
        }
    }
    private void SubscribeToObjectActions()
    {
        _currentDragableObject.onTriggerEnter += AddCollidedObject;
        _currentDragableObject.onTriggerExit += RemoveCollidedObject;
    }
    private void ClearSubscriptions()
    {
        _currentDragableObject.onTriggerEnter -= AddCollidedObject;
        _currentDragableObject.onTriggerExit -= RemoveCollidedObject;
        _collidedObjectsAmount = 0;
    }
    private void AddCollidedObject()
    {
        _collidedObjectsAmount++;
    }
    private void RemoveCollidedObject()
    {
        _collidedObjectsAmount--;
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
    private RaycastCheckResult CheckForObjectByLayer(LayerMask layer)
    {
        RaycastCheckResult result = new RaycastCheckResult();
        Ray ray = new Ray(_playerCamera.position, _playerCamera.forward);

        if (Physics.Raycast(ray, out _hit, _pickUpDistance, layer))
        {
            result.DistanceToObject = _hit.distance;
            result.HasObject = true;
        }
        else
        {
            result.HasObject = false;
        }
        return result;
    }
}
public class RaycastCheckResult
{
    public bool HasObject;
    public float DistanceToObject;
}