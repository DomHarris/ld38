using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPooler : MonoBehaviour {

	[SerializeField] GameObject[] prefabs;

	GameObject[,] objArray;
	int[,] map;

	int width, height;

	Transform tileParent = null;	
	public void InstantiateObjects (Transform parent, int newWidth, int newHeight, int[,] newMap)
	{
		map = newMap;
		tileParent = parent;
		width = newWidth;
		height = newHeight;
		objArray = new GameObject[width, height];

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				if (map[x,y] > 0)
				{
					objArray[x,y] = Instantiate (prefabs[map[x,y]]) as GameObject;
					objArray[x,y].transform.SetParent(parent);
					objArray[x,y].SetActive(false);

					if (map[x,y] == 5)
					{
						objArray[x,y].GetComponent<Rigidbody2D>().simulated = false;
					}
				}
			}
		}
	}

	public IEnumerator ReInstantiate (List<Coord> tiles, int[,] newMap)
	{
		map = newMap;

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				if (map[x,y] == 1)
				{
					if (!objArray[x,y].activeSelf)
						objArray[x,y].SetActive(true);
				}
				for (int i = 0; i < tiles.Count; ++i)
				{
					if (x == tiles[i].x && y == tiles[i].y)
					{
						//Destroy(objArray[x,y]);
						GameObject o = GetAvailable(tiles[i].type);
						if (o != null)
						{
							o.SetActive(true);
							objArray[x,y] = o;
						}
						else if (tiles[i].type > 0)
							objArray[x,y] = Instantiate (prefabs[tiles[i].type]) as GameObject;

						if (tiles[i].type > 0)
						{
							float wallSize = 0.125f;
							objArray[x,y].transform.position = new Vector3 ((-width/2*wallSize) + (x * wallSize), (-height/2*wallSize) + (y*wallSize));
							objArray[x,y].name = string.Format ("{0},{1}_{2}", x, y, tiles[i].type);

							if (tiles[i].type == 5)
								objArray[x,y].GetComponent<Rigidbody2D>().simulated = false;
						}
					}
				}
			}
			if (x % 2 == 0)
				yield return new WaitForEndOfFrame();
		}
	}


	public GameObject GetAvailable (int type)
	{
		string typeTest = "_" + type;
		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				if (objArray[x,y] != null && objArray[x,y].name.Contains( typeTest ) && !objArray[x,y].activeSelf)
					return objArray[x,y];
			}
		}
		return null;
	}

	public GameObject GetNextWall (int xDone, int yDone)
	{
		string typeTest = "_1";
		for (int x = xDone; x < width; ++x)
		{
			for (int y = yDone; y < height; ++y)
			{
				if (objArray[x,y] != null && objArray[x,y].name.Contains( typeTest ))
					return objArray[x,y];
			}
		}
		return null;
	}

	public List<Coord> GetAvailableItems ()
	{
		List<Coord> toReturn = new List<Coord>();

		for (int x = 0; x < width; ++x)
		{
			for (int y = 0; y < height; ++y)
			{
				for (int i = 1; i < prefabs.Length; ++i)
				{
					string typeTest = "_" + i;
					if (objArray[x,y] != null && objArray[x,y].name.Contains( typeTest ) )
					{
						toReturn.Add(new Coord(x, y));
					}
				}
			}
		}

		return toReturn;
	}

	public GameObject GetAvailable (int x, int y)
	{
		if (objArray[x,y] != null && !objArray[x,y].activeSelf)
			return objArray[x,y];
		else return null;
	}

	
	public GameObject GetAt (int x, int y)
	{
		return objArray[x,y];
	}

	public void SetActive (int gridX, int gridY, bool active)
	{
		if (objArray[gridX,gridY] != null)
			objArray[gridX,gridY].SetActive(active);
	}
	
	public void RemoveFromGrid (int x, int y)
	{
		if (objArray[x,y] != null)
			objArray[x,y].SetActive(false);
			
		objArray[x,y] = null;
	}
}
