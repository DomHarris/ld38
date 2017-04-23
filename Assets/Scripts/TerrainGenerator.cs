using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Coord {
	public int x;
	public int y;

	public int type;

	public Coord(int newX, int newY)
	{
		x = newX;
		y = newY;
		type = 0;
	}	
}
public class TerrainGenerator : MonoBehaviour {

	[SerializeField] int width;
	[SerializeField] int height;
	[SerializeField] string seed;

	[SerializeField] TerrainPooler pooler;

	[SerializeField] GameObject player;
	[SerializeField] GameObject gem;
	[SerializeField] GameObject gem1;
	[SerializeField] GameObject enemy;

	[SerializeField] float wallSize;
	public bool randomSeed = true;

	[Range(0,100)][SerializeField] int randomFillPercent;

	[SerializeField] Transform terrainParent;

	[SerializeField] Image progress;
	[SerializeField] Text currentTask;

	[SerializeField] InputHandler input;

	int [,] map;
	int mapRegenerations = 0;

	bool isInitialized = false;

	bool updatingMap = false;


	string[] textSplashes = new string[14] {
		"Gitting gud",
		"Ethically journalizing",
		"Sorry. This feature is unavailable at this time.",
		"Culling Frustrums",
		"Removing Dinklebot",
		"Adding DLC",
		"Generating hype",
		"Reticulating Splines",
		"Perpetuating crunch",
		"Burying cartidges",
		"Making you John Romero's... sandwich",
		"Removing JonTron Va",
		"Animating faces",
		"Finalizing your demise"
	};


	void Update ()
	{
		if (isInitialized && !updatingMap)
			UpdateColliders();
	}

	public bool IsEdge (int x, int y)
	{
		 return x == 0 || y == 0 || x == width -1 || y == height - 1;
	}

	public bool IsWall (int x, int y)
	{
		return map[x,y] == 1;
	}

	public IEnumerator GenerateMap()
	{
		System.Random rng = new System.Random(Guid.NewGuid().ToString().GetHashCode());
		map = new int [width, height];
		currentTask.text = textSplashes[rng.Next(0, textSplashes.Length-1)];
		progress.fillAmount = 0;
		RandomFillMap();
		yield return new WaitForEndOfFrame();
		LeanTween.value(progress.gameObject, UpdateProgress, 0, 0.2f, .5f ).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(.5f);

		for (int i = 0; i < 10; ++i)
		{
			SmoothMap();
		}
		currentTask.text = textSplashes[rng.Next(0, textSplashes.Length-1)];
		yield return new WaitForEndOfFrame();
		LeanTween.value(progress.gameObject, UpdateProgress, 0.2f, 0.4f, .5f ).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(.5f);
		//InstantiateMap();
		PopulateMap();
		currentTask.text = textSplashes[rng.Next(0, textSplashes.Length-1)];
		yield return new WaitForEndOfFrame();
		LeanTween.value(progress.gameObject, UpdateProgress, 0.4f, 0.6f, .5f ).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(.5f);

		pooler.InstantiateObjects(terrainParent, width, height, map);
		currentTask.text = textSplashes[rng.Next(0, textSplashes.Length-1)];
		yield return new WaitForEndOfFrame();
		LeanTween.value(progress.gameObject, UpdateProgress, 0.6f, 0.8f, .5f ).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(.5f);

		InstantiateMap();
		currentTask.text = textSplashes[rng.Next(0, textSplashes.Length-1)];
		yield return new WaitForEndOfFrame();
		LeanTween.value(progress.gameObject, UpdateProgress, 0.8f, 1.0f, .5f ).setEase(LeanTweenType.linear);
		yield return new WaitForSeconds(.5f);

		
		input.paused = false;
		yield return new WaitForEndOfFrame();
		isInitialized = true;
	}

	public void UpdateProgress(float val)
	{
		progress.fillAmount = val;
	}

	void RandomFillMap()
	{
		if (randomSeed)
			seed = Guid.NewGuid().ToString();
		
		System.Random rng = new System.Random (seed.GetHashCode());

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				if (x == 0 || x == width-1 || y == 0 || y == height-1)
					map[x,y] = 1;
				else
					map[x,y] = rng.Next(0, 100) < randomFillPercent ? 1 : 0;
			}
		}
	}

	void InstantiateMap()
	{
		int playerX=0, playerY=0;

		GetPlayerGrid(ref playerX, ref playerY);

		for (int x = 0; x < height; ++x)
		{
			for (int y = 0; y < width; ++y)
			{
				if (InBounds(x,y))
				{
					GameObject o = pooler.GetAvailable(x,y);

					if (o != null)
					{
						if (map[x,y] != 0)
						{
							o.SetActive(true);
							o.transform.position = new Vector3 ((-width/2*wallSize) + (x * wallSize), (-height/2*wallSize) + (y*wallSize));
							
							o.name = string.Format ("{0},{1}_{2}", x, y,map[x,y]);
						} else o.SetActive(false);
					}
				}
			}
		}
	}

	public IEnumerator RegenerateMap ()
	{
		updatingMap = true;
		++mapRegenerations;

		for (int x = 0; x < height; ++x)
		{
			for (int y = 0; y < width; ++y)
			{
				if (map[x,y] > 1) 
				{	
					pooler.RemoveFromGrid(x,y);
					map[x,y] = 0;
				}
			}
		}
	
		yield return RePopulateMap();
		updatingMap = false;
	}

	IEnumerator RePopulateMap ()
	{
		List<List<Coord>> rooms = GetAllRegions(0);

		List<Coord> newItems = new List<Coord>();

		int numGems = 50 + (10 * mapRegenerations);
		
		for (int i = 0; i < numGems; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 2;
			tile.type = 2;
			newItems.Add(tile);
		}
		
		int numPowerups = 5 + mapRegenerations;

		for (int i = 0; i < numPowerups; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 3;
			tile.type = 2;
			newItems.Add(tile);
		}

		int numTimers = 10 + (2*mapRegenerations);

		for (int i = 0; i < numTimers; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 4;
			tile.type = 4;
			newItems.Add(tile);
		}

		int numEnemies = 20 + (5 * mapRegenerations);

		for (int i = 0; i < numEnemies; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 5;
			tile.type = 5;			
			newItems.Add(tile);
		}

		yield return pooler.ReInstantiate(newItems, map);
	}

	void PopulateMap ()
	{
		List<List<Coord>> rooms = GetAllRegions(0);
		List<List<Coord>> largeRooms = new List<List<Coord>>();
		for (int i = 0; i < rooms.Count; ++i)
		{
			if (rooms[i].Count > 200) largeRooms.Add(rooms[i]);
		}

		int spawnRoom = UnityEngine.Random.Range(0, largeRooms.Count);

		rooms.Remove(largeRooms[spawnRoom]);
		Coord spawnMiddle = largeRooms[spawnRoom][ Mathf.RoundToInt(largeRooms.Count/2)];

		player.transform.position = new Vector3 ((-width/2*wallSize) + (spawnMiddle.x * wallSize), (-height/2*wallSize) + (spawnMiddle.y*wallSize));

		int numGems = 50;
		
		for (int i = 0; i < numGems; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 2;
		}
		
		int numPowerups = 5;

		for (int i = 0; i < numPowerups; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 3;
		}

		int numTimers = 10;

		for (int i = 0; i < numTimers; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 4;
		}

		int numEnemies = 20;

		for (int i = 0; i < numEnemies; ++i)
		{
			int room = UnityEngine.Random.Range(0, rooms.Count);
			int spawnTile = UnityEngine.Random.Range(0, rooms[room].Count);
			Coord tile = rooms[room][spawnTile];
			rooms[room].RemoveAt(spawnTile);
			map[tile.x, tile.y] = 5;
		}
	}

	void UpdateColliders ()
	{
		int playerX=0, playerY=0;

		GetPlayerGrid(ref playerX, ref playerY);

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				if (InBounds(x,y))
				{
					GameObject o = pooler.GetAt(x, y);

					if (o != null)
 					{
						if (x > playerX - 25 && x < playerX + 25 && y > playerY - 25 && y < playerY + 25)
						{
							
							o.GetComponent<Collider2D>().enabled = true;

							if (map[x,y] == 5)
							{
								o.GetComponent<Rigidbody2D>().simulated = true;
							}
						} else {
							o.GetComponent<Collider2D>().enabled = false;
							if (map[x,y] == 5)
							{
								o.GetComponent<Rigidbody2D>().simulated = false;
							}
						}
					}
				}
			}
		}
	}

	void GetPlayerGrid (ref int x, ref int y)
	{
		// posX - (-width/2*wallsize) = x * wallsize
		// x = posX - (-width/2*wallsize) / wallsize
		
		x = Mathf.RoundToInt((player.transform.position.x + (width/2*wallSize)) / wallSize);
		y = Mathf.RoundToInt((player.transform.position.y + (height/2*wallSize)) / wallSize);
	}

	int GetSurrondingWallCount (int checkx, int checky)
	{
		int wallCount = 0;
		for (int x = checkx - 1; x <= checkx + 1; ++x)
		{
			for (int y = checky - 1; y <= checky + 1; ++y)
			{
				if ((x != checkx || y != checky))
				{
					if (InBounds(x,y))
						wallCount += map[x,y];
					else
						++wallCount;
				}
			}
		}

		return wallCount;
	}

	public bool InBounds(int x, int y)
	{
		return (x >= 0 && x < width && y >= 0 && y < height);
	}

	void SmoothMap ()
	{
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				int surroundingWalls = GetSurrondingWallCount(x, y);
		
				if (surroundingWalls > 4)
					map[x,y] = 1;
				else if (surroundingWalls < 4)
					map[x,y] = 0;
			}
		}
	}

	public void UpdateMap (int x, int y, int val)
	{
		map[x,y] = val;
	}

	List<Coord> GetRegion (int startX, int startY)
	{
		List<Coord> tiles = new List<Coord>();
		int [,] mapFlags = new int [width,height];
		int tileType = map [startX, startY];

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX, startY));
		mapFlags[startX, startY] = 1;

		while (queue.Count > 0)
		{
			Coord tile = queue.Dequeue();
			tiles.Add(tile);
			for (int x = tile.x -1; x <= tile.x +1; ++x)
			{
				for (int y = tile.y -1; y <= tile.y +1; ++y)
				{
					if (InBounds(x,y) && (y == tile.y || x == tile.x))
					{
						if (mapFlags[x,y] == 0 && map[x,y] == tileType)
						{
							mapFlags[x,y] = 1;
							queue.Enqueue(new Coord(x,y)) ;
						}
					}
				}
			}
		}

		return tiles;
	}

	List<List<Coord>> GetAllRegions(int tileType)
	{
		List<List<Coord>> regions = new List<List<Coord>>();
		int[,] mapFlags = new int[width,height];

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				if (mapFlags[x,y] == 0 && map[x,y] == tileType)
				{
					List<Coord> newRegion = GetRegion(x,y);
					regions.Add (newRegion);
					for (int i = 0; i < newRegion.Count; ++i)
						mapFlags[newRegion[i].x,newRegion[i].y] = 1;
				}
			}
		}

		return regions;
	}
}
