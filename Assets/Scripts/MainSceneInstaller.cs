using System;
using Leopotam.EcsLite;
using Systems;
using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
{
	[SerializeField] private DoorsUnitySettings doorsUnitySettings;

	private EcsWorld   _ecsWorld;
	private EcsSystems _ecsSystems;

	private SystemsHelper _systemsHelper;

	public override void InstallBindings()
	{
		_systemsHelper = new SystemsHelper();
		
		Container.Bind<SystemsHelper>().FromInstance(_systemsHelper).AsSingle();
		
		Container.Bind<DoorsUnitySettings>().FromInstance(doorsUnitySettings).AsSingle();
		var doorsPositions        = new Vector3[doorsUnitySettings.Buttons.Length];
		var buttonsPositions      = new Vector3[doorsUnitySettings.Buttons.Length];
		for (var i = 0; i < doorsUnitySettings.Buttons.Length; i++)
		{
			doorsPositions[i]   = doorsUnitySettings.Buttons[i].Door.transform.position;
			buttonsPositions[i] = doorsUnitySettings.Buttons[i].transform.position;
		}
		var doorsButtonsPositions = new DoorsButtonsPositions(doorsPositions, buttonsPositions);
		Container.Bind<DoorsButtonsPositions>().FromInstance(doorsButtonsPositions).AsSingle();
	}

	public override void Start()
	{
		base.Start();
		_ecsWorld   = new EcsWorld();
		_ecsSystems = new EcsSystems(_ecsWorld);

		var deleteEventsSystem = new DeleteEventsSystem();

		var eventsWorld = new EcsWorld();
		eventsWorld.AddEventListener(deleteEventsSystem);
		
		_ecsSystems
			.AddWorld(eventsWorld, SystemsHelper.Events)
			.Add(CreateInjectedSystem<UnityUserInputSystem>())
			.Add(CreateInjectedSystem<UnityPointerEffectSystem>())
			.Add(CreateInjectedSystem<PlayerSystem>())
			.Add(CreateInjectedSystem<RotationSystem>())
			.Add(CreateInjectedSystem<MovingSystem>())
			.Add(CreateInjectedSystem<UnityInitPlayerSystem>())
			.Add(CreateInjectedSystem<ButtonsColliderSystem>())
			.Add(CreateInjectedSystem<DoorsSystem>())
			.Add(CreateInjectedSystem<UnityDoorsSystem>())
			.Add(CreateInjectedSystem<UnityCameraSystem>())
			.Add(new UnitySystem())
			.Add(deleteEventsSystem)
			.Init();

		T CreateInjectedSystem<T>()
		where T : new()
		{
			var system = new T();
			Container.Inject(system);
			return system;
		}
	}

	private void Update()
	{
		_systemsHelper.DeltaTime = Time.deltaTime;
		_ecsSystems?.Run();
	}

	private void OnDestroy()
	{
		_ecsSystems?.Destroy();
		_ecsWorld?.Destroy();
		_ecsSystems = null;
		_ecsWorld   = null;
	}
}

[Serializable]
public class DoorsUnitySettings
{
	[SerializeField] private ButtonView[] buttons;
	public                   ButtonView[] Buttons => buttons;
}

public class DoorsButtonsPositions
{
	private readonly Vector3[] _doorsPositions;
	private readonly Vector3[] _buttonsPositions;

	public Vector3[] DoorsPositions   => _doorsPositions;
	public Vector3[] ButtonsPositions => _buttonsPositions;

	public DoorsButtonsPositions(Vector3[] doorsPositions, Vector3[] buttonsPositions)
	{
		_doorsPositions        = doorsPositions;
		_buttonsPositions = buttonsPositions;
	}
}