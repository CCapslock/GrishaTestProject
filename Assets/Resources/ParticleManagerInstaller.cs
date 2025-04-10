using UnityEngine;
using Zenject;

public class ParticleManagerInstaller : MonoInstaller
{
    public GameObject Managers;
    public override void InstallBindings()
    {
        Container.Bind<ParticlesManager>().FromComponentInHierarchy(Managers).AsSingle();
    }
}