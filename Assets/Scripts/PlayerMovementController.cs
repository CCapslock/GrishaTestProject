using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Vector3 _cameraCrawlPosition;
    [SerializeField] private float _speed;
    [SerializeField] private float _crawlSpeed;
    [SerializeField] private float _cameraSpeedToCrawl;

    private Vector3 _playerMovementVector;
    private Vector3 _playerVelocityVector;
    private Vector3 _cameraStartPosition;
    private Vector3 _cameraDestanationPosition;
    private bool _needToMoveCamera;
    private bool _isCrawl;


    private void Start()
    {
        _playerMovementVector = new Vector3();
        _playerVelocityVector = new Vector3();
        _cameraStartPosition = _cameraTransform.localPosition;
    }
    private void FixedUpdate()
    {
        if (_needToMoveCamera)
        {
            MoveCamera();
        }
    }
    private void MoveCamera()
    {
        _cameraTransform.localPosition = Vector3.MoveTowards(_cameraTransform.localPosition, _cameraDestanationPosition, _cameraSpeedToCrawl);
        if (_cameraTransform.localPosition == _cameraDestanationPosition)
            _needToMoveCamera = false;
    }
    public void CameraLook(Vector3 cameraRotation, Vector3 playerRotation)
    {
        _cameraTransform.localRotation = Quaternion.Euler(cameraRotation);
        _playerTransform.Rotate(playerRotation);
    }
    public void MovePlayer(float xInput, float zInput)
    {
        _playerMovementVector = _playerTransform.right * xInput + _playerTransform.forward * zInput;

        if (_isCrawl)
            _characterController.Move(_playerMovementVector * _crawlSpeed * Time.deltaTime);
        else
            _characterController.Move(_playerMovementVector * _speed * Time.deltaTime);

    }
    public void StartCrawl()
    {
        _cameraDestanationPosition = _cameraCrawlPosition;
        _needToMoveCamera = true;
        _isCrawl = true;
    }
    public void StopCrawl()
    {
        _cameraDestanationPosition = _cameraStartPosition;
        _needToMoveCamera = true;
        _isCrawl = false;
    }
    private void Move()
    {

    }
}
