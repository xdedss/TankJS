using NiL.JS;
using NiL.JS.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    public float RoundInterval { get { return tanks.Count == 0 ? Configurations.RoundTime : Configurations.RoundTime / tanks.Count; } }
    //public float totalInterval = ;//TODO: UI
    //public int itemInterval = 5;
    //public int itemMaxCount = 5;
    [Space]
    public GameObject tankPrefab;
    public GameObject itemPrefab;
    [Space]

    //public GameObject blockPrefab;
    public Transform mapRoot;
    public Transform[,] mapBlocks;
    public int[][] mapData;
    private List<Vector2Int> availableSpace = new List<Vector2Int>();
    public int xWidth, zWidth;
    public List<TankControl> tanks;
    public List<ItemControl> items;

    [Space]
    public Texture2D noise;
    public int seedX;
    public int seedZ;
    public bool useSeed;

    private int currentId = 0;
    private int itemIntervalCount = 0;

    private Coroutine roundCoroutine;

    private static Vector2[] rot2f = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };



    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
//        Context ctx = new Context();ctx.DebuggerCallback += (o, e) => { Debug.Log(e.Statement.Position + "," + e.Statement.EndPosition + "   " + e.Statement.ToString()); };ctx.Debugging = true;
//        Script s = Script.Parse(@"
//a = function(){
//    console.log(no);
//    return 233;
//};
//b = [1,2];");
//        Script s2 = Script.Parse("something = a();");
//        ctx.Eval(@"
//a = function(){
//    console.log(no);
//    return 233;
//};
//b = [1,2];");
//        //s.Evaluate(ctx);
//        s2.Evaluate(ctx);
//        Debug.Log(ctx.Eval("something"));
//        //StartCoroutine(co1());
    }

    //IEnumerator co1()
    //{
        
    //    Debug.Log("co1 1");
    //    for(int i = 0; i < 100; i++)
    //    {
    //        yield return co2();
    //    }
    //    Debug.Log("co1 2");
    //    Debug.Log("co1 3");
    //}
    //IEnumerator co2()
    //{
    //    //Debug.Log("co2 1");
    //    yield return 0;
    //}
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Init();
        }
    }

    public void Init()
    {
        //SetupMap(12, 24);
        //SpawnTank("tank1", 0, 0, 1, 1, new InteractiveBot());
        //SpawnTank("tank2", 11, 23, 3, 2, new ScriptBot(@"D:\LZR\MyFiles\temp\js\bot.js"));
        //SpawnTank("Tankno", 3, 0, 1, 3, new NoBot());

        Stop();

        Configurations.Reload();

        SetupMap(Configurations.MapSizeX, Configurations.MapSizeZ);//TODO: UI//no, don't do ui
        
        foreach (var botSelect in BotSelect.instances)
        {
            try
            {
                var bot = botSelect.GetBot();
                SpawnTankRandom(botSelect.tankName, currentId, bot);//no allies
            }
            catch
            {
                Debug.Log("failed to load bot " + botSelect.tankName);
                //TODO: show error
            }
        }

        roundCoroutine = StartCoroutine(RoundLoop());
    }

    public void Stop()
    {
        if (tanks.Count != 0)
        {
            foreach (var tank in tanks)
            {
                Destroy(tank.gameObject);
            }
        }
        if (items.Count != 0)
        {
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
        }
        tanks.Clear();
        items.Clear();
        currentId = 0;

        if (roundCoroutine != null)
        {
            StopCoroutine(roundCoroutine);
        }
    }

    IEnumerator RoundLoop()
    {
        while (true)
        {
            if(itemIntervalCount == 0)
            {
                itemIntervalCount = Configurations.ItemInterval;
                if(items.Count < Configurations.ItemMaxCount)
                {
                    SpawnItem(Mathf.FloorToInt(Random.value * xWidth), Mathf.FloorToInt(Random.value * zWidth), Mathf.FloorToInt(Random.value * 3) + 1);
                }
            }
            itemIntervalCount--;
            if(tanks.Count == 0)
            {
                yield return new WaitForSeconds(RoundInterval);
            }
            foreach(var t in tanks.ToArray())
            {
                yield return t.Round();
                yield return new WaitForSeconds(RoundInterval);
            }
            if(Configurations.loop && tanks.Select(t => t.tankInformation.health > 0).ToList().Count <= 1)
            {
                Init();
            }
        }
    }

    public bool CanSee(int x1, int z1, int rotation, int x2, int z2)
    {
        var p1 = new Vector3(x1, 0.5f, z1);
        var p2 = new Vector3(x2, 0.5f, z2);
        var front = rot2f[rotation % 4];
        var dir = p2 - p1;
        var dist = dir.magnitude;
        var angle = Vector2.Angle(front, new Vector2(dir.x, dir.z));
        //Debug.LogWarning(angle);
        if(dist < Configurations.VisibleRange)
        {
            return true;
        }
        else
        {
            if(angle > Configurations.FieldOfView / 2)
            {
                return false;
            }
            else
            {
                Ray ray = new Ray(p1, dir);
                return !Physics.Raycast(ray, dist, 1 << 9);
            }
        }
    }
    public bool CanSee(TankControl t1, TankControl t2)
    {
        return CanSee(t1.tankInformation.x, t1.tankInformation.z, t1.tankInformation.rotation, t2.tankInformation.x, t2.tankInformation.z);
    }
    //public bool CanSee(int id1, int id2)
    //{
    //    TankControl t1 = null, t2 = null;
    //    foreach(var t in tanks)
    //    {
    //        if (t.tankInformation.id == id1) t1 = t;
    //        else if (t.tankInformation.id == id2) t2 = t;
    //    }
    //    if(t1 && t2)
    //    {
    //        return CanSee(t1, t2);
    //    }
    //    return false;
    //}

    public int Sample(int x, int z)
    {
        if(x < 0 || x >= xWidth || z < 0 || z >= zWidth)
        {
            return -1;
        }
        return mapData[x][z];
    }
    public TankControl SampleTank(int x, int z)
    {
        foreach(TankControl t in tanks)
        {
            if(t.tankInformation.x == x && t.tankInformation.z == z)
            {
                return t;
            }
        }
        return null;
    }
    public ItemControl SampleItem(int x, int z)
    {
        foreach(var item in items)
        {
            if(item.itemInformation.x == x && item.itemInformation.z == z)
            {
                return item;
            }
        }
        return null;
    }

    private void SetupMap(int x, int z)
    {
        xWidth = x;
        zWidth = z;
        mapBlocks = new Transform[xWidth, zWidth];
        mapData = new int[xWidth][];
        for(int i= 0; i < xWidth; i++)
        {
            mapData[i] = new int[zWidth];
        }

        int cc = mapRoot.childCount;
        for(int i = 0; i < cc; i++)
        {
            var block = mapRoot.GetChild(i);
            var pos = block.position;
            int bx = Mathf.RoundToInt(pos.x);
            int bz = Mathf.RoundToInt(pos.z);
            if(bx < 0 || bx >= xWidth || bz < 0 || bz >= zWidth)
            {
                block.gameObject.SetActive(false);
            }
            else
            {
                mapBlocks[bx, bz] = block;
            }
        }

        TerrainGen();
        var fail = 0;
        while (!CheckPath(0, 0, xWidth - 1, zWidth - 1) && fail < 10)
        {
            useSeed = false;
            TerrainGen();
            fail++;
        }
        Debug.Log("fail:" + fail);
    }

    /// <summary>
    /// 检查路径存在
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="z1"></param>
    /// <param name="x2"></param>
    /// <param name="z2"></param>
    /// <returns></returns>
    private bool CheckPath(int x1, int z1, int x2, int z2)
    {
        availableSpace.Clear();
        FillAll(0, 233);
        Fill(x1, z1, 233, 0);
        for (int xi = 0; xi < xWidth; xi++)
        {
            for (int zi = 0; zi < zWidth; zi++)
            {
                if (mapData[xi][zi] == 0)
                {
                    availableSpace.Add(new Vector2Int(xi, zi));
                }
            }
        }
        bool hasPath = mapData[x2][z2] == 0;
        FillAll(233, 0);
        return hasPath;
    }

    /// <summary>
    /// 全部替换
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private void FillAll(int from, int to)
    {
        for (int xi = 0; xi < xWidth; xi++)
        {
            for (int zi = 0; zi < zWidth; zi++)
            {
                if (mapData[xi][zi] == from)
                {
                    mapData[xi][zi] = to;
                }
            }
        }
    }
    /// <summary>
    /// DFS填充
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private void Fill(int x, int z, int from, int to)
    {
        if (x < 0 || x >= xWidth || z < 0 || z >= zWidth) return;
        if(mapData[x][z] == from)
        {
            mapData[x][z] = to;
            Fill(x + 1, z, from, to);
            Fill(x - 1, z, from, to);
            Fill(x, z + 1, from, to);
            Fill(x, z - 1, from, to);
        }
    }

    private void TerrainGen()
    {
        availableSpace.Clear();
        int startx = Mathf.FloorToInt(Random.value * 256);
        int startz = Mathf.FloorToInt(Random.value * 256);
        if (useSeed)
        {
            startx = seedX;
            startz = seedZ;
        }
        Debug.Log("seed:" + startx + "," + startz);
        int step = 3;
        float downLimit = 0.2f;
        float upLimit = 0.7f;
        for (int xi = 0; xi < xWidth; xi++)
        {
            for (int zi = 0; zi < zWidth; zi++)
            {
                float noiseVal = noise.GetPixel(startx + xi * step, startz + zi * step).r;
                if (noiseVal > upLimit)
                {
                    mapBlocks[xi, zi].position = new Vector3(xi, 0.5f, zi);
                    mapBlocks[xi, zi].gameObject.SetActive(true);
                    mapData[xi][zi] = 1;
                }
                else if (noiseVal < downLimit)
                {
                    mapBlocks[xi, zi].gameObject.SetActive(false);
                    mapData[xi][zi] = -1;
                }
                else
                {
                    mapBlocks[xi, zi].position = new Vector3(xi, -0.5f, zi);
                    mapBlocks[xi, zi].gameObject.SetActive(true);
                    mapData[xi][zi] = 0;
                }
            }
        }
    }

    private void SpawnTank(string name, int x, int z, int rotation, int faction, IBot bot)
    {
        var space = new Vector2Int(x, z);
        if (availableSpace.Contains(space))
        {
            var tank = Instantiate(tankPrefab);
            var id = currentId++;
            if (name == "") name = "Tank " + id;
            tank.gameObject.name = name;
            bot.Name = name;
            tank.transform.position = new Vector3(x, 0, z);
            tank.transform.eulerAngles = new Vector3(0, 90 * rotation, 0);
            var tankControl = tank.GetComponent<TankControl>();
            tankControl.bot = bot;
            tankControl.tankName = name;
            tankControl.Init();
            tankControl.tankInformation.faction = faction;
            tankControl.tankInformation.id = id;
            tanks.Add(tankControl);
            availableSpace.Remove(space);
        }
    }

    private void SpawnTankRandom(string name, int faction, IBot bot)
    {
        if (availableSpace.Count <= 0) return;
        var space = availableSpace[Mathf.FloorToInt(Random.value * availableSpace.Count)];
        SpawnTank(name, space.x, space.y, Mathf.FloorToInt(Random.value * 4), faction, bot);
    }

    private void SpawnItem(int x, int z, int type)
    {
        if(Sample(x, z) == 0 && !SampleTank(x, z) && !SampleItem(x, z))
        {
            var item = Instantiate(itemPrefab);
            item.transform.position = new Vector3(x, 0, z);
            var itemControl = item.GetComponent<ItemControl>();
            itemControl.Init(type);
            items.Add(itemControl);
        }
    }
}
