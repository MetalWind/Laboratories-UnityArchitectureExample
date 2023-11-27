using Zenject;

namespace Laboratory.Core
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<TypeObjFactory>().AsSingle().NonLazy();
            Container.Bind<DataStore>().AsSingle().NonLazy();
            Container.Bind<LabCenter>().AsSingle().NonLazy();
        }

        public virtual void BindStateMachine() { } 
    }
}