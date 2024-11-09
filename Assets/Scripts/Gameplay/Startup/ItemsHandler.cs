using System.Collections.Generic;
using Bones.Gameplay.Flocks.Pulling;
using Bones.Gameplay.Items;
using Railcar.Time;
using Zenject;

namespace Bones.Gameplay.Startup
{
    public class ItemsHandler : BaseHandler
    {
        private readonly PullingContainer<HashSet<Item>, Item> _itemsContainer;
        
        public ItemsHandler(
            [Inject(Id = TimeID.Fixed)] IStopwatch stopwatch,
            [Inject] ItemsContainer itemsContainer)
            : base(stopwatch)
        {
            _itemsContainer = itemsContainer;
        }

        protected override void OnUpdated(float deltaTime)
        {
            _itemsContainer.Update(deltaTime);
        }
    }
}