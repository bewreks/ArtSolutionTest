using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class UnityInitPlayerSystem : IEcsInitSystem,
	                                     IEcsRunSystem
	{
		[Inject] private UnityPlayerSettings _playerSettings;
		[Inject] private SystemsHelper       _systemsHelper;

		public void Init(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			if (_systemsHelper.PlayerEntity.Unpack(world, out var player))
			{
				var     viewPool       = world.GetPool<UnityViewComponent>();
				var     playerViewPool = world.GetPool<PlayerViewComponent>();
				var     playerObject   = Object.Instantiate(_playerSettings.PlayerPrefab);
				ref var viewComponent  = ref viewPool.Add(player);
				viewComponent.Transform = playerObject.transform;
				ref var view = ref playerViewPool.Add(player);
				view.PlayerView = playerObject;
			}
		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			if (_systemsHelper.PlayerEntity.Unpack(world, out var player))
			{
				var eventsWorld = systems.GetWorld(SystemsHelper.Events);

				var playerViewPool  = world.GetPool<PlayerViewComponent>();
				var startMovingPool = eventsWorld.GetPool<StartMovingEvent>();
				var endMovingPool   = eventsWorld.GetPool<EndMovingEvent>();

				var filter = eventsWorld.Filter<StartMovingEvent>().End();

				foreach (var entity in filter)
				{
					ref var movingEvent = ref startMovingPool.Get(entity);

					if (movingEvent.MovedEntity.Equals(_systemsHelper.PlayerEntity))
					{
						ref var viewComponent = ref playerViewPool.Get(player);
						viewComponent.PlayerView.StartWalking();
					}
				}

				filter = eventsWorld.Filter<EndMovingEvent>().End();

				foreach (var entity in filter)
				{
					ref var movingEvent = ref endMovingPool.Get(entity);

					if (movingEvent.MovedEntity.Equals(_systemsHelper.PlayerEntity))
					{
						ref var viewComponent = ref playerViewPool.Get(player);
						viewComponent.PlayerView.StopWalking();
					}
				}
			}
		}
	}
}