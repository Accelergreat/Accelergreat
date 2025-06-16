namespace Accelergreat.Components;

internal interface IAccelergreatComponentDecorator : IAccelergreatComponent
{
    internal IAccelergreatComponent GetComponent();
    internal bool IsSingleton();
}