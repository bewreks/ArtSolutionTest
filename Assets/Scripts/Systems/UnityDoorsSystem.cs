using Components;
using Leopotam.EcsLite;
using Zenject;

namespace Systems
{
	public class UnityDoorsSystem : IEcsInitSystem
	{
		[Inject] private DoorsUnitySettings _doorsUnitySettings;

		public void Init(IEcsSystems systems)
		{
			var world       = systems.GetWorld();
			var buttons     = world.Filter<ButtonComponent>().End();
			var buttonsPool = world.GetPool<ButtonComponent>();
			var unityPool   = world.GetPool<UnityViewComponent>();
			foreach (var button in buttons)
			{
				ref var buttonComponent = ref buttonsPool.Get(button);

				if (buttonComponent.DoorEntity.Unpack(world, out var door))
				{
					ref var viewComponent = ref unityPool.Add(door);
					viewComponent.Transform = _doorsUnitySettings.Buttons[buttonComponent.Id].Door;
				}
			}
		}
	}
}