using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "CollidersSettingsInstaller", menuName = "Installers/CollidersSettingsInstaller")]
public class CollidersSettingsInstaller : ScriptableObjectInstaller<CollidersSettingsInstaller>
{
	[SerializeField] private CollidersSettings collidersSettings;

	public override void InstallBindings()
	{
		Container.Bind<CollidersSettings>().FromInstance(collidersSettings);
	}
}

[Serializable]
public class CollidersSettings
{
	[SerializeField] private float playerRadius = 0.5f;
	[SerializeField] private float buttonRadius = 0.5f;

	public float PlayerRadius => playerRadius;
	public float ButtonRadius => buttonRadius;
}