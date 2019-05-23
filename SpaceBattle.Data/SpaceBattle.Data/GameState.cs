using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Data.ClientInteraction;

namespace SpaceBattle.Data
{
    public class GameState
    {/*
        //TODO: перенести в клиента
        //public const int ElementSize = 32;
        public readonly Random Rnd = new Random();

        //public static IGameObject[,] Map { get; private set; }
        public List<IEntity> ExistingEntities { get; private set; }
        public List<EntityAnimation> Animations;
        public readonly int MapWidth;// => Map.GetLength(0);
        public readonly int MapHeight;// => Map.GetLength(1);
        public bool IsInsideGameField(int x, int y) => x >= 0 && x < MapWidth && y >= 0 && y < MapHeight;
        public bool IsInsideGameField(Point position) => IsInsideGameField(position.X, position.Y);

        public int MaxDifficulty { get; private set; }
        public int CurrentDifficulty { get; private set; }
        public int DifficultyIncreaseRate { get; private set; }
        public double PercentageOfShieldPenetration = 0.1;//=> (MaxDifficulty / 10d) / 100d;

        public GameActCommands Commands;


        public bool IsStarted;
        public bool InAsteroidField;
        public bool IsOver;
        public bool Win;
        public bool PlayerIsShielded => KeyPressed == Keys.Down;

        public Player PlayerObject { get; private set; }
        public int MaxPlayerHealth => 100;
        public Boss BossObject { get; private set; }
        public int MaxBossHealth => MaxDifficulty;

        public GameState()
        {
            
        }*/
    }
}