using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSetup", menuName = "AProjectiles/ProjectileData")]
public class ProjectileDataObject : ScriptableObject, IProjData
{


	[SerializeField]
	public ProjectileData projD;


	public ProjectileData ProjInfo { get { return projD; } }
}
