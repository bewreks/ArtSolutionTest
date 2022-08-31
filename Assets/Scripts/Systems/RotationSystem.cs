using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class RotationSystem : IEcsRunSystem
	{
		[Inject] private SystemsHelper _systemsHelper;
		
		public void Run(IEcsSystems systems)
		{
			var world            = systems.GetWorld();
			var transformsFilter = world.Filter<TransformComponent>().Inc<RotateToComponent>().End();
			var rotationPool     = world.GetPool<RotateToComponent>();
			var stopRotationPool = world.GetPool<StopRotatingComponent>();
			var transformPool    = world.GetPool<TransformComponent>();

			foreach (var entity in transformsFilter)
			{
				ref var rotateTo  = ref rotationPool.Get(entity);
				ref var transform = ref transformPool.Get(entity);
				rotateTo.Time -= _systemsHelper.DeltaTime;

				if (rotateTo.Time <= 0)
				{
					transform.Rotation = rotateTo.FinalRotation;
					stopRotationPool.Add(entity);
				}
				else
				{
					transform.Rotation = Quaternion.Lerp(rotateTo.FinalRotation, rotateTo.FromRotation,
					                                     rotateTo.Time / rotateTo.TotalTime);
				}
			}
		}
	}
}