using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "DoorsSettingsInstaller", menuName = "Installers/DoorsSettingsInstaller")]
public class DoorsSettingsInstaller : ScriptableObjectInstaller<DoorsSettingsInstaller>
{
	[SerializeField] private DoorsSettings doorsSettings;

	public override void InstallBindings()
	{
		Container.Bind<DoorsSettings>().FromInstance(doorsSettings);
	}
}

[Serializable]
public class DoorsSettings
{
	[SerializeField] private float doorHeight;
	[SerializeField] private float doorOpenAnimationTime;

	public float DoorHeight        => doorHeight;
	public float DoorOpenAnimationTime => doorOpenAnimationTime;
}