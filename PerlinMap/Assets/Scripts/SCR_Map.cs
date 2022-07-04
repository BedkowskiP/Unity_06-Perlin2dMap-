using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Map : MonoBehaviour
{
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject[] prefabs_green;

    int map_width = 400;
    int map_height = 400;

    List<List<int>> noise_grid = new List<List<int>>();
    List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    //4-20
    public float magnification = 18.0f;
    public float x_offset = 0;
    public float y_offset = 0;

    void Start()
    {
        CreateTileset();
        CreateTileGroups();
        x_offset = Random.Range(-100f, 100f);
        y_offset = Random.Range(-100f, 100f);
        GenerateMap(x_offset, y_offset);
    }

    void CreateTileset() 
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefabs_green[0]); //water
        tileset.Add(1, prefabs_green[1]); //plains
        tileset.Add(2, prefabs_green[2]); //forest
        tileset.Add(3, prefabs_green[3]); //hills
        tileset.Add(4, prefabs_green[4]); //mountains
    }

    void CreateTileGroups() 
    {
        tile_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset) 
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = this.gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0f, 0f, 0f);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }

    void GenerateMap(float offset_x, float offset_y) 
    {
        for (int x = 0; x < map_width; x++) 
        {
            noise_grid.Add(new List<int>());
            tile_grid.Add(new List<GameObject>());
            for (int y = 0; y < map_height; y++)
            {
                int tile_id = GetIdUsingPerlin(x, y, offset_x, offset_y);
                noise_grid[x].Add(tile_id);
                CreateTile(tile_id, x, y);
            }
        }
    }

    int GetIdUsingPerlin(int x, int y, float offset_x, float offset_y) 
    {
        float perlin_x = (x - offset_x) / magnification;
        float perlin_y = (y - offset_y) / magnification;
        float raw_perlin = Mathf.PerlinNoise(perlin_x, perlin_y);

        float clamp_perlin = Mathf.Clamp(raw_perlin, 0f, 1f);

        float scale_perlin = clamp_perlin * tileset.Count;

        if (scale_perlin == tileset.Count) scale_perlin = tileset.Count-1;

        return Mathf.FloorToInt(scale_perlin);
    }

    void CreateTile(int tile_id, int x, int y) 
    {
        GameObject tile_prefab = tileset[tile_id];
        GameObject tile_group = tile_groups[tile_id];

        GameObject tile = Instantiate(tile_prefab, tile_group.transform);
        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0f);

        tile_grid[x].Add(tile);
    }

}
