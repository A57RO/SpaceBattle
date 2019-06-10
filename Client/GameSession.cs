using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Forms;
using GameData;
using GameData.ClientInteraction;

namespace Client
{
    public class GameSession
    {
        public readonly GameForm GameForm;
        private readonly GameState topSideState;
        private readonly GameState bottomSideState;
        private readonly ControlSettings controlSettings;
        private int tickCount = 0;

        public GameSession(ControlSettings controlSettings, bool playerIsRed, int mapWidth, int bottomMapHeight, int topSideMapHeight)
        {
            this.controlSettings = controlSettings;
            bottomSideState = new GameState(bottomMapHeight, mapWidth);
            topSideState = new GameState(topSideMapHeight, mapWidth);

            GameForm = new GameForm(playerIsRed, mapWidth, bottomMapHeight + topSideMapHeight);

            //var tickCount = 0;
            /*
            var tickFunc = new TimerCallback(OnTick);
            var timer = new System.Threading.Timer(tickFunc, null, 0, 10);
            */

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += OnTick;
            timer.Start();

        }

        private void OnTick(object sender, EventArgs e)//object obj)
        {
            if (tickCount == 0)
            {
                bottomSideState.GiveCommandsFromClient(new GameActCommands(controlSettings, GameForm.PressedKeys));
                lock (bottomSideState.Animations)
                    GameEngine.BeginAct(bottomSideState);
                lock (GameForm.BottomSideHUD)
                {
                    GameForm.BottomSideHUD.Clear();
                    if (!bottomSideState.GameOver)
                        Visual.UpdatePlayerHUD(GameForm.BottomSideHUD, bottomSideState.PlayerEntity, GameForm.ClientSize, true);
                }
                if (!bottomSideState.GameOver)
                    Sound.PlaySoundsAtBeginAct(bottomSideState.PlayerEntity);

                lock (topSideState.Animations)
                    GameEngine.BeginAct(topSideState);
                lock (GameForm.TopSideHUD)
                {
                    GameForm.TopSideHUD.Clear();
                    if (!topSideState.GameOver)
                        Visual.UpdatePlayerHUD(GameForm.TopSideHUD, topSideState.PlayerEntity, GameForm.ClientSize, false);
                }
            }

            lock (GameForm.BottomSideField)
            {
                GameForm.BottomSideField.Clear();
                Visual.UpdateDrawingElements(GameForm.BottomSideField, bottomSideState, true, GameForm.BottomIsRed, tickCount);
            }
            lock (GameForm.TopSideField)
            {
                GameForm.TopSideField.Clear();
                Visual.UpdateDrawingElements(GameForm.TopSideField, topSideState, false, !GameForm.BottomIsRed, tickCount);
            }

            tickCount++;
            if (tickCount == 9)
            {
                lock (bottomSideState.Animations)
                    GameEngine.EndAct(bottomSideState, topSideState);
                if (!bottomSideState.GameOver)
                    Sound.PlaySoundsAtEndAct(bottomSideState.PlayerEntity);
                tickCount = 0;
            }

            GameForm.Invalidate();
        }
    }
}