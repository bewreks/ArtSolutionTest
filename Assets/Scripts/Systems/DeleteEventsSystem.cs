using System.Collections.Generic;
using Components;
using Leopotam.EcsLite;

namespace Systems
{
	public class DeleteEventsSystem : IEcsRunSystem,
	                                  IEcsWorldEventListener
	{
		private List<int> _events = new();

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld(SystemsHelper.Events);

			foreach (var eventEntity in _events)
			{
				world.DelEntity(eventEntity);
			}

			_events.Clear();

			world = systems.GetWorld();
			var rotationPool     = world.GetPool<RotateToComponent>();
			var stopRotationPool = world.GetPool<StopRotatingComponent>();
			var movingPool       = world.GetPool<MoveToComponent>();
			var stopMovingPool   = world.GetPool<StopMovingComponent>();

			var filter = world.Filter<RotateToComponent>().Inc<StopRotatingComponent>().End();

			foreach (var entity in filter)
			{
				rotationPool.Del(entity);
				stopRotationPool.Del(entity);
			}

			filter = world.Filter<MoveToComponent>().Inc<StopMovingComponent>().End();

			foreach (var entity in filter)
			{
				movingPool.Del(entity);
				stopMovingPool.Del(entity);
			}
		}

		public void OnEntityCreated(int entity)
		{
			_events.Add(entity);
		}

		public void OnEntityChanged(int entity) { }

		public void OnEntityDestroyed(int entity) { }

		public void OnFilterCreated(EcsFilter filter) { }

		public void OnWorldResized(int newSize) { }

		public void OnWorldDestroyed(EcsWorld world) { }
	}
}