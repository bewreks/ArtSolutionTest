using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class MovingSystem : IEcsRunSystem
	{
		[Inject] private SystemsHelper _systemsHelper;
		
		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();
			var transformsFilter = world.Filter<TransformComponent>().Inc<MoveToComponent>().Exc<RotateToComponent>()
			                            .End();
			var movingPool     = world.GetPool<MoveToComponent>();
			var stopMovingPool = world.GetPool<StopMovingComponent>();
			var transformPool  = world.GetPool<TransformComponent>();

			foreach (var entity in transformsFilter)
			{
				ref var moveTo    = ref movingPool.Get(entity);
				ref var transform = ref transformPool.Get(entity);
				moveTo.Time -= _systemsHelper.DeltaTime;

				if (moveTo.Time <= 0)
				{
					var eventsWorld = systems.GetWorld(SystemsHelper.Events);
					var eventsPool  = eventsWorld.GetPool<EndMovingEvent>();
					var eventEntity = eventsWorld.NewEntity();
					transform.Position = moveTo.Position;
					stopMovingPool.Add(entity);
					ref var endMovingEvent = ref eventsPool.Add(eventEntity);
					endMovingEvent.MovedEntity = world.PackEntity(entity);
				}
				else
				{
					transform.Position = Vector3.Lerp(moveTo.Position, moveTo.From,
					                                  moveTo.Time / moveTo.TotalTime);
				}
			}
		}
	}
}