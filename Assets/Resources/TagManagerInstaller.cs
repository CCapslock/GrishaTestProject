using Zenject;

public class TagManagerInstaller : MonoInstaller<TagManagerInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<TagManager>().FromNew().AsSingle();
    }
}