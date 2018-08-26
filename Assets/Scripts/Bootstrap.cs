using Unity.Rendering;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public static MeshInstanceRenderer BulletRenderer;

    [SerializeField] private MeshInstanceRenderer _bulletRenderer;


    private void Awake()
    {
        BulletRenderer = _bulletRenderer;
    }
}