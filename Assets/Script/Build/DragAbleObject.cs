using UnityEngine;
using EPOOutline;

public class DragAbleObject : MonoBehaviour
{
    [SerializeField] private Outlinable _outlinable;
    [SerializeField] private Collider _collider;
    [SerializeField] private bool _stackAble;

    public bool IsPickedUp { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsStackAble => _stackAble;

    private void Start()
    {
        SelectObject(false);
    }
    public void SelectObject(bool state)
    {
        IsSelected = state;
        _outlinable.enabled = state;
    }
    public void PickUpObject()
    {
        _collider.enabled = false;
        IsPickedUp = true;
        if (IsSelected)
        {
            SelectObject(false);
        }
    }
    public void ReleaseObject()
    {
        _collider.enabled = true;
        IsPickedUp = false;
    }
}
