using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

class Configurations
{

    public static string BotFolder
    {
        get
        {
            return System.Environment.CurrentDirectory + @"\Scripts\";
        }
    }

    public static Dictionary<string, string> pairs = new Dictionary<string, string>();
    private static Dictionary<string, float> pairsf = new Dictionary<string, float>();
    private static Dictionary<string, int> pairsi = new Dictionary<string, int>();
    private static Dictionary<string, bool> pairsb = new Dictionary<string, bool>();

    public static void Reload()
    {
        pairs.Clear();
        pairsf.Clear();
        pairsi.Clear();
        pairsb.Clear();
        var sr = File.OpenText(BotFolder + "config.cfg");
        string ln;
        while((ln = sr.ReadLine()) != null)
        {
            var sp = ln.Split('=');
            if(sp.Length == 2)
            {
                pairs.Add(sp[0].Trim(), sp[1].Trim());
            }
        }
    }

    public static float FieldOfView { get { return GetFloat("FIELD_OF_VIEW", 180); } }
    public static float VisibleRange { get { return GetFloat("VISIBLE_RANGE", 2.1f); } }
    public static int RadarDuration { get { return GetInt("RADAR_DURATION", 5); } }
    public static int DefaultAttack { get { return GetInt("DEFAULT_ATTACK", 10); } }
    public static int BuffAttack { get { return GetInt("BUFF_ATTACK", 20); } }
    public static int DefaultHealth { get { return GetInt("DEFAULT_HEALTH", 100); } }
    public static int BuffHealth { get { return GetInt("BUFF_HEALTH", 10); } }
    public static float DistanceFac { get { return GetFloat("DISTANCE_FACTOR", 1); } }
    public static int ItemInterval { get { return GetInt("ITEM_INTERVAL", 5); } }
    public static int ItemMaxCount { get { return GetInt("ITEM_MAX_COUNT", 5); } }
    public static int MapSizeX { get { return GetInt("MAP_SIZE_X", 12); } }
    public static int MapSizeZ { get { return GetInt("MAP_SIZE_Z", 24); } }
    public static int ScriptTimeout { get { return GetInt("SCRIPT_TIMEOUT", 1000); } }





    private static float GetFloat(string name, float defaultValue)
    {
        if (pairsf.ContainsKey(name)) return pairsf[name];
        float res;
        if(pairs.ContainsKey(name) && float.TryParse(pairs[name], out res))
        {
            pairsf.Add(name, res);
            return res;
        }
        return defaultValue;
    }

    private static int GetInt(string name, int defaultValue)
    {
        if (pairsi.ContainsKey(name)) return pairsi[name];
        int res;
        if (pairs.ContainsKey(name) && int.TryParse(pairs[name], out res))
        {
            pairsi.Add(name, res);
            return res;
        }
        return defaultValue;
    }

    private static bool Getbool(string name, bool defaultValue)
    {
        if (pairsb.ContainsKey(name)) return pairsb[name];
        bool res;
        if (pairs.ContainsKey(name) && bool.TryParse(pairs[name], out res))
        {
            pairsb.Add(name, res);
            return res;
        }
        return defaultValue;
    }

}
