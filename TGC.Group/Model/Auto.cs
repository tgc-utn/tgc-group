using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    class Auto
    {
        //Altura del salto
        private const float ALTURA_SALTO = 75f;

        //Velocidad de movimiento del auto
        private float MOVEMENT_SPEED = 0f;

        //Rozamiento del piso
        private const float ROZAMIENTO = 100f;

        //Velocidad Maxima
        private const float MAX_SPEED = 1200f;

        //Velocidad de rotación del auto
        private const float ROTATION_SPEED = 120f;

        //Mesh del auto
        private TgcMesh Mesh { get; set; }

        //BoudingBox Obb del auto.
        private TgcBoundingOrientedBox ObbMesh;

        //Ruedas
        TgcMesh ruedaDerechaDelanteraMesh;
        TgcMesh ruedaDerechaTraseraMesh;
        TgcMesh ruedaIzquierdaDelanteraMesh;
        TgcMesh ruedaIzquierdaTraseraMesh;

        //Lista de ruedas
        List<TgcMesh> RuedasJugador;

        //Rotación del auto
        float rotate = 0;
        float rotacionVertical = 0;
        float rotAngle = 0;

        //Posición de las ruedas
        List<float> dx = new List<float> { 23, -23, -23, 23 };
        List<float> dy = new List<float> { -30, 32, -31, 30 };

        //Estado de salto
        private bool falling = false;
        private bool jumping = false;

        //Nro. de Jugador
        private int NroJugador;

        //MediaDir
        private string MediaDir;

        public Auto(string MediaDir, int NroJugador)
        {
            this.NroJugador = NroJugador;
            this.MediaDir = MediaDir;
        }

        public void RenderObb()
        {
            this.ObbMesh.render();
        }

        public TgcMesh GetMesh()
        {
            return this.Mesh;
        }

        public Vector3 GetBBCenter()
        {
            return this.Mesh.BoundingBox.calculateBoxCenter();
        }

        public float GetBBRadius()
        {
            return this.Mesh.BoundingBox.calculateBoxRadius();
        }

        public void SetMesh(TgcMesh unMesh)
        {
            //Guardo el mesh
            this.Mesh = unMesh;

            //Movemos el auto un poco para arriba para que se pueda mover
            this.Mesh.AutoTransformEnable = true;
            this.Mesh.move(0, 0.5f, 0);
            this.Mesh.updateBoundingBox();

            //Cargo el bouding box obb del auto a partir de su AABB
            this.ObbMesh = TgcBoundingOrientedBox.computeFromAABB(Mesh.BoundingBox);
        }

        public float GetRotationAngle ()
        {
            return this.rotAngle;
        }

        public Vector3 GetPosition ()
        {
            return this.Mesh.Position;    
        }

        public void SetPositionMesh(Vector3 unVector, bool rotar)
        {
            if (rotar)
            {
                this.Mesh.Transform = this.Mesh.Transform * Matrix.RotationY(180 * (FastMath.PI / 180));
            }

            this.Mesh.Transform = this.Mesh.Transform * Matrix.Translation(unVector);
        }

        public void SetRuedas(TgcSceneLoader loader)
        {
            //Cargo las ruedas de los autos
            ruedaDerechaDelanteraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Derecha-TgcScene.xml").Meshes[0];
            ruedaDerechaTraseraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Derecha-TgcScene.xml").Meshes[0];
            ruedaIzquierdaDelanteraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Izquierda-TgcScene.xml").Meshes[0];
            ruedaIzquierdaTraseraMesh = loader.loadSceneFromFile(MediaDir + "Vehiculos\\Auto_Rueda_Izquierda-TgcScene.xml").Meshes[0];

            ruedaDerechaDelanteraMesh.AutoTransformEnable = true;
            ruedaDerechaDelanteraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaDerechaTraseraMesh.AutoTransformEnable = true;
            ruedaDerechaTraseraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaIzquierdaDelanteraMesh.AutoTransformEnable = true;
            ruedaIzquierdaDelanteraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            ruedaIzquierdaTraseraMesh.AutoTransformEnable = true;
            ruedaIzquierdaTraseraMesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);

            RuedasJugador = new List<TgcMesh> { ruedaDerechaDelanteraMesh, ruedaDerechaTraseraMesh, ruedaIzquierdaDelanteraMesh, ruedaIzquierdaTraseraMesh };
        }

        private void CalcularPosicionRuedas (bool MoverRuedas)
        {
            float ro, alfa_rueda, rotacionRueda;
            float posicion_x;
            float posicion_y;

            //Posiciono las ruedas
            for (int i = 0; i < 4; i++)
            {
                ro = FastMath.Sqrt(dx[i] * dx[i] + dy[i] * dy[i]);

                alfa_rueda = FastMath.Asin(dx[i] / ro);

                if (i == 0 || i == 2)
                {
                    alfa_rueda += FastMath.PI;
                }

                posicion_x = FastMath.Sin(alfa_rueda + Mesh.Rotation.Y) * ro;
                posicion_y = FastMath.Cos(alfa_rueda + Mesh.Rotation.Y) * ro;

                RuedasJugador[i].Position = (new Vector3(posicion_x, 7.5f, posicion_y) + Mesh.Position);

                rotacionRueda = 0;

                if (i == 0 || i == 2)
                {
                    rotacionRueda = 0.5f * Math.Sign(rotate);
                }

                //Si no aprieta para los costados, dejo la rueda derecha
                if (MoverRuedas)
                    RuedasJugador[i].Rotation = new Vector3(rotacionVertical, Mesh.Rotation.Y + rotacionRueda, 0f);
                else
                    RuedasJugador[i].Rotation = new Vector3(rotacionVertical, Mesh.Rotation.Y, 0f);
            }

            for (int i = 0; i < 4; i++)
            {
                RuedasJugador[i].render();
            }
        }

        public void MoverAutoConColisiones(bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime)
        {
            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new Vector3(0, 0, 0);
            var moveForward = 0f;
            var rotating = false;

            rotate = 0;

            //Movernos de izquierda a derecha, sobre el eje X.
            if (Izquierda)
            {
                rotate = -ROTATION_SPEED;
                rotating = true;
            }
            else if (Derecha)
            {
                rotate = ROTATION_SPEED;
                rotating = true;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (Avanzar)
            {
                moveForward += -this.Acelerar(200f, ElapsedTime);
            }

            if (Frenar)
            {
                moveForward += -this.Acelerar(-250f, ElapsedTime);
            }

            //El auto dejo de acelerar e ira frenando de apoco 
            if (!Avanzar && !Frenar)
            {
                moveForward = -this.Acelerar(0f, ElapsedTime);
            }

            if (rotating)
            {
                this.rotAngle = (this.MOVEMENT_SPEED * 0.2f * Math.Sign(rotate) * ElapsedTime) * (FastMath.PI / 180);
                this.Mesh.rotateY(rotAngle);
                this.ObbMesh.rotate(new Vector3(0, rotAngle, 0));
            }
            else
                rotAngle = 0;

            if (Saltar && !falling)
            {
                jumping = true;
            }

            if (jumping)
            {
                Mesh.move(0, 100 * ElapsedTime * 2, 0);

                if (Mesh.Position.Y >= ALTURA_SALTO)
                {
                    jumping = false;
                    falling = true;
                }
            }

            if (falling)
            {
                Mesh.move(0, -100 * ElapsedTime * 3, 0);

                if (Mesh.Position.Y < 0.5f)
                {
                    Mesh.move(0, 0.5f - Mesh.Position.Y, 0);
                    falling = false;
                }
            }

            //Guardar posicion original antes de cambiarla
            var originalPos = Mesh.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movement *= this.MOVEMENT_SPEED * ElapsedTime;

            rotacionVertical -= this.MOVEMENT_SPEED * ElapsedTime / 60;

            Mesh.moveOrientedY(moveForward * ElapsedTime);
            ObbMesh.Center = Mesh.Position;

            if (DetectarColisionesObb())
            {
                this.Mesh.Position = originalPos;
                this.MOVEMENT_SPEED = (-1) * Math.Sign(this.MOVEMENT_SPEED) * Math.Abs(this.MOVEMENT_SPEED * 0.3f);
                this.Mesh.moveOrientedY((-1) * moveForward * ElapsedTime);
            }
        }

        private bool DetectarColisionesObb()
        {
            List<bool> booleanosColision = new List<bool>();

            /*
            foreach (var unMesh in allMesh)
            {
                if ((unMesh != this.Mesh) && (unMesh.Name != "Room-1-Roof-0") && (unMesh.Name != "Room-1-Floor-0") &&
                    (unMesh.Name != "Pasto") && (unMesh.Name != "Plane_5")) //siempre que el mesh sea distinto al auto sino colisionara con el mismo
                    booleanosColision.Add(TgcCollisionUtils.testObbAABB(ObbMesh, unMesh.BoundingBox)); //me fijo si hubo alguna colision este booleano lo meto en una lista
            }
            */

            return booleanosColision.Find(valor => valor.Equals(true)); // me fijo si alguno de la lista dio true
        }

        private float Acelerar(float aceleracion, float ElapsedTime)
        {
            if ((this.MOVEMENT_SPEED < MAX_SPEED))
            {
                this.MOVEMENT_SPEED = MOVEMENT_SPEED + ((aceleracion + ObtenerRozamiento()) * ElapsedTime);

                if (this.MOVEMENT_SPEED > Math.Abs(MAX_SPEED))
                    this.MOVEMENT_SPEED = MAX_SPEED - 1;

                return this.MOVEMENT_SPEED;
            }
            else return this.MOVEMENT_SPEED;
        }

        private float ObtenerRozamiento()
        {
            if (this.MOVEMENT_SPEED > 0)
                return (-ROZAMIENTO);

            if (this.MOVEMENT_SPEED < 0)
                return ROZAMIENTO;

            return 0;
        }

        public void Update(bool MoverRuedas, bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime)
        {
            this.CalcularPosicionRuedas(MoverRuedas);
            this.MoverAutoConColisiones(Avanzar, Frenar, Izquierda, Derecha, Saltar, ElapsedTime);
        }

        public void Render()
        {
            Mesh.render();

            for (int i = 0; i < 4; i++)
            {
                RuedasJugador[i].render();
            }
        }

        public void Dispose()
        {
            foreach (var unaRueda in RuedasJugador)
            {
                unaRueda.dispose();
            }
        }
    }
}
