using UnityEngine;

public class HorizontalDragableObject : DragableObject
{
    private float _distanceToWall;
    private bool _isCameraSeeWall;
    public override void DragObject(Transform cameraTransform, float distanceToPickUp)
    {
        SetCalculateCameraDistanceToWall(cameraTransform, distanceToPickUp);
        _hasPlacedPosition = false; 
        if (_isCameraSeeWall)
        {
            CheckForWall(cameraTransform);
        }
        //if (_currentDragableObject.CanStack)
        //{
        //    CalculateCameraDistanceToDragableObject();
        //    if (_isCameraSeeDragableObject)
        //    {
        //        CheckForStackAbleObject();
        //    }
        //}
        MoveDragableItem(cameraTransform, distanceToPickUp);
        CheckIfCanPlace();
    }
    private void SetCalculateCameraDistanceToWall(Transform cameraTransform, float distanceToPickUp)
    {
        RaycastCheckResult temp = CheckForObjectByLayer(cameraTransform,_wallLayer, distanceToPickUp);

        _isCameraSeeWall = temp.HasObject;
        _distanceToWall = temp.DistanceToObject - 0.1f;
    }
    private void CheckForWall(Transform cameraTransform)
    {
        _calculatedRaycastStartPosition = cameraTransform.position + cameraTransform.forward * _distanceToWall;

        RaycastHit hit;
        Ray ray = new Ray(_calculatedRaycastStartPosition, transform.forward * -1f);

        if (Physics.Raycast(ray, out hit, _snapDistance + 1f, _wallLayer))
        {
            _hasPlacedPosition = true;
            _calculatedPlacedPosition = hit.point + transform.forward * _snapDiffrence;
        }
    }
}
