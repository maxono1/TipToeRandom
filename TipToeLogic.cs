using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private Material pathMat;
    //[SerializeField] private Shader platformShader;
    private readonly int width = 10;
    private readonly int depth = 13;
    private readonly float gap = 3;

    private bool[,] paths;



    // Start is called before the first frame update
    void Start()
    {
        paths = new bool[width, depth];

        platformPrefab.transform.localScale = new Vector3(2.9f, 0.2f, 2.9f);
        for (int i = 0; i < depth; i++)
        {
            Debug.Log(i);
            if (i <= 5)
            {
                paths[4, i] = true;
            }
            if (i >= 5)
            {
                paths[5, i] = true;
            }

        }
        startAlgo();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                //Debug.Log("wtf");
                GameObject gameObject = Instantiate(platformPrefab, new Vector3(-13.52f + (gap * i), 0, 10 + (gap * j)), Quaternion.identity);
                // https://docs.unity3d.com/ScriptReference/GameObject.AddComponent.html
                //gameObject.AddComponent(typeof(BoxCollider));

                if (paths[i, j])
                {
                    if (j == 0 || (j > 0 && paths[i, j - 1]))
                    {
                        NavMeshLink backLink = gameObject.AddComponent(typeof(NavMeshLink)) as NavMeshLink;
                        backLink.endPoint = new Vector3(0, 0, 0);
                    }
                    if (i < width - 1 && paths[i + 1, j])
                    {
                        NavMeshLink rightLink = gameObject.AddComponent(typeof(NavMeshLink)) as NavMeshLink;

                        rightLink.endPoint = new Vector3(0, 0, 0);
                        rightLink.startPoint = new Vector3(2, 0, 0);
                    }
                    if (i > 0 && paths[i - 1, j])
                    {
                        NavMeshLink leftLink = gameObject.AddComponent(typeof(NavMeshLink)) as NavMeshLink;
                        leftLink.endPoint = new Vector3(0, 0, 0);
                        leftLink.startPoint = new Vector3(-2, 0, 0);
                    }
                    if (j == depth - 1 || (j < depth - 1 && paths[i, j + 1]))
                    {
                        NavMeshLink topLink = gameObject.AddComponent(typeof(NavMeshLink)) as NavMeshLink;
                        topLink.endPoint = new Vector3(0, 0, 2);
                        topLink.startPoint = new Vector3(0, 0, 0);
                    }
                    gameObject.layer = 8;
                    gameObject.AddComponent(typeof(NavMeshSurface));
                    gameObject.GetComponent<TipToePlatform>().isPath = true;
                    gameObject.GetComponent<TipToePlatform>().defaultMaterial = pathMat;
                    gameObject.GetComponent<NavMeshSurface>().layerMask = LayerMask.GetMask("Nav");
                    gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    //x ist width, y depth
    //startpunkt random 0-(width-1), 1
    //diese methode setzt die ersten beiden felder random, aber zusammen
    void startAlgo()
    {
        bool[,] bools = new bool[width, depth];
        int x = Random.Range(0, width);
        bools[x, 0] = true;
        bools[x, 1] = true;

        PlatformPos currentPos = new PlatformPos(x, 1);
        this.paths = bools;
        backtracking(bools, currentPos);
    }

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
        }
        else
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
    //
    //neue platform hat nur 1 nachbar, da wo es herkommt
    List<PlatformPos> findLegalMovements(PlatformPos currentPos)
    {
        List<PlatformPos> legalMovements = new();
        List<PlatformPos> psblMovements = give_NWOS_OfPlatformPos(currentPos);

        foreach (PlatformPos p in psblMovements)
        {
            int numberOfNeighbors = 0;


            List<PlatformPos> psblNeighbors = give_NWOS_OfPlatformPos(p);
            foreach (PlatformPos n in psblNeighbors)
            {
                //wenn ein gültiger nachbar ist, und schon ein pfad drauf ist, ist es ein richtiger nachbar
                if (isInsideBounds(n) && paths[n.x, n.y])
                {
                    numberOfNeighbors++;
                }
            }


            if (isInsideBounds(p) && numberOfNeighbors == 1 && !paths[p.x, p.y])
            {
                legalMovements.Add(p);
            }
        }

        return legalMovements;
    }

    bool backtracking(bool[,] bools, PlatformPos currentPlatformPos)
    {
        //wo kann man sich legal hinbewegen?
        List<PlatformPos> legalMovements = findLegalMovements(currentPlatformPos);
        while (legalMovements.Count > 0)
        {
            int movementOfChoiceIndex = Random.Range(0, legalMovements.Count);
            PlatformPos movementOfChoice = legalMovements[movementOfChoiceIndex];
            legalMovements.RemoveAt(movementOfChoiceIndex);

            bools[movementOfChoice.x, movementOfChoice.y] = true;
            if (movementOfChoice.y == depth - 1)
            {
                paths = bools;
                return true;
            }
            if (backtracking(bools, movementOfChoice))
            {
                return true;
            }
            else
            {
                //das wird rausgenommen, damit automatisch sackgassen entstehen
                //bools[movementOfChoice.x, movementOfChoice.y] = false;


                //wir müssen nochmal die Legal movements suchen, weil nach backtracking sich das feld verändert hat
                legalMovements = findLegalMovements(currentPlatformPos);
            }

            //psbl error, was wenn die anderen nicht nachrücken?
        }

        //wenn keine legalMovements vorhanden, return false
        return false;
    }
}