using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity;


    [SerializeField] private PlayerMovementController _playerMovement;
    [SerializeField] private DragController _dragController;


    private Transform _cameraTransform;
    private Vector3 _cameraRotationVector;
    private Vector3 _playerRotationVector;
    private Vector2 _mouseScroll;
    private float _xRotation;
    private float _mouseX;
    private float _mouseY;
    private float _movementX;
    private float _movementZ;
    private bool _isGameStarted;
    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _cameraRotationVector = new Vector3();
        StartInput();

    }
    public void StartInput()
    {
        _isGameStarted = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void StopInput()
    {
        _isGameStarted = false;
        Cursor.lockState = CursorLockMode.None;
    }


    private void Update()
    {
        if (_isGameStarted)
        {
            TakeMouseInput();
            TakeWheelInput();
            TakeMovementVector();
            CheckForMouseInput();

            _playerMovement.CameraLook(_cameraRotationVector, _playerRotationVector);
            _playerMovement.MovePlayer(_movementX, _movementZ);
        }
    }
    private void TakeMouseInput()
    {
        _mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _cameraRotationVector.x = _xRotation;
        _playerRotationVector = Vector3.up * _mouseX;

    }
    private void TakeWheelInput()
    {

        _mouseScroll = Input.mouseScrollDelta;

        _dragController.TryRotateObject(_mouseScroll);

    }
    private void TakeMovementVector()
    {
        _movementX = Input.GetAxis("Horizontal");
        _movementZ = Input.GetAxis("Vertical");

    }
    private void CheckForMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _dragController.TryLMBInput();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _playerMovement.StartCrawl();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _playerMovement.StopCrawl();
        }
    }
}
