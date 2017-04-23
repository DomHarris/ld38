using UnityEngine;

public class ParticleCollision : MonoBehaviour {
	[SerializeField] CameraShake shaker;
	[SerializeField] TerrainGenerator terrain;
	[SerializeField] TerrainPooler pool;

	[SerializeField] int radius = 0;

	[SerializeField] ScoreManager score;
	[SerializeField] InputHandler input;

	[SerializeField] AudioManager audioManager;
	void OnParticleCollision(GameObject other)
	{
		string[] xy = other.name.Split(new char[1] { ','}, System.StringSplitOptions.None);
		string[] yt = xy[1].Split(new char[1] { '_'}, System.StringSplitOptions.None);
		int xPos = int.Parse(xy[0]);
		int yPos = int.Parse(yt[0]);
		int type = int.Parse(yt[1]);

		bool isEdge = terrain.IsEdge(xPos, yPos);
		if (radius != 0 && type == 1)
		{
			for (int x = xPos -radius; x <= xPos +radius; ++x)
			{
				for (int y = yPos -radius; y <= yPos +radius; ++y)
				{
					bool topLeft = x == xPos - radius && y == yPos + radius;
					bool topRight = x == xPos + radius && y == yPos + radius;
					bool bottomLeft = x == xPos - radius && y == yPos - radius;
					bool bottomRight = x == xPos + radius && y == yPos - radius;

					if (terrain.InBounds(x,y) && terrain.IsWall(x,y) && !terrain.IsEdge(x,y) && !(topLeft || topRight || bottomLeft || bottomRight))
					{
						//terrain.UpdateMap(x, y, 0);
						if (pool.GetAt(x,y) != null)
							pool.SetActive(x,y, false);
					}
				}
			}
		} else {
			//terrain.UpdateMap(xPos, yPos, 0);
			if (pool.GetAt(xPos,yPos) != null && !isEdge)
			{
				pool.SetActive(xPos,yPos, false);
				//other.SetActive(false);
				//Debug.Log (type);
			}
		}

		shaker.Shake();

		switch (other.tag)
		{
			case "Wall":
				if (!isEdge)
					score.UpdateScore(1);
				audioManager.PlaySound("explosion");
				break;
			case "Gem0": 
				score.UpdateScore(100);
				audioManager.PlaySound("pickup");
				break;
			case "Gem1": 
				input.Powerup();
				audioManager.PlaySound("powerup");
				break;
			case "Gem2": 
				score.UpdateTimer(5);
				audioManager.PlaySound("powerup");
				break;
			case "Enemy": 
				audioManager.PlaySound("hit");
				break;
		}
	}

	public void IncreaseRadius ()
	{
		radius += 2;
	}
}
