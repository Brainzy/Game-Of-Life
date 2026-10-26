using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameOfLife.Editor
{
    public static class LifeMenu
    {
        [MenuItem("Tools/Game of Life/Run 10 Generations")]
        public static void Run()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            string output = Path.Combine(root, "output.txt");
            int count = LifeFile.RunFile(Path.Combine(root, "input.txt"), output);
            Debug.Log($"Game of Life: completed 10 generations; wrote {count} live cells to {output}");
        }
    }
}
