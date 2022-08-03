using System;

namespace MinecraftDB
{
    internal static class Program{
        private static void Main(string[] args)
        {
            var world = new WorldDatabase("E:\\Dokumente\\MCWelten\\world");
            world.Start();
        }
    }
}

