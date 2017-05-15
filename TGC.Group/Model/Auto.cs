using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    public class Auto
    {
        //Altura del salto
        private const float ALTURA_SALTO = 95f;

        //Velocidad de movimiento del auto
        private float MOVEMENT_SPEED = 0f;

        //Rozamiento del piso
        private const float ROZAMIENTO = 100f;

        //Velocidad Maxima
        private const float MAX_SPEED = 1000f;

        //Velocidad de rotación del auto
        private const float ROTATION_SPEED = 0.3f;

        //Mesh del auto
        private TgcMesh Mesh { get; set; }

        //BoudingBox Obb del auto.
        public TgcBoundingOrientedBox ObbMesh;

        //Obbs del auto
        private TgcBoundingOrientedBox ObbArriba;
        private TgcBoundingOrientedBox ObbMedio;
        private TgcBoundingOrientedBox ObbAbajo;

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

        //Colisiono
        public bool colisiono { get; set; }
        public float pesoImpacto { get; set; }

        public Auto(string MediaDir, int NroJugador)
        {
            this.NroJugador = NroJugador;
            this.MediaDir = MediaDir;
            this.colisiono = false;
        }

        public void RenderObb()
        {
            this.ObbMesh.render();
            this.ObbArriba.render();
            this.ObbMedio.render();
            this.ObbAbajo.render();
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
            this.ObbMesh = TgcBoundingOrientedBox.computeFromAABB(this.Mesh.BoundingBox);
            this.ObbArriba = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 40)).BoundingBox);
            this.ObbArriba.setRenderColor(System.Drawing.Color.IndianRed);
            this.ObbMedio = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 40)).BoundingBox);
            this.ObbMedio.setRenderColor(System.Drawing.Color.Green);
            this.ObbAbajo = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 40)).BoundingBox);
            this.ObbAbajo.setRenderColor(System.Drawing.Color.Blue);
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
            this.Mesh.AutoTransformEnable = false;
            this.Mesh.AutoUpdateBoundingBox = true;

            if (rotar)
            {
                this.Mesh.Transform = this.Mesh.Transform * Matrix.RotationY(180 * (FastMath.PI / 180));
            }

            this.Mesh.Transform = this.Mesh.Transform * Matrix.Translation(unVector);
            this.Mesh.BoundingBox.transform(this.Mesh.Transform);

            //Cargo el bouding box obb del auto a partir de su AABB
            this.ObbMesh = TgcBoundingOrientedBox.computeFromAABB(this.Mesh.BoundingBox);
            this.Mesh.Position = unVector;
            
            /*this.Mesh.AutoTransformEnable = true;
            this.Mesh.move(unVector);
            this.Mesh.UpdateMeshTransform();
            this.Mesh.updateBoundingBox();
            this.ObbMesh = TgcBoundingOrientedBox.computeFromAABB(this.Mesh.BoundingBox);
            this.ObbMesh.updateValues();*/

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

                posicion_x = FastMath.Sin(alfa_rueda + this.Mesh.Rotation.Y) * ro;
                posicion_y = FastMath.Cos(alfa_rueda + this.Mesh.Rotation.Y) * ro;

                this.RuedasJugador[i].Position = (new Vector3(posicion_x, 7.5f, posicion_y) + this.Mesh.Position);

                rotacionRueda = 0;

                if (i == 0 || i == 2)
                {
                    rotacionRueda = 0.5f * Math.Sign(rotate);
                }

                //Si no aprieta para los costados, dejo la rueda derecha
                if (MoverRuedas)
                    this.RuedasJugador[i].Rotation = new Vector3(rotacionVertical, this.Mesh.Rotation.Y + rotacionRueda, 0f);
                else
                    this.RuedasJugador[i].Rotation = new Vector3(rotacionVertical, this.Mesh.Rotation.Y, 0f);
            }
        }

        public void CalcularPosObb() //funcion medio rara que calcula posicion de los 3 obb meidante cordenadas polares
        {
            float distanciaObb = 40;
            float alfaObbArriba = FastMath.PI;
            float alfaObbAbajo = 0;

            float posicion_xArriba = FastMath.Sin(alfaObbArriba + this.Mesh.Rotation.Y) * distanciaObb;
            float posicion_yArriba = FastMath.Cos(alfaObbArriba + this.Mesh.Rotation.Y) * distanciaObb;

            float posicion_xAbajo = FastMath.Sin(alfaObbAbajo + this.Mesh.Rotation.Y) * distanciaObb;
            float posicion_yAbajo = FastMath.Cos(alfaObbAbajo + this.Mesh.Rotation.Y) * distanciaObb;
         
            this.ObbArriba.Center = (new Vector3(posicion_xArriba, 0, posicion_yArriba) + this.Mesh.Position);
            this.ObbArriba.rotate((new Vector3(0, rotAngle, 0)));
            this.ObbMedio.Center = (this.Mesh.Position);
            this.ObbMedio.rotate((new Vector3(0, rotAngle, 0)));
            this.ObbAbajo.Center = (new Vector3(posicion_xAbajo, 0, posicion_yAbajo) + this.Mesh.Position);
            this.ObbAbajo.rotate((new Vector3(0, rotAngle, 0)));
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
                if (this.MOVEMENT_SPEED <= (Auto.MAX_SPEED / 2))
                    this.rotAngle = (this.MOVEMENT_SPEED * 0.2f * Math.Sign(rotate) * ElapsedTime) * (FastMath.PI / 50) * Math.Abs(ROTATION_SPEED);
                else
                    this.rotAngle = (this.MOVEMENT_SPEED * 0.2f * Math.Sign(rotate) * ElapsedTime) * (FastMath.PI / 180) * Math.Abs(ROTATION_SPEED);

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
            var originalPos = this.Mesh.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movement *= this.MOVEMENT_SPEED * ElapsedTime;

            rotacionVertical -= this.MOVEMENT_SPEED * ElapsedTime / 60;

            this.Mesh.moveOrientedY(moveForward * ElapsedTime);

            if (this.DetectarColisionesObb())
            {
                this.Mesh.Position = originalPos;
                this.MOVEMENT_SPEED = (-1) * Math.Sign(this.MOVEMENT_SPEED) * Math.Abs(this.MOVEMENT_SPEED * 0.2f);
                this.Mesh.moveOrientedY((-3.5f) * moveForward * ElapsedTime);

                this.colisiono = false;
            }

            this.ObbMesh.Center = this.Mesh.Position;
        }

        private bool ColisionesObb(List<TgcMesh> ListaMesh)
        {
            //Me fijo si colisiona con algo
            if (ListaMesh != null)
            {
                foreach (var unMesh in ListaMesh)
                {
                    if ((unMesh.Name != "Room-1-Roof-0") && (unMesh.Name != "Room-1-Floor-0") &&
                        (unMesh.Name != "Pasto") && (unMesh.Name != "Plane_5"))
                    {
                        //me fijo si hubo alguna colision vuelvo
                        if (TgcCollisionUtils.testObbAABB(this.ObbMesh, unMesh.BoundingBox))
                        {
                            return true;
                        }
                    }
                }
            }

            //No colisiono nada
            return false;
        }

        private bool ColisionesObbObb(List<Auto> ListaMesh)
        {
            //Me fijo si colisiona con algo
            if (ListaMesh != null)
            {
                foreach (var unMesh in ListaMesh)
                {
                    if (unMesh.GetMesh() != this.Mesh)
                    {
                        //me fijo si hubo alguna colision vuelvo
                        if (TgcCollisionUtils.testObbObb(this.ObbMesh.toStruct(), unMesh.ObbMesh.toStruct()))
                        {
                            return true;
                        }
                    }
                }
            }

            //No colisiono nada
            return false;
        }        

        private bool DetectarColisionesObb()
        {/*
            if (this.ColisionesObb(GameModel.ScenePpal.Meshes))
                return true;

            if (this.ColisionesObb(GameModel.MeshPinos))
                return true;

            if (this.ColisionesObb(GameModel.MeshRocas))
                return true;

            if (this.ColisionesObb(GameModel.MeshPalmeras))
                return true;

            if (this.ColisionesObb(GameModel.MeshArbolesBananas))
                return true;
        */
            if (this.colisiono || this.ColisionesObbObb(GameModel.ListaMeshAutos))
               return true;

            return false;
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
            this.MoverAutoConColisiones(Avanzar, Frenar, Izquierda, Derecha, Saltar, ElapsedTime);
            this.CalcularPosicionRuedas(MoverRuedas);
            this.CalcularPosObb();     
        }

        public void Render()
        {
            this.Mesh.render();

            for (int i = 0; i < 4; i++)
            {
                this.RuedasJugador[i].render();
            }
        }

        public void Dispose()
        {
            foreach (var unaRueda in this.RuedasJugador)
            {
                unaRueda.dispose();
            }
        }
    }
}
