using UnityEngine;

public class HexInfos : MonoBehaviour
{
    public EventData _event = null;
    public int biomeID = -1;

    public GameObject center;
    public MeshRenderer _renderer;

    public bool walkable = false;

    void Start() 
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void AddOutline(Material mat) 
    {
        Material[] mats = _renderer.materials;
        mats[1] = mat;
        _renderer.materials = mats;
    }

    public void SetEvent(EventData eventData, Material mat) 
    {
        _event = eventData;
        _renderer.material = mat;
        //if (id >= 0) SpawnObstacle();
    }

    private void SpawnObstacle() 
    {
    }
}
