using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Vector3 CameraCrawlPosition;
    [SerializeField] private float Speed;
    [SerializeField] private float CrawlSpeed;
    [SerializeField] private float CameraSpeedToCrawl;

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
        _cameraStartPosition = CameraTransform.localPosition;
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
        CameraTransform.localPosition = Vector3.MoveTowards(CameraTransform.localPosition, _cameraDestanationPosition, CameraSpeedToCrawl);
        if (CameraTransform.localPosition == _cameraDestanationPosition)
            _needToMoveCamera = false;
    }
    public void CameraLook(Vector3 cameraRotation, Vector3 playerRotation)
    {
        CameraTransform.localRotation = Quaternion.Euler(cameraRotation);
        PlayerTransform.Rotate(playerRotation);
    }
    public void MovePlayer(float xInput, float zInput)
    {
        _playerMovementVector = PlayerTransform.right * xInput + PlayerTransform.forward * zInput;
        _playerVelocityVector.y += -9.81f * Time.deltaTime;

        if (_isCrawl)
            _characterController.Move(_playerMovementVector * CrawlSpeed * Time.deltaTime);
        else
            _characterController.Move(_playerMovementVector * Speed * Time.deltaTime);

        _characterController.Move(_playerMovementVector * Time.deltaTime);
    }
    public void StartCrawl()
    {
        _cameraDestanationPosition = CameraCrawlPosition;
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
