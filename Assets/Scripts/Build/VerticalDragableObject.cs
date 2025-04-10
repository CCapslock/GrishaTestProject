using UnityEngine;

public class VerticalDragableObject : DragableObject
{
    private float _distanceToFloor;
    private float _distanceToObject;
    private bool _isCameraSeeFloor;
    private bool _isCameraSeeDragableObject;
    public override void DragObject(Transform cameraTransform, float distanceToPickUp)
    {
        CalculateCameraDistanceToFloor(cameraTransform, distanceToPickUp);
        _hasPlacedPosition = false;
        if (_isCameraSeeFloor)
        {
            CheckForFloor(cameraTransform);
        }
        if (CanStack)
        {
            CalculateCameraDistanceToDragableObject(cameraTransform, distanceToPickUp);
            if (_isCameraSeeDragableObject)
            {
                CheckForStackAbleObject(cameraTransform, distanceToPickUp);
            }
            else
            {
                _hasBottomObject = false;
            }
        }
        MoveDragableItem(cameraTransform, distanceToPickUp);
        CheckIfCanPlace();
    }
    private void CalculateCameraDistanceToFloor(Transform cameraTransform, float distanceToPickUp)
    {
        RaycastCheckResult temp = CheckForObjectByLayer(cameraTransform, _floorLayer, distanceToPickUp);

        _isCameraSeeFloor = temp.HasObject;
        _distanceToFloor = temp.DistanceToObject;
    }
    private void CalculateCameraDistanceToDragableObject(Transform cameraTransform, float distanceToPickUp)
    {
        RaycastCheckResult temp = CheckForObjectByLayer(cameraTransform, _dragableObjectLayer, distanceToPickUp);

        _distanceToObject = temp.DistanceToObject;
        _isCameraSeeDragableObject = temp.HasObject;
    }
    private void CheckForFloor(Transform cameraTransform)
    {
        _calculatedRaycastStartPosition = cameraTransform.position + cameraTransform.forward * _distanceToFloor;

        _calculatedRaycastStartPosition.y = _snapDistance;

        RaycastHit hit;
        Ray ray = new Ray(_calculatedRaycastStartPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, _snapDistance + 1f, _floorLayer))
        {
            _hasPlacedPosition = true;
            _calculatedPlacedPosition = hit.point;
        }
    }
    private void CheckForStackAbleObject(Transform cameraTransform, float distanceToPickUp)
    {
        _hasBottomObject = false;
        if (_isCameraSeeDragableObject)
        {
            _calculatedRaycastStartPosition = cameraTransform.position + cameraTransform.forward * _distanceToObject;
        }
        else
        {
            if (_isCameraSeeFloor)
            {
                _calculatedRaycastStartPosition = cameraTransform.position + cameraTransform.forward * _distanceToFloor;
            }
            else
            {
                _calculatedRaycastStartPosition = cameraTransform.position + cameraTransform.forward * distanceToPickUp;

            }
        }
        _calculatedRaycastStartPosition.y = _snapDistance;

        RaycastHit hit;
        Ray ray = new Ray(_calculatedRaycastStartPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, _snapDistance + 1f, _dragableObjectLayer))
        {
            if (hit.collider.TryGetComponent(out _bottomObject))
            {
                if (_bottomObject.CanStackOn)
                {
                    _hasPlacedPosition = true;
                    _hasBottomObject = true;
                    _calculatedPlacedPosition = hit.point + Vector3.up * _snapDiffrence;
                }
            }
        }
    }
}