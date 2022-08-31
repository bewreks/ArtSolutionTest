using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PlayerSettingsInstaller", menuName = "Installers/PlayerSettingsInstaller")]
public class PlayerSettingsInstaller : ScriptableObjectInstaller<PlayerSettingsInstaller>
{
	[SerializeField] private PlayerSettings      playerSettings;
	[SerializeField] private UnityPlayerSettings unityPlayerSettings;

	public override void InstallBindings()
	{
		Container.Bind<PlayerSettings>().FromInstance(playerSettings);
		Container.Bind<UnityPlayerSettings>().FromInstance(unityPlayerSettings);
	}
}

[Serializable]
public class PlayerSettings
{
	[SerializeField] private float movementSpeed;
	[SerializeField] private float rotationSpeed;

	public float MovementSpeed => movementSpeed;
	public float RotationSpeed => rotationSpeed;
}

[Serializable]
public class UnityPlayerSettings
{
	[SerializeField] private PlayerView playerPrefab;

	public PlayerView PlayerPrefab => playerPrefab;
}