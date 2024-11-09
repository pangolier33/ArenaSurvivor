using System;
using UniRx;

namespace Bones.Gameplay.Events.Callbacks.Templates.Base
{
    internal interface ICallbackTemplate
    {
        IDisposable Subscribe(IMessageReceiver broker);
    }
}