using Components;
using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Systems
{
	public class PlayerSystem : IEcsInitSystem,
	                            IEcsRunSystem
	{
		[Inject] private SystemsHelper     _systemsHelper;
		[Inject] private PlayerSettings    _playerSettings;
		[Inject] private CollidersSettings _collidersSettings;

		private EcsWorld                    _world;
		private EcsWorld                    _eventsWorld;
		private EcsPool<TransformComponent> _transformPool;
		private EcsPool<MoveToComponent>    _movingPool;
		private EcsPool<RotateToComponent>  _rotatingPool;
		private EcsPool<UserClickEvent>     _eventPool;
		private EcsPool<StartMovingEvent>   _startMovingEventsPool;

		public void Init(IEcsSystems systems)
		{
			_world       = systems.GetWorld();
			_eventsWorld = systems.GetWorld(SystemsHelper.Events);

			_eventPool             = _eventsWorld.GetPool<UserClickEvent>();
			_startMovingEventsPool = _eventsWorld.GetPool<StartMovingEvent>();

			_rotatingPool  = _world.GetPool<RotateToComponent>();
			_movingPool    = _world.GetPool<MoveToComponent>();
			_transformPool = _world.GetPool<TransformComponent>();

			var colliderPool = _world.GetPool<CircleColliderComponent>();

			var     player    = _world.NewEntity();
			ref var transform = ref _transformPool.Add(player);
			ref var collider  = ref colliderPool.Add(player);

			_systemsHelper.PlayerEntity = _world.PackEntity(player);

			transform.Position = Vector3.zero;
			transform.Rotation = Quaternion.identity;
			collider.Radius    = _collidersSettings.PlayerRadius;
		}

		public void Run(IEcsSystems systems)
		{
			if (_systemsHelper.PlayerEntity.Unpack(_world, out var player))
			{
				var filter = _eventsWorld.Filter<UserClickEvent>().End();

				foreach (var entity in filter)
				{
					ref var pointerPosition = ref _eventPool.Get(entity);

					ref var transform    = ref _transformPool.Get(player);
					var     targetVector = pointerPosition.ClickPosition - transform.Position;

					if (_rotatingPool.Has(player))
					{
						SetRotation(ref _rotatingPool.Get(player),
						            ref transform,
						            ref targetVector);
					}
					else
					{
						SetRotation(ref _rotatingPool.Add(player),
						            ref transform,
						            ref targetVector);
					}

					if (_movingPool.Has(player))
					{
						SetMove(ref _movingPool.Get(player),
						        ref transform,
						        ref pointerPosition,
						        ref targetVector);
					}
					else
					{
						SetMove(ref _movingPool.Add(player),
						        ref transform,
						        ref pointerPosition,
						        ref targetVector);
					}

					var eventEntity      = _eventsWorld.NewEntity();
					ref var startMovingEvent = ref _startMovingEventsPool.Add(eventEntity);
					startMovingEvent.MovedEntity = _systemsHelper.PlayerEntity;

					_eventPool.Del(entity);
				}
			}

			void SetRotation(ref RotateToComponent  rotateTo,
			                 ref TransformComponent transform,
			                 ref Vector3            targetVector)
			{
				var forwardVector = Vector3.forward;
				var targetAngle   = Vector3.SignedAngle(targetVector, forwardVector, Vector3.down);

				rotateTo.FromRotation  = transform.Rotation;
				rotateTo.FinalRotation = Quaternion.Euler(0, targetAngle, 0);
				var rotationLength = Mathf.Abs(targetAngle - transform.Rotation.eulerAngles.y) % 360;

				if (rotationLength >= 180)
				{
					rotationLength = Mathf.Abs(rotationLength - 360);
				}

				rotateTo.TotalTime = rotateTo.Time = rotationLength * _playerSettings.RotationSpeed;
			}

			void SetMove(ref MoveToComponent    moveTo,
			             ref TransformComponent transform,
			             ref UserClickEvent     pointerPosition,
			             ref Vector3            targetVector)
			{
				moveTo.From      = transform.Position;
				moveTo.Position  = pointerPosition.ClickPosition;
				moveTo.TotalTime = moveTo.Time = targetVector.magnitude * _playerSettings.MovementSpeed;
			}
		}
	}
}