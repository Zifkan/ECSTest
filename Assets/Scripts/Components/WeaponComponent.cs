using Unity.Entities;
using UnityEngine;

public struct Weapon : IComponentData
{

}

public class WeaponComponent : ComponentDataWrapper<Weapon>
{
    
}
