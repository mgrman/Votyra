using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class EditableMatrixImageInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {

            Container.BindInterfacesAndSelfTo<InitialStateSetter2f>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<ImageConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<InitialImageConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<EditableMatrixImage2f>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<EditableMatrixMask2e>()
                .AsSingle();
        }
    }
}