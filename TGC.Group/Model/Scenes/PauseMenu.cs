using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Input;
using TGC.Core.Text;
using TGC.Core.Direct3D;
using Microsoft.DirectX;
using TGC.Core.Mathematica;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Scenes
{
    class PauseMenu : Scene
    {
        public delegate void Callback();
        private Callback preRender = () => {};
        private Callback onReturnToGameCallback = () => {}, onGoToStartMenuCallback = () => {};

        TgcText2D textBig = new TgcText2D(), textSmall = new TgcText2D();

        enum Pointer
        {
            UP,
            DOWN
        }
        Pointer pointer = Pointer.UP;
        public PauseMenu(TgcD3dInput input) : base(input)
        {
            _uses3DCamera = false;
            textBig.changeFont(new System.Drawing.Font("Arial Black", 40f));
            textSmall.changeFont(new System.Drawing.Font("Arial Black", 20f));
        }
        public override void Update()
        {
            if (Input.keyPressed(Key.Return))
            {
                Decide();
                pointer = Pointer.UP;
            }
            if (Input.keyPressed(Key.Escape))
            {
                onReturnToGameCallback();
                pointer = Pointer.UP;
            }
            if (Input.keyPressed(Key.UpArrow))
            {
                pointer = Pointer.UP;
            }
            if (Input.keyPressed(Key.DownArrow))
            {
                pointer = Pointer.DOWN;
            }
        }
        public override void Render()
        {
            preRender();

            textBig.drawText("PAUSE", 580, 250, Color.DarkGray);
            textSmall.drawText("BACK TO THE GAME", 530, 500, pointer == Pointer.UP ? Color.White : Color.DarkGray);
            textSmall.drawText("GO TO START MENU", 530, 550, pointer == Pointer.UP ? Color.DarkGray : Color.White);
            textSmall.drawText("->", 480, 500 + (int)pointer * 50, Color.White);
        }
        public PauseMenu OnReturnToGame(Callback onEnterCallback)
        {
            this.onReturnToGameCallback = onEnterCallback;
            return this;
        }
        public PauseMenu OnGoToStartMenu(Callback onEscapeCallback)
        {
            this.onGoToStartMenuCallback = onEscapeCallback;
            return this;
        }
        private void Decide()
        {
            (pointer == Pointer.UP ? onReturnToGameCallback : onGoToStartMenuCallback)();
        }
        public PauseMenu WithPreRender(Callback preRender)
        {
            this.preRender = preRender;
            return this;
        }
    }
}
