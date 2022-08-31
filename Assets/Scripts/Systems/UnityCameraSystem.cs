using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class UnityCameraSystem : IEcsRunSystem
	{
		[Inject] private SystemsHelper _systemsHelper;

		private Vector3 _cameraBasePosition;

		public UnityCameraSystem()
		{
			_cameraBasePosition = Camera.main.transform.position;
		}

		public void Run(IEcsSystems systems)
		{
			var world = systems.GetWorld();

			if (_systemsHelper.PlayerEntity.Unpack(world, out var player))
			{
				var     transformPool = world.GetPool<TransformComponent>();
				ref var transform     = ref transformPool.Get(player);
				Camera.main.transform.position = _cameraBasePosition + transform.Position;
			}
		}
	}
}