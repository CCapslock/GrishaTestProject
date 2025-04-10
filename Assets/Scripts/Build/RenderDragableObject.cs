using EPOOutline;
using UnityEngine;
using Zenject;

public class RenderDragableObject : MonoBehaviour
{
    [SerializeField] private Outlinable _outlinable;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animatorController;
    [SerializeField] private Material _greenTransparentMaterial;
    [SerializeField] private Material _redTransparentMaterial;
    [SerializeField] private Color _outlineAvailableColor;
    [SerializeField] private Color _outlineBlockedColor;

    private ParticlesManager _particleManager;
    private Material _basicMaterial;
    private string _popVerticalTrigger = "PopVertical";
    private void Start()
    {
        _basicMaterial = _renderer.sharedMaterial;
    }
    [Inject]
    private void Construct(TagManager tagManager, ParticlesManager particlesManager)
    {
        _particleManager = particlesManager;
    }
    public void ReleaseObject()
    {
        _animatorController.SetTrigger(_popVerticalTrigger);
        _particleManager.MakeParticles(ParticleType.PuffParticle, transform.position);
        _renderer.sharedMaterial = _basicMaterial;
    }
    public void SelectObject(bool state)
    {
        _outlinable.enabled = state;
    }
    public void SetAvailabilityMaterial(bool isAvailable)
    {
        if (isAvailable)
        {
            _renderer.sharedMaterial = _greenTransparentMaterial;
        }
        else
        {
            _renderer.sharedMaterial = _redTransparentMaterial;
        }
    }
    public void SetAvailabilityOutline(bool isAvailable)
    {
        if (isAvailable)
        {
            _outlinable.FrontParameters.Color = _outlineBlockedColor;
            _outlinable.BackParameters.Color = _outlineBlockedColor;
        }
        else
        {
            _outlinable.FrontParameters.Color = _outlineAvailableColor;
            _outlinable.BackParameters.Color = _outlineAvailableColor;
        }
    }
}
