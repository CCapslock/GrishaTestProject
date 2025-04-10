using UnityEngine;
using System.Collections.Generic;

public class DragController : MonoBehaviour
{
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private float _objectSeeDistance;
    [SerializeField] private float _pickUpDistance;
    [SerializeField] private float _objectRotationSpeed;

    private List<DragableObject> _rotateDragableObjects = new List<DragableObject>();
    private RaycastHit _hit;
    private DragableObject _raycastedDragAbleObject;
    private DragableObject _currentDragableObject;
    private bool _itemPickedUp;
    private bool _itemSelected;

    private void Update()
    {
        if (_itemPickedUp)
        {
            _currentDragableObject.DragObject(_playerCamera, _pickUpDistance);
        }
        else
        {
            CheckForItems();
        }
        RotateObjects();
    }
    private void RotateObjects()
    {
        for (int i = _rotateDragableObjects.Count - 1; i >= 0; i--)
        {
            if (_rotateDragableObjects[i].NeedToRotate)
            {
                _rotateDragableObjects[i].RotateObject(_objectRotationSpeed);
            }
            else
            {
                _rotateDragableObjects.RemoveAt(i);
            }
        }
    }
    public void PickUpItem(DragableObject dragableObject)
    {
        if (!_itemPickedUp)
        {
            _itemSelected = false;
            _currentDragableObject = dragableObject;
            _currentDragableObject.PickUpObject();
            _itemPickedUp = true;
        }
    }
    private void PlaceItem()
    {
        _itemPickedUp = false;
        _currentDragableObject.PlaceObject();

    }
    public void TryRotateObject(Vector2 scrollInput)
    {
        if (_itemPickedUp)
        {
            _currentDragableObject.StartRotateObject(scrollInput);
            if (!_rotateDragableObjects.Contains(_currentDragableObject))
            {
                _rotateDragableObjects.Add(_currentDragableObject);
            }
        }
    }
    public void TryLMBInput()
    {
        if (_itemPickedUp)
        {
            if (_currentDragableObject.CanBePlaced)
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