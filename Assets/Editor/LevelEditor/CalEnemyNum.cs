using System;
using System.Collections.Generic;
using UnityEngine;

public class CalEnemyNum
{

    public static bool PrintLog = false;

    private const int inf = 0x3f3f3f3f;
    private struct node
    {
        public int to;
        public int cap;
        public int next;
        public void Init(int to, int cap, int next)
        {
            this.to = to;
            this.cap = cap;
            this.next = next;
        }
    }

    private List<node> p = new List<node>();
    private List<int> dis = new List<int>();
    private List<int> gap = new List<int>();
    private List<int> next = new List<int>();
    private int cnt, s, e;
    private int n, NN;

    private void add(int from, int to, int cap)
    {
        node tmp = new node();
        tmp.Init(to, cap, next[from]);
        p.Add(tmp);
        next[from] = cnt++;
        tmp.Init(from, 0, next[to]);
        p.Add(tmp);
        next[to] = cnt++;
    }

    private int dfs(int pos, int cost)
    {
        if (pos == e) return cost;
        int i, mdis = NN, f = cost;
        for (i = next[pos]; i != -1; i = p[i].next)
        {
            int to = p[i].to;
            int cap = p[i].cap;
            if (cap > 0)
            {
                if (dis[to] + 1 == dis[pos])
                {
                    int d = Math.Min(f, cap);
                    d = dfs(to, d);
                    node tmp = p[i];
                    tmp.cap -= d;
                    p[i] = tmp;
                    tmp = p[i ^ 1];
                    tmp.cap += d;
                    p[i ^ 1] = tmp;
                    f -= d;
                    if (dis[s] >= NN) { return cost - f; }
                    if (f == 0) { break; }
                }
                if (dis[to] < mdis) { mdis = dis[to]; }
            }
        }
        if (f == cost)
        {
            --gap[dis[pos]];
            if (gap[dis[pos]] == 0) dis[s] = NN;
            dis[pos] = mdis + 1;
            ++gap[dis[pos]];
        }
        return cost - f;
    }

    private int isap(int b, int t)
    {
        int ret = 0;
        s = b;
        e = t;
        gap.Clear();
        dis.Clear();
        for (int i = 0; i <= NN + 1; ++i)
        {
            gap.Add(0);
            dis.Add(0);
        }
        gap[s] = NN;
        while (dis[s] < NN)
        {
            ret += dfs(s, inf);
        }

        return ret;
    }

    private int slove(List<LevelWave> _wave, uint enemyid)
    {
        Dictionary<int, int> map = new Dictionary<int, int>();
        int sum = 0;
        int ans = 0;
        int Sid = 0;
        n = 0;
        foreach (LevelWave wave in _wave)
        {
            if (wave._id < 1000)
            {
                ++n;
                if (map.ContainsKey(wave._id) == false)
                {
                    map.Add(wave._id, ++Sid);
                }
            }
        }
        p.Clear();
        cnt = 0;
        int b = 0;
        int t = n + n + n + 1;
        NN = n + n + n + 2;
        next.Clear();
        for (int i = 0; i <= NN; ++i)
            next.Add(-1);
        foreach (LevelWave wave in _wave)
        {
            if (wave._id >= 1000) continue;
            int count = wave._prefabSlot.Count + wave.RoundCount;
            int id = map[wave._id];
            int Percent = wave._preWavePercent;
            if (wave.EnemyID == enemyid)
            {
                add(b, id, count);
                add(id + n, t, count);
                sum += count;
            }
            else
            {
                add(b, id, 0);
                add(id + n, t, 0);
            }
            add(id + n, id, inf);
            add(id + n + n, id, inf);
            add(id + n + n, t, 0);

            if (wave._preWaves == null) continue;
            List<string> preWave = new List<string>(wave._preWaves.Split(','));
            foreach (string pre in preWave)
            {
                if (pre.Length == 0) continue;

                int tmp;
                if (int.TryParse(pre, out tmp))
                {
                    if (map.ContainsKey(int.Parse(pre)) == false)
                    {
                        if (PrintLog) Debug.LogError(string.Format("Wave {0} is rely on Wave {1}, but Wave {1} is not exist!!!", wave._id, pre));
                        continue;
                    }
                    if (Percent == 0)
                    {
                        add(id, map[int.Parse(pre)] + n, inf);
                    }
                    else
                    {
                        add(id, map[int.Parse(pre)] + n + n, inf);
                    }
                }
                else
                {
                    if (PrintLog) Debug.LogError(string.Format("Wave {0} PreWave String Can't be Parse!!!", wave._id));
                }
            }
        }
        ans = isap(b, t);
        return sum - ans;
    }

    /************************************************************************/
    /* Calculate Enemy Num                                                  */
    /************************************************************************/
    public Dictionary<uint, int> CalNum(List<LevelWave> _wave)
    {
        Dictionary<uint, int> map = new Dictionary<uint, int>();
        //int sum = 0, res = 0;
        foreach (LevelWave wave in _wave)
        {
            if (wave.SpawnType == LevelSpawnType.Spawn_Source_Doodad)
                continue;

            if (wave._id < 1000)
            {
                if (map.ContainsKey(wave.EnemyID) == false)
                {
                    int tmp = slove(_wave, wave.EnemyID);
                    map.Add(wave.EnemyID, tmp);

                    if (tmp >= 10)
                    {
                        if (PrintLog) Debug.LogError(string.Format("EnemyID {0} will preload {1}!!!", wave.EnemyID, tmp));
                    }
                }
            }
        }
        return map;
    }

}
