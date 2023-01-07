using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipToeLogic : MonoBehaviour
{
    class PlatformPos
    {
        public int x;
        public int y;
        public PlatformPos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [SerializeField] private GameObject platformPrefab;
    //[SerializeField] private Shader platformShader;
    private readonly int width = 10;
    private readonly int depth = 13;
    private readonly float gap = 3;

    private bool[,] paths;

    

    // Start is called before the first frame update
    void Start()
    {
        paths = new bool[depth, width];

        platformPrefab.transform.localScale = new Vector3(2.9f, 0.2f, 2.9f);
        for (int i = 0; i < depth; i++)
        {
            Debug.Log(i);   
            if (i <= 5)
            {
                paths[i, 3] = true;
            } 
            if(i >= 5)
            {
                paths[i, 4] = true;
            }
            
        }

        for (int i = 0; i < depth; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //Debug.Log("wtf");
                GameObject gameObject = Instantiate(platformPrefab, new Vector3(-13.52f + (gap * j), 0, 10 + (gap * i)), Quaternion.identity);
                // https://docs.unity3d.com/ScriptReference/GameObject.AddComponent.html
                //gameObject.AddComponent(typeof(BoxCollider));
                //gameObject.AddComponent(typeof(TipToePlatform));
                if (paths[i, j])
                {
                    gameObject.GetComponent<TipToePlatform>().isPath = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //current koordinate
    //x ist width, y depth
    //startpunkt random 0-(width-1), 1


    //
    // N: x, y+1
    // W: x-1, y
    // O: x+1, y
    // S: x, y-1
    //
    List<PlatformPos> give_NWOS_OfPlatformPos(PlatformPos pos)
    {
        List<PlatformPos> neighbors = new();
        PlatformPos N = new PlatformPos(pos.x, pos.y + 1);
        PlatformPos W = new PlatformPos(pos.x - 1, pos.y);
        PlatformPos O = new PlatformPos(pos.x + 1, pos.y);
        PlatformPos S = new PlatformPos(pos.x, pos.y - 1);
        neighbors.Add(N);
        neighbors.Add(W);
        neighbors.Add(O);
        neighbors.Add(S);

        return neighbors;
    }

    bool isInsideBounds(PlatformPos p)
    {
        if (p.x >= 0 && p.x < width && p.y > 0 && p.y < depth)
        {
            return true;
        } else
        {
            return false; 
        }
    }


    //legalMovement definition:
    //y > 0 && y < depth
    //x >= 0 && x < width
    //--> inside bounds
    //
    //ist noch nicht true im bool array(noch kein pfad gesetzt)
    //neue platform hat nur 1 nachbar, da wo es herkommt
    List<PlatformPos> findLegalMovements(PlatformPos currentPos)
    {
        List<PlatformPos> legalMovements = new();
        List<PlatformPos> psblMovements = give_NWOS_OfPlatformPos(currentPos);
        
        foreach (PlatformPos p in psblMovements)
        {
            int numberOfNeighbors = 0;
            bool notOccupied = false;
            
            List<PlatformPos> psblNeighbors = give_NWOS_OfPlatformPos(p);
            foreach(PlatformPos n in psblNeighbors){
                //wenn ein gültiger nachbar ist, und schon ein pfad drauf ist, ist es ein richtiger nachbar
                if (isInsideBounds(n) && paths[n.x, n.y])
                {
                    numberOfNeighbors++;
                }
            }
            if(!paths[p.x, p.y])
            {
                notOccupied = true;
            }

            if(isInsideBounds(p) && numberOfNeighbors == 1 && notOccupied)
            {
                legalMovements.Add(p);
            }
        }

        return legalMovements;
    }

    bool backtracking(PlatformPos currentPlatformPos)
    {
        //wo kann man sich legal hinbewegen?
        List<PlatformPos> legalMovements = findLegalMovements(currentPlatformPos);
        int movementOfChoice = Random.Range(0, legalMovements.Count);
        //wir brauchen eine liste von haram movements
        //diese movements in einem array speichern



        //wähle einen neuen teillösungsschritt -- hier random
        bool gueltig = true;
        if (gueltig)
        {
            //wenn vollständig, dann return true
        }

        return false;
    }
}
