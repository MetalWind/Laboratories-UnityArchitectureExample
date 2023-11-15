using Zenject;

namespace Laboratory.Core
{
    public abstract class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<TypeObjFactory>().AsSingle().NonLazy();
            Container.Bind<DataStore>().AsSingle().NonLazy();
            Container.Bind<LabCenter>().AsSingle().NonLazy();
        }

        public abstract void BindStateMachine(); 
    }
}