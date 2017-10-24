using Level;
using System.Collections.Generic;
using UnityEngine;

namespace XEditor
{
    public class LevelEntityStatistics
    {
        /// <summary>
        /// 根据wave 最大的
        /// </summary>
        public static Dictionary<int, int> suggest = new Dictionary<int, int>();
        /// <summary>
        /// 统计
        /// </summary>
        public static Dictionary<int, int> statistics = new Dictionary<int, int>();

    
        public static void CulWaves(List<EditorWave> waves)
        {
            suggest.Clear();
            statistics.Clear();
            foreach (EditorWave wave in waves)
            {
                if (wave.SpawnType == LevelSpawnType.Spawn_Buff) continue;
                if (wave.ID < 1000)
                {
                    if (!suggest.ContainsKey(wave.uid))
                    {
                        suggest.Add(wave.uid, wave.Count);
                        statistics.Add(wave.uid, wave.Count);
                    }
                    else
                    {
                        suggest[wave.uid] = Mathf.Max(suggest[wave.uid], wave.Count);
                        statistics[wave.uid] += wave.Count;
                    }
                }
            }
        }
    }
}

