using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LineOfSight : MonoBehaviour
{
    public LayerMask wall;
    private List<LignOfSightObject> objects = new List<LignOfSightObject>();

    [SerializeField]
    Material mat;
    [SerializeField]
    Camera cam;
    [Tooltip("Leave tileMap as null if you want to set things manually or dont have a tilemap")]
    [SerializeField]
    Tilemap tileMap;
    [Tooltip("Recogmended value of viewPoint is the transform component of your player")]
    [SerializeField]
    Transform viewPoint;
    [Tooltip("Recogmended value of maxDistanceToRender is roughly 2.5 * camera size")]
    [SerializeField]
    float maxDistanceToRender;

    private void Start()
    {
        LignOfSightObject.maxViewRange = maxDistanceToRender;
        UpdateTileMap();

        /* \/ EXAMPLE OF HOW TO ADD YOUR OWN OBJECT \/
         
        objects.Add(new LignOfSightObject
            (
            new List<Vector2>{
                new Vector2(5, 0),
                new Vector2(6, 0),
                new Vector2(7, 0),
                new Vector2(7, 1),
                new Vector2(6, 1),
                new Vector2(6, 2),
                new Vector2(5, 2),
                new Vector2(5, 1),
            },
            wall,
            mat,
            false // set this true if you are making a small object. If you are unsure set it false
            ));
            
         */
    }
    public void UpdateTileMap()
    {
        if (tileMap == null)
        {
            return;
        }

        List<Vector2> tiles = new List<Vector2>();

        foreach (var pos in tileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tileMap.CellToWorld(localPlace);
            if (tileMap.HasTile(localPlace))
            {
                tiles.Add(place);
            }
        }

        objects.Clear();
        for (int i = 0; i < tiles.Count; i++)
        {
            objects.Add(
                new LignOfSightObject(
                    new List<Vector2>
                    {
                        tiles[i],
                        tiles[i] + new Vector2(1, 0),
                        tiles[i] + new Vector2(1, 1),
                        tiles[i] + new Vector2(0, 1),
                    },
                    wall,
                    mat,
                    true
                    )
                );
        }
    }
    public void OnPostRender()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            LignOfSightObject thisObj = objects[i];
            List<Vector3[]> quads = thisObj.GetQuadsToRender(viewPoint.position);
            foreach (Vector3[] quad in quads) {
                RenderQuad(quad, cam);
            }
        }
    }
    public void RenderQuad(Vector3[] vectors, Camera cam)
    {
        if (vectors.Length != 4)
        {
            Debug.LogError("There must be exactly 4 vectors passes into RenderQuad. You passed in " + vectors.Length.ToString()+".");
            return;
        }

        Vector3[] screenVectors = new Vector3[4];

        for (int i = 0; i < vectors.Length; i++)
        {
            float camWidth = cam.pixelWidth;
            float camHeight = camWidth / cam.aspect;
            Vector3 pixelScreenPoint = cam.WorldToScreenPoint(vectors[i]);
            Vector3 screenPoint = new Vector3(pixelScreenPoint.x / camWidth, pixelScreenPoint.y / camHeight, 0);
            screenVectors[i] = screenPoint;
        }

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        GL.Vertex(screenVectors[0]);
        GL.Vertex(screenVectors[1]);
        GL.Vertex(screenVectors[2]);
        GL.Vertex(screenVectors[3]);
        GL.End();
        GL.PopMatrix();
    }
}

public class LignOfSightObject
{
    public static float maxViewRange = 50;

    public List<Vector2> nodes = new List<Vector2>();
    public LayerMask wall;
    public Material mat;
    private bool fastMode; // set this true for large objects

    public LignOfSightObject(List<Vector2> nodes, LayerMask wall, Material mat, bool fastMode)
    {
        this.nodes = nodes;
        this.wall = wall;
        this.mat = mat;
        this.fastMode = fastMode;
    }


    public List<Vector3[]> GetQuadsToRender(Vector2 viewPoint)
    {
        if (!fastMode)
        {
            bool inRange = false;
            for (int i = 0; i < this.nodes.Count; i++)
            {
                if (Vector2.Distance(viewPoint, this.nodes[i]) < LignOfSightObject.maxViewRange)
                {
                    inRange = true;
                    break;
                }
            }
            if (!inRange)
            {
                return new List<Vector3[]>();
            }
        }
        else
        {
            if (Vector2.Distance(viewPoint, this.nodes[0]) > LignOfSightObject.maxViewRange)
            {
                return new List<Vector3[]>();
            }
        }


        List<Vector3[]> ret = new List<Vector3[]>();
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3[] vertices = new Vector3[] 
            {
                nodes[(i + 1) % nodes.Count],
                nodes[i],
                GetPointInDir(nodes[i], viewPoint),
                GetPointInDir(nodes[(i + 1) % nodes.Count], viewPoint)
            };
            ret.Add(vertices);
        }
        return ret;
    }
    public Vector2 GetPointInDir(Vector2 point, Vector2 viewPoint)
    {
        Vector2 dir = (point - viewPoint).normalized;
        dir *= 1000;
        return dir + viewPoint;
    }

}