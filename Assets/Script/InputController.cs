using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private float MouseSensitivity;
    [SerializeField] private float ObjectMaxDistance;


    [SerializeField] private PlayerMovementController _playerMovement;
    [SerializeField] private DragController _dragController;


    private Transform _cameraTransform;
    private Renderer _selectedItemRenderer;
    private DragAbleObject _dragAbleObject;
    private Vector3 _cameraRotationVector;
    private Vector3 _playerRotationVector;
    private float _xRotation;
    private float _mouseX;
    private float _mouseY;
    private float _movementX;
    private float _movementZ;
    private bool _itemSelected;
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
            TakeMovementVector();
            CheckForMouseInput();
            CheckForItems();

            _playerMovement.CameraLook(_cameraRotationVector, _playerRotationVector);
            _playerMovement.MovePlayer(_movementX, _movementZ);
        }
    }
    private void TakeMouseInput()
    {
        _mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _cameraRotationVector.x = _xRotation;
        _playerRotationVector = Vector3.up * _mouseX;

    }
    private void TakeMovementVector()
    {
        _movementX = Input.GetAxis("Horizontal");
        _movementZ = Input.GetAxis("Vertical");

    }
    private void CheckForMouseInput()
    {
        RaycastHit hit;
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        if (Input.GetMouseButtonDown(0))
        {

            if (Physics.Raycast(ray, out hit, ObjectMaxDistance))
            {
                //if (hit.collider.try(TagManager.GetTag(TagType.DragAble)))
                //{
                //    _dragController.TakeItem(hit.transform);
                //}

            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            //_dragController.ThrowItem();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, ObjectMaxDistance))
            {
                //if (hit.collider.CompareTag(TagManager.GetTag(TagType.Baby)))
                //{
                //    _dragController.TryUseOnBaby();

                //}
                //else
                //{
                //    _dragController.ReleaseItem();
                //}

            }
            else
            {
                //_dragController.ReleaseItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, ObjectMaxDistance))
            {
                //if (hit.collider.CompareTag(TagManager.GetTag(TagType.Baby)))
                //{
                //    _dragController.TryUseOnBaby();
                //}
            }
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
    private void CheckForItems()
    {
        RaycastHit hit;
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        if (Physics.Raycast(ray, out hit, ObjectMaxDistance))
        {
            if (hit.collider.TryGetComponent(out _dragAbleObject))
            {

            }




            if (hit.collider.CompareTag(""))
            {
                if (!_itemSelected)
                {
                    _itemSelected = true;
                    _dragAbleObject = hit.transform.GetComponent<DragAbleObject>();
                    //_selectedItemRenderer = _dragAbleObject.ItemRenderer;
                    //_selectedItemRenderer.sharedMaterial = _dragAbleObject.OutlinedMaterial;
                }
            }
            else
            {
                if (_itemSelected)
                {
                    _itemSelected = false;
                    //_selectedItemRenderer.sharedMaterial = _dragAbleObject.BasicMaterial;
                }
            }
        }

    }
}
