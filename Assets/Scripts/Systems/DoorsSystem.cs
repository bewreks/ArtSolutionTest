using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class DoorsSystem : IEcsInitSystem,
	                           IEcsRunSystem
	{
		[Inject] private DoorsButtonsPositions _positions;
		[Inject] private CollidersSettings     _collidersSettings;
		[Inject] private DoorsSettings         _doorsSettings;

		private EcsWorld                                _world;
		private EcsWorld                                _eventsWorld;
		private EcsPool<ButtonComponent>                _buttonsPool;
		private EcsPool<TransformComponent>             _transformPool;
		private EcsPool<DoorComponent>                  _doorPool;
		private EcsPool<MoveToComponent>                _movingPool;
		private EcsPool<ButtonTriggerEnterEvent>        _enterEventPool;
		private EcsPool<ButtonTriggerExitEvent>         _exitEventPool;
		private EcsPool<StartMovingEvent>               _movingEventPool;

		public void Init(IEcsSystems systems)
		{
			_world       = systems.GetWorld();
			_eventsWorld = systems.GetWorld(SystemsHelper.Events);

			_buttonsPool   = _world.GetPool<ButtonComponent>();
			_doorPool      = _world.GetPool<DoorComponent>();
			_transformPool = _world.GetPool<TransformComponent>();
			_movingPool    = _world.GetPool<MoveToComponent>();

			_enterEventPool  = _eventsWorld.GetPool<ButtonTriggerEnterEvent>();
			_exitEventPool   = _eventsWorld.GetPool<ButtonTriggerExitEvent>();
			_movingEventPool = _eventsWorld.GetPool<StartMovingEvent>();

			var colliderPool = _world.GetPool<CircleColliderComponent>();

			for (var i = 0; i < _positions.ButtonsPositions.Length; i++)
			{
				var buttonEntity = _world.NewEntity();
				var doorEntity   = _world.NewEntity();

				ref var button    = ref _buttonsPool.Add(buttonEntity);
				ref var transform = ref _transformPool.Add(buttonEntity);
				ref var collider  = ref colliderPool.Add(buttonEntity);

				button.Id          = i;
				button.DoorEntity  = _world.PackEntity(doorEntity);
				transform.Rotation = Quaternion.identity;
				transform.Position = _positions.ButtonsPositions[i];
				collider.Radius    = _collidersSettings.ButtonRadius;

				_doorPool.Add(doorEntity);
				transform          = ref _transformPool.Add(doorEntity);
				transform.Rotation = Quaternion.identity;
				transform.Position = _positions.DoorsPositions[i];
			}
		}

		public void Run(IEcsSystems systems)
		{
			var filter = _eventsWorld.Filter<ButtonTriggerEnterEvent>().End();

			foreach (var eventEntity in filter)
			{
				ref var @event = ref _enterEventPool.Get(eventEntity);

				if (@event.DoorEntity.Unpack(_world, out var door))
				{
					ref var transform = ref _transformPool.Get(door);

					if (_movingPool.Has(door))
					{
						SetMove(ref _movingPool.Get(door), ref transform);
					}
					else
					{
						SetMove(ref _movingPool.Add(door), ref transform);
					}

					var     movingEventEntity        = _eventsWorld.NewEntity();
					ref var startMovingEvent = ref _movingEventPool.Add(movingEventEntity);
					startMovingEvent.MovedEntity = @event.DoorEntity;
				}
			}

			filter = _eventsWorld.Filter<ButtonTriggerExitEvent>().End();
			
			foreach (var eventEntity in filter)
			{
				ref var @event = ref _exitEventPool.Get(eventEntity);
				
				if (@event.DoorEntity.Unpack(_world, out var door))
				{
					if (_movingPool.Has(door))
					{
						_movingPool.Del(door);
					}
				}
			}

			void SetMove(ref MoveToComponent    moveTo,
			             ref TransformComponent transform)
			{
				moveTo.From     = transform.Position;
				moveTo.Position = new Vector3(transform.Position.x, -_doorsSettings.DoorHeight, transform.Position.z);
				var length		= (moveTo.From - moveTo.Position).magnitude / _doorsSettings.DoorHeight;

				moveTo.TotalTime = moveTo.Time = length * _doorsSettings.DoorOpenAnimationTime;
			}
		}
	}
}