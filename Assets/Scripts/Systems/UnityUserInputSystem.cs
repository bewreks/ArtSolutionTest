using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class UnityUserInputSystem : IEcsRunSystem
	{
		[Inject] private PointerSettings _pointerSettings;

		public void Run(IEcsSystems systems)
		{
			if (Input.GetMouseButtonDown(0))
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out var info, 100, _pointerSettings.Mask))
				{
					var     world      = systems.GetWorld(SystemsHelper.Events);
					var     eventPool  = world.GetPool<UserClickEvent>();
					var     clickEvent = world.NewEntity();
					ref var @event     = ref eventPool.Add(clickEvent);
					@event.ClickPosition = info.point;
				}
			}
		}
	}
}