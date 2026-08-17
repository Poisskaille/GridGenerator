using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 25;
    public int height = 25;
    public float hexSize = 1f;

    [Header("Noise Settings")]
    public float scale = 0.1f;

    [Header("Hexagone")]
    [SerializeField]
    private GameObject _prefab;

    //[SerializeField]
    //private GameObject cloudPrefab;

    private Vector2Int start, exit;

    [Header("Temps")]
    public Material _yellow;
    public Material _black;
    public Material _orange; // Temp start
    public Material _pink; // Temp Exit

    // Changement a faire
    private Vector2 seed;

    // Debug
    //private int index = 0;
    void Start()
    {
        InitializeSeed();
        StartCoroutine(Generate());
    }

    private void InitializeSeed()
    {
        seed = new Vector2(23919, 86692);//new Vector2(Random.Range(0, 99999), Random.Range(0, 99999)); // ;
        Random.InitState(seed.GetHashCode());
        //Debug.Log(seed);
    }

    public void InitializeSeed(int seedValue)
    {
        // Redefinir la seed plus tard en simple int.
        //seed =  seedValue// new Vector2(23919,86692);
        Random.InitState(seed.GetHashCode());
        //Debug.Log(seed);
    }

    private IEnumerator Generate(int biomeID = 0)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        BiomeData biome = BiomesRegistery.Instance._biomes[biomeID];

        Dictionary<Vector2Int, (HexInfos hex, bool walkable)> allhexesTemp = new Dictionary<Vector2Int, (HexInfos, bool walkable)>();


        float hexWidth = hexSize * 1.732f;
        float hexHeight = hexSize * 2.0f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * hexWidth;
                float zPos = y * (hexHeight * 0.75f);

                if (y % 2 != 0)
                    xPos += hexWidth / 2f;

                float noise = Mathf.PerlinNoise(x * scale + seed.x, y * scale + seed.y);

                float yPos = 0;

                Vector3 pos = new Vector3(xPos, yPos, zPos);
                GameObject hex = Instantiate(_prefab, pos, Quaternion.identity, transform);

                hex.transform.parent = this.transform;
                hex.name = $"Hex {x}x{y}y";

                var hexInfo = hex.AddComponent<HexInfos>();
                var child = hexInfo.GetComponentInChildren<Transform>();
                hexInfo.center = child.gameObject;

                allhexesTemp.Add(new Vector2Int(x, y), (hexInfo, noise <= 0.5));
                PathfindingBFS.Instance._hexes.Add(new Vector2Int(x, y), hexInfo);

                yield return new WaitForSeconds(0.0001f);
            }
        }

        int nbBiome = Random.Range(1, 3);

        for (int i = 0; i < nbBiome; i++)
            allhexesTemp = GenerateSecundaryBiome(allhexesTemp, biome);

        allhexesTemp = GenerateMainBiome(allhexesTemp, biome);

        GenerateStartExit(allhexesTemp);

        allhexesTemp = CheckAllPath(allhexesTemp, exit);

        foreach (var coord in allhexesTemp)
        {
            if (coord.Value.walkable)
                GenerateEvent(coord.Value.hex);
            else
                GenerateObstacle(coord.Value.hex);  

                yield return new WaitForSeconds(0.0001f);
        }

        //GenerateFog(allhexesTemp);
        CleanFinaleList(allhexesTemp);
    }

    private void GenerateEvent(HexInfos _hex)
    {

        if (_hex._event != null) return;

        int currentRNG = Random.Range(0, 10);
        if (currentRNG < 2)
        {
            //_hex.gameObject.GetComponent<MeshRenderer>().material = _yellow;
            bool r = Random.value < 0.5f;
            _hex.SetEvent(r ? EventRegistery.instance.GetRandomMainEvent(): EventRegistery.instance.GetRandomSubEvent(), r ? _yellow : _black);
        }

    }

    private void GenerateObstacle(HexInfos _hex)
    {
        Vector3 posOffset = _hex.transform.position;
        _hex.transform.position = posOffset;

        var children = _hex.GetComponentsInChildren<Transform>().Skip(1).ToArray();

        int randomChild = Random.Range(0, children.Length);

        int totalWeight = 0;
        foreach(var _object in BiomesRegistery.Instance.GetBiome(_hex.biomeID)._obstaclesPrefab) 
            totalWeight += _object.weight;


        int randomWeight = Random.Range(0, totalWeight);
        GameObject newObstacle = null;
        foreach (var _object in BiomesRegistery.Instance.GetBiome(_hex.biomeID)._obstaclesPrefab)
        {
            randomWeight -= _object.weight;
            if (randomWeight <= 0)
            {
                newObstacle = _object.prefab;
                break;
            }
        }

        GameObject obstacle = Instantiate(newObstacle, children[randomChild]);
        obstacle.transform.localPosition = Vector3.zero;
        obstacle.transform.localRotation = Quaternion.Euler(obstacle.transform.localEulerAngles.x, Random.Range(0, 180f), obstacle.transform.localEulerAngles.z);
    }

    private Dictionary<Vector2Int, (HexInfos hex, bool walkable)> GenerateSecundaryBiome(Dictionary<Vector2Int, (HexInfos hex, bool walkable)> allhexesTemp, BiomeData mainBiome)
    {
        BiomeData newBiome = BiomesRegistery.Instance.GetRandomBiomes(mainBiome.GetBiomeID());

        Vector2Int coord = new Vector2Int();

        while (true)
        {
            coord.x = Random.Range(0, width);
            coord.y = Random.Range(0, height);

            if (allhexesTemp[coord].hex.biomeID == -1)
                break;
        }

        HexInfos hex = allhexesTemp[coord].hex;
        hex.biomeID = newBiome.GetBiomeID();

        if (allhexesTemp[coord].walkable)
            hex.gameObject.GetComponent<MeshRenderer>().material = newBiome._walkableMaterial;
        else
            hex.gameObject.GetComponent<MeshRenderer>().material = newBiome._nonWalkableMaterial;

        int index = -1;

        List<Vector2Int> allNeighbors = PathfindingBFS.Instance.GetNeighbors(coord);

        while (allNeighbors.Count > 0)
        {
            index++;
            foreach (Vector2Int neighbour in new List<Vector2Int>(allNeighbors))
            {
                HexInfos newHex = allhexesTemp[neighbour].hex;
                newHex.biomeID = newBiome.GetBiomeID();
                if (allhexesTemp[neighbour].walkable)
                    newHex.gameObject.GetComponent<MeshRenderer>().material = newBiome._walkableMaterial;
                else
                    newHex.gameObject.GetComponent<MeshRenderer>().material = newBiome._nonWalkableMaterial;

                List<Vector2Int> newNeighbors = PathfindingBFS.Instance.GetNeighbors(neighbour);
                allNeighbors.Remove(neighbour);
                foreach (Vector2Int nNeighbor in newNeighbors)
                {
                    if (!allhexesTemp.ContainsKey(nNeighbor)) continue;

                    float probability = 1f - (float)index / newBiome.biomeScale;
                    if (allhexesTemp[nNeighbor].hex.biomeID == -1 && Random.value < probability)
                    {
                        allhexesTemp[nNeighbor].hex.biomeID = newBiome.GetBiomeID();
                        allNeighbors.Add(nNeighbor);
                    }
                }
            }
        }

        return allhexesTemp;
    }

    private Dictionary<Vector2Int, (HexInfos hex, bool walkable)> GenerateMainBiome(Dictionary<Vector2Int, (HexInfos hex, bool walkable)> allhexesTemp, BiomeData biomeData)
    {
        foreach (var coord in allhexesTemp)
        {
            if (coord.Value.hex.biomeID != -1) continue;

            coord.Value.hex.biomeID = biomeData.GetBiomeID();

            if (coord.Value.walkable)
                coord.Value.hex.gameObject.GetComponent<MeshRenderer>().material = biomeData._walkableMaterial;
            else
                coord.Value.hex.gameObject.GetComponent<MeshRenderer>().material = biomeData._nonWalkableMaterial;

        }

        return allhexesTemp;
    }

    private void GenerateStartExit(Dictionary<Vector2Int, (HexInfos hex, bool walkable)> allhexesTemp)
    {
        Vector2Int coord = new Vector2Int();

        while (true)
        {
            coord.x = Random.Range(0, width);
            coord.y = Random.Range(0, height);

            if (allhexesTemp[coord].walkable && allhexesTemp[coord].hex._event == null)
            {
                allhexesTemp[coord].hex._event = EventRegistery.instance.GetMainEvent(0);
                allhexesTemp[coord].hex.gameObject.GetComponent<MeshRenderer>().material = _orange;
                start = coord;
                break;
            }
        }

        while (true)
        {
            coord.x = Random.Range(0, width);
            coord.y = Random.Range(0, height);

            if (allhexesTemp[coord].walkable
                && allhexesTemp[coord].hex._event == null
                && PathfindingBFS.Instance.PathFindingBfs(start, coord).Count >= 7)
            {
                allhexesTemp[coord].hex._event = EventRegistery.instance.GetMainEvent(1);
                allhexesTemp[coord].hex.gameObject.GetComponent<MeshRenderer>().material = _pink;
                exit = coord;
                break;
            }

        }
    }

    private Dictionary<Vector2Int, (HexInfos hex, bool walkable)> CheckAllPath(Dictionary<Vector2Int, (HexInfos hex, bool walkable)> allhexesTemp, Vector2Int exit)
    {
        Dictionary<Vector2Int, HexInfos> allWalkableHexes = new Dictionary<Vector2Int, HexInfos>();
        Dictionary<Vector2Int, bool> pathFound = new Dictionary<Vector2Int, bool>();

        foreach (Vector2Int coord in allhexesTemp.Keys)
        {
            if (allhexesTemp[coord].walkable)
                allWalkableHexes.Add(coord, allhexesTemp[coord].hex);
        }

        foreach (Vector2Int coord in allhexesTemp.Keys)
        {
            if (allhexesTemp[coord].walkable)
                pathFound.Add(coord, PathfindingBFS.Instance.PathFindingBfs(coord, exit, allWalkableHexes).Count > 0);
        }

        while (pathFound.Any(x => !x.Value))
        {
            foreach (var coord in new Dictionary<Vector2Int, bool>(pathFound))
            {
                if (coord.Value) continue;

                HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
                List<Vector2Int> frontier = new List<Vector2Int> { coord.Key };
                visited.Add(coord.Key);
                bool found = false;

                while (frontier.Count > 0 && !found)
                {
                    List<Vector2Int> nextFrontier = new List<Vector2Int>();

                    foreach (Vector2Int current in frontier)
                    {
                        foreach (Vector2Int neighbor in PathfindingBFS.Instance.GetNeighbors(current))
                        {
                            if (visited.Contains(neighbor)) continue;
                            visited.Add(neighbor);

                            if (pathFound.ContainsKey(neighbor) && pathFound[neighbor])
                            {
                                Dictionary<Vector2Int, HexInfos> path = PathfindingBFS.Instance.PathFindingBfs(coord.Key, neighbor);
                                foreach (Vector2Int _coord in path.Keys)
                                {
                                    if (allhexesTemp[_coord].hex._event == null)
                                        allhexesTemp[_coord].hex.gameObject.GetComponent<MeshRenderer>().material =
                                            BiomesRegistery.Instance.GetBiome(allhexesTemp[_coord].hex.biomeID)._walkableMaterial;
                                    var current2 = allhexesTemp[_coord];
                                    allhexesTemp[_coord] = (current2.hex, true);
                                    pathFound[_coord] = true;

                                    int randomNeighbor = Random.Range(1, 3);
                                    List<Vector2Int> currentNeighbors = PathfindingBFS.Instance.GetNeighbors(_coord);
                                    for (int i = 0; i < randomNeighbor; i++)
                                    {
                                        int currentRandom = Random.Range(0, currentNeighbors.Count);

                                        if (allhexesTemp[currentNeighbors[currentRandom]].hex._event == null)
                                            allhexesTemp[currentNeighbors[currentRandom]].hex.gameObject.GetComponent<MeshRenderer>().material =
                                                BiomesRegistery.Instance.GetBiome(allhexesTemp[currentNeighbors[currentRandom]].hex.biomeID)._walkableMaterial;
                                        var current3 = allhexesTemp[currentNeighbors[currentRandom]];
                                        allhexesTemp[currentNeighbors[currentRandom]] = (current3.hex, true);
                                    }
                                }
                                found = true;
                                break;
                            }

                            nextFrontier.Add(neighbor);
                        }
                        if (found) break;
                    }

                    frontier = nextFrontier;
                }
            }
        }
        return allhexesTemp;
    }

    private void CleanFinaleList(Dictionary<Vector2Int, (HexInfos hex, bool walkable)> allhexesTemp)
    {
        foreach (var coord in allhexesTemp)
        {
            if (!allhexesTemp[coord.Key].walkable)
                PathfindingBFS.Instance._hexes.Remove(coord.Key);
        }
    }

    /*private void GenerateFog(Dictionary<Vector2Int, (HexInfos hex, bool walkable)> allhexesTemp)
    {
        foreach (var coord in allhexesTemp) 
        {
            GameObject currentCloud = Instantiate(cloudPrefab, coord.Value.hex.gameObject.transform, coord.Value.hex);
            Vector3 offsetPos = Vector3.zero;
            offsetPos.y += 4;
            currentCloud.transform.localPosition = offsetPos;
        }
    }*/
}