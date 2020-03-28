using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class EditableMatrixImageInstaller2 : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AggregatableImage2f>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<AggregatableMask2e>()
                .AsSingle();
        }
    }
}