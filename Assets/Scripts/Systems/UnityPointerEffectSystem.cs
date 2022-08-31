using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class UnityPointerEffectSystem : IEcsRunSystem,
	                                        IEcsInitSystem
	{
		[Inject] private PointerSettings _pointerSettings;
		[Inject] private SystemsHelper   _systemsHelper;

		private int                                      _pointerEntity;
		private EcsWorld                                 _eventsWorld;
		private EcsPool<PointerViewComponent>            _pointerViewPool;
		private EcsPool<PointerAnimationEffectComponent> _animatedPool;
		private EcsPool<UserClickEvent>                  _eventPool;

		public void Init(IEcsSystems systems)
		{
			_eventsWorld = systems.GetWorld(SystemsHelper.Events);
			_eventPool   = _eventsWorld.GetPool<UserClickEvent>();

			var world = systems.GetWorld();
			_pointerEntity   = world.NewEntity();
			_pointerViewPool = world.GetPool<PointerViewComponent>();
			_animatedPool    = world.GetPool<PointerAnimationEffectComponent>();

			ref var viewComponent = ref _pointerViewPool.Add(_pointerEntity);
			viewComponent.Pointer            = Object.Instantiate(_pointerSettings.PointerPrefab).transform;
			viewComponent.Pointer.localScale = _pointerSettings.FinalSize;
		}

		public void Run(IEcsSystems systems)
		{
			var filter = _eventsWorld.Filter<UserClickEvent>().End();
			foreach (var entity in filter)
			{
				ref var clickEvent  = ref _eventPool.Get(entity);
				ref var pointerView = ref _pointerViewPool.Get(_pointerEntity);
				pointerView.Pointer.position   = clickEvent.ClickPosition;
				pointerView.Pointer.localScale = Vector3.one;
				pointerView.AnimationTime      = _pointerSettings.HidingAnimationTime;
				if (!_animatedPool.Has(_pointerEntity))
				{
					_animatedPool.Add(_pointerEntity);
				}
			}

			if (_animatedPool.Has(_pointerEntity))
			{
				ref var pointerView = ref _pointerViewPool.Get(_pointerEntity);
				pointerView.AnimationTime -= _systemsHelper.DeltaTime;
				pointerView.Pointer.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,
				                                              pointerView.AnimationTime /
				                                              _pointerSettings.HidingAnimationTime);
				if (pointerView.AnimationTime <= 0)
				{
					_animatedPool.Del(_pointerEntity);
				}
			}
		}
	}
}