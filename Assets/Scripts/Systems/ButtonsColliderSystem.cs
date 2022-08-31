using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class ButtonsColliderSystem : IEcsRunSystem
	{
		[Inject] private SystemsHelper _systemsHelper;

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			if (_systemsHelper.PlayerEntity.Unpack(world, out var playerEntity))
			{
				var eventsWorld = systems.GetWorld(SystemsHelper.Events);

				var transformPool = world.GetPool<TransformComponent>();
				var colliderPool  = world.GetPool<CircleColliderComponent>();
				var triggeredPool = world.GetPool<ButtonTriggered>();
				var buttonPool    = world.GetPool<ButtonComponent>();

				var enterEventPool = eventsWorld.GetPool<ButtonTriggerEnterEvent>();
				var exitEventPool  = eventsWorld.GetPool<ButtonTriggerExitEvent>();

				ref var playerTransform = ref transformPool.Get(playerEntity);
				ref var playerCollider  = ref colliderPool.Get(playerEntity);

#region ExitEvent

				var filter = world.Filter<ButtonComponent>()
				                  .Inc<CircleColliderComponent>()
				                  .Inc<TransformComponent>()
				                  .Inc<ButtonTriggered>().End();

				foreach (var buttonEntity in filter)
				{
					ref var transform = ref transformPool.Get(buttonEntity);
					ref var collider  = ref colliderPool.Get(buttonEntity);

					if (Vector3.Distance(transform.Position, playerTransform.Position) >
					    collider.Radius + playerCollider.Radius)
					{
						triggeredPool.Del(buttonEntity);

						var     triggerEvent = eventsWorld.NewEntity();
						ref var @event       = ref exitEventPool.Add(triggerEvent);
						ref var button       = ref buttonPool.Get(buttonEntity);
						@event.DoorEntity = button.DoorEntity;
					}
				}

#endregion

#region EnterEvent

				filter = world.Filter<ButtonComponent>()
				              .Inc<CircleColliderComponent>()
				              .Inc<TransformComponent>()
				              .Exc<ButtonTriggered>().End();

				foreach (var buttonEntity in filter)
				{
					ref var transform = ref transformPool.Get(buttonEntity);
					ref var collider  = ref colliderPool.Get(buttonEntity);

					if (Vector3.Distance(transform.Position, playerTransform.Position) <=
					    collider.Radius + playerCollider.Radius)
					{
						triggeredPool.Add(buttonEntity);

						var     triggerEvent = eventsWorld.NewEntity();
						ref var @event       = ref enterEventPool.Add(triggerEvent);
						ref var button       = ref buttonPool.Get(buttonEntity);
						@event.DoorEntity = button.DoorEntity;
					}
				}

#endregion
			}
		}
	}
}