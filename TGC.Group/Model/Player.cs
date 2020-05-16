using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Crafting;

namespace TGC.Group.Model
{
    class Player
    {
        //Gameplay vars
        private Inventory inventory = new Inventory();
        private float oxygen = 100f;
        private float health = 100f;
        private bool estaEnNave;

        private const float OXYGEN_LOSS_SPEED = 10f;
        private const float OXYGEN_RECOVER_SPEED = OXYGEN_LOSS_SPEED * 3.2f;
        private const float OXYGEN_DAMAGE = 5f;
        private const float WATER_LEVEL = 10f; //When players reaches a position above this level, then recovers oxygen.

        TgcD3dInput Input;

        //Dev vars
        private bool godmode = false;

        
        //Transformations vars
        private TGCBox mesh { get; set; }
        private TGCVector3 size = new TGCVector3(2, 5, 2);
        private TGCQuaternion rotation = TGCQuaternion.Identity;

        //Config vars
        private float speed = 25f; //foward and horizontal speed
        private float vspeed = 10f; //vertical speed


        public Player(TgcD3dInput Input) { this.Input = Input; }

        //Tgc functions

        public TGCVector3 Position() { return mesh.Position; }
        public TgcBoundingAxisAlignBox BoundingBox() { return mesh.BoundingBox; }

        public void InitMesh() { mesh = TGCBox.fromSize(size, null); }

        /// <summary>
        /// Update del jugador cuando esta FUERA de la nave.
        /// </summary>
        public void Update(FPSCamara Camara, float ElapsedTime) {
            estaEnNave = false;
            CheckInputs(Camara, ElapsedTime, null);
            GameplayUpdate(ElapsedTime);
            UpdateTransform();
        }

        /// <summary>
        /// Update del jugador cuando esta DENTRO de la nave.
        /// </summary>
        public void Update(FPSCamara Camara, float ElapsedTime, List<TgcMesh> Paredes)
        {
            estaEnNave = true;
            CheckInputs(Camara, ElapsedTime, Paredes);
            GameplayUpdate(ElapsedTime);
            UpdateTransform();
        }

        public void Render() { }

        private void CheckInputs(FPSCamara Camara, float ElapsedTime, List<TgcMesh> Paredes)
        {
            Console.WriteLine(estaEnNave);
            int w = Input.keyDown(Key.W) ? 1 : 0;
            int s = Input.keyDown(Key.S) ? 1 : 0;
            int d = Input.keyDown(Key.D) ? 1 : 0;
            int a = Input.keyDown(Key.A) ? 1 : 0;
            int space = Input.keyDown(Key.Space) ? 1 : 0;
            int ctrl = Input.keyDown(Key.LeftControl) ? 1 : 0;

            float fmov = w - s; //foward movement
            float hmov = a - d; //horizontal movement
            float vmov = space - ctrl; //vertical movement

            //Check for in-ship movement            
            var LookDir = Camara.LookDir();
            var LeftDir = Camara.LeftDir();

            if (estaEnNave)
            {
                LookDir.Y = 0;
                LeftDir.Y = 0;
                vmov = 0;
            }

            //Move player
            TGCVector3 movement = LookDir * fmov * speed + Camara.LeftDir() * hmov * speed + TGCVector3.Up * vmov * vspeed;
            movement *= ElapsedTime;

            if (estaEnNave && Paredes != null)
                Move(movement, Paredes);
            else
                Move(movement);

            //Dev
            bool p = Input.keyDown(Key.P);
            if (p) { godmode = !godmode; GodMode(godmode); }
        }

        private void Move(TGCVector3 movement) { mesh.Position += movement; }
        private void Move(TGCVector3 movement, List<TgcMesh> Paredes)
        {
            TGCVector3 lastPos = mesh.Position;
            Move(movement);

            //Check for collisions
            bool collided = false;
            foreach (var pared in Paredes)
            {
                if (TgcCollisionUtils.testAABBAABB(mesh.BoundingBox, pared.BoundingBox))
                {
                    collided = true;
                    break;
                }
            }
            //If any collision then go to last position.
            if (collided)
                mesh.Position = lastPos;

        }

        public void Dispose() { mesh.Dispose(); }

        public void UpdateTransform() { mesh.Transform = TGCMatrix.Scaling(mesh.Scale) * TGCMatrix.Translation(mesh.Position); }   


        //Gameplay functions
        private void GameplayUpdate(float ElapsedTime)
        {
            if (!godmode)
            {
                if (IsOutsideWater()) RecoverOxygen(ElapsedTime); else LoseOxygen(ElapsedTime);
            }
        }

        private void LoseOxygen(float ElapsedTime) { 
            oxygen = Math.Max(0, oxygen - OXYGEN_LOSS_SPEED * ElapsedTime);
            if (oxygen == 0) GetDamage(OXYGEN_DAMAGE * ElapsedTime);
        }

        public void GetHeal(float amount) { health = Math.Min(100f, health + amount); }
        public void GetDamage(float amount) { health = Math.Max(0, health - amount); }
        private void RecoverOxygen(float ElapsedTime) { oxygen = Math.Min(100, oxygen + OXYGEN_RECOVER_SPEED * ElapsedTime); }
        private bool IsOutsideWater() { return estaEnNave || mesh.Position.Y > WATER_LEVEL; }

        public float Oxygen() { return oxygen; }
        public float Health() { return health; }

        //Dev functions
        private void GodMode(bool enabled)
        {
            if (enabled)
            {
                health = 100f;
                oxygen = 100f;
            }
        }
    }
}
