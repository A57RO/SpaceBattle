using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Data.ClientInteraction;
using SpaceBattle.Data.Entities;

namespace SpaceBattle.Data
{
    public static class GameEngine
    {
        public static void BeginAct(GameState redSide, GameState blueSide)
        {

        }
    }

    public class GameState
    {
        //public IDifficulty Difficulty { get; private set; }
        //public static IGameObject[,] Map { get; private set; }
        public readonly int MapWidth;// => Map.GetLength(0);
        public readonly int MapHeight;// => Map.GetLength(1);

        public List<IEntity> ExistingEntities { get; internal set; }
        public Player PlayerEntity { get; internal set; }

        public List<EntityAnimation> Animations { get; internal set; }
        public GameActCommands Commands { get; internal set; }

        public bool IsInsideGameField(int x, int y) => x >= 0 && x < MapWidth && y >= 0 && y < MapHeight;
        public bool IsInsideGameField(Point position) => IsInsideGameField(position.X, position.Y);
        
        public bool IsStarted { get; internal set; }
        public bool IsOver { get; internal set; }
        public bool IsWin { get; internal set; }
        
        public GameState(bool playerColorIsBlue, int mapWidth, int mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            PlayerEntity = new Player(new Point(mapWidth / 2, 0));
            ExistingEntities = new List<IEntity> { PlayerEntity };
        }
    }
}