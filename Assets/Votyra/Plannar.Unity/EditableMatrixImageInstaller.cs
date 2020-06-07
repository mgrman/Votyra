using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class EditableMatrixImageInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<InvalidatableEditableMatrixImage2f>()
                .AsSingle();
        }
    }
}
