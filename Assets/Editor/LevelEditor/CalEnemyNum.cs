using Level;
using System;
using System.Collections.Generic;


namespace XEditor
{
    public class CalEnemyNum
    {
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

        private int Slove(List<EditorWave> waves, uint entityid)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            int sum = 0;
            int ans = 0;
            int Sid = 0;
            n = 0;
            foreach (EditorWave wave in waves)
            {
                if (wave.ID < 1000)
                {
                    ++n;
                    if (!map.ContainsKey(wave.ID))
                    {
                        map.Add(wave.ID, ++Sid);
                    }
                }
            }
            p.Clear();
            cnt = 0;
            int b = 0;
            int t = n + n + n + 1;
            NN = n + n + n + 2;
            next.Clear();
            for (int i = 0; i <= NN; ++i) next.Add(-1);
            foreach (EditorWave wave in waves)
            {
                if (wave.ID >= 1000) continue;
                int count = wave._prefabSlot.Count + wave.RoundCount;
                int id = map[wave.ID];
                if (wave.EntityID == entityid)
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

                if (wave.preWaves == null) continue;
                List<string> preWave = new List<string>(wave.preWaves.Split(','));
                foreach (string pre in preWave)
                {
                    if (pre.Length == 0) continue;

                    int tmp;
                    if (int.TryParse(pre, out tmp))
                    {
                        if (!map.ContainsKey(int.Parse(pre)))
                        {
                            continue;
                        }
                        else
                        {
                            add(id, map[int.Parse(pre)] + n + n, inf);
                        }
                    }
                }
            }
            ans = isap(b, t);
            return sum - ans;
        }

        public Dictionary<uint, int> CalNum(List<EditorWave> waves)
        {
            Dictionary<uint, int> map = new Dictionary<uint, int>();
            foreach (EditorWave wave in waves)
            {
                if (wave.SpawnType == LevelSpawnType.Spawn_Buff) continue;
                if (wave.ID < 1000)
                {
                    if (map.ContainsKey(wave.EntityID) == false)
                    {
                        int tmp = Slove(waves, wave.EntityID);
                        map.Add(wave.EntityID, tmp);
                    }
                }
            }
            return map;
        }
    }

}