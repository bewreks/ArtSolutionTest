using System;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PointerSettingsInstaller", menuName = "Installers/PointerSettingsInstaller")]
public class PointerSettingsInstaller : ScriptableObjectInstaller<PointerSettingsInstaller>
{
	[SerializeField] private PointerSettings pointerSettings;

	public override void InstallBindings()
	{
		Container.Bind<PointerSettings>().FromInstance(pointerSettings).AsSingle();
	}
}

[Serializable]
public class PointerSettings
{
	[SerializeField] private LayerMask  mask;
	[SerializeField] private GameObject pointerPrefab;
	[SerializeField] private float      hidingAnimationTime;
	[SerializeField] private Vector3    startSize;
	[SerializeField] private Vector3    finalSize;

	public GameObject PointerPrefab       => pointerPrefab;
	public float      HidingAnimationTime => hidingAnimationTime;
	public Vector3    StartSize           => startSize;
	public Vector3    FinalSize           => finalSize;
	public LayerMask  Mask                => mask;
}