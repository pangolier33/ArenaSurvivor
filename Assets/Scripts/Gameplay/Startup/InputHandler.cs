using Bones.Gameplay.Inputs;
using Bones.Gameplay.Players;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Startup
{
    public sealed class InputHandler : BaseHandler
    {
        private readonly Player _player;
        private readonly IDirectionalInput _input;

        private InputHandler(
            [Inject(Id = TimeID.Fixed)] IStopwatch stopwatch,
            [Inject] Player player,
            [Inject] IDirectionalInput input
            ) : base(stopwatch)
        {
            _player = player;
            _input = input;
        }

        protected override void OnUpdated(float deltaTime)
        {
            _player.Move(_input.Direction, _input.JoySpeedModifier, deltaTime);
        }
    }
}