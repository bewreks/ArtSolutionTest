using Components;
using Leopotam.EcsLite;

namespace Systems
{
	public class UnitySystem : IEcsRunSystem
	{
		public void Run(IEcsSystems systems)
		{
			var world           = systems.GetWorld();
			var transformFilter = world.Filter<TransformComponent>()
			                           .Inc<UnityViewComponent>()
			                           .Inc<MoveToComponent>().End();
			var transformPool   = world.GetPool<TransformComponent>();
			var unityViewPool   = world.GetPool<UnityViewComponent>();
			foreach (var entity in transformFilter)
			{
				ref var transform  = ref transformPool.Get(entity);
				ref var unityView = ref unityViewPool.Get(entity);
				unityView.Transform.position = transform.Position;
				unityView.Transform.rotation = transform.Rotation;
			}
		}
	}
}