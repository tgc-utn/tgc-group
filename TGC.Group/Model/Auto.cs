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
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Sound;
using TGC.UtilsGroup;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model
{
    public class Auto
    {
        //Velocidad a partir de la cual se deforma el auto
        private const int VELOCIDAD_DEFORMACION = 300;

        //Altura del salto
        private const float ALTURA_SALTO = 95f;

        //Velocidad de movimiento del auto
        public float MOVEMENT_SPEED = 0f;

        //Rozamiento del piso
        private const float ROZAMIENTO = 100f;

        //Velocidad Maxima
        private const float MAX_SPEED = 800f;
        private const float MAX_SPEEDIA = 400f;

        //Velocidad de rotación del auto
        private const float ROTATION_SPEED = 0.3f;

        //Mesh del auto
        private TgcMesh Mesh { get; set; }

        //BoudingBox Obb del auto.
        public TgcBoundingOrientedBox ObbMesh;

        //Obbs del auto
        /*
        private TgcBoundingOrientedBox ObbArriba;
        private TgcBoundingOrientedBox ObbMedio;
        private TgcBoundingOrientedBox ObbAbajo;
		*/
        private TgcBoundingOrientedBox ObbArribaIzq;
        public TgcBoundingOrientedBox ObbArribaDer;
        private TgcBoundingOrientedBox ObbAbajoIzq;
        private TgcBoundingOrientedBox ObbAbajoDer;
        private TgcBoundingOrientedBox ObbArriba;
        private TgcBoundingOrientedBox ObbAbajo;
        private List<TgcBoundingOrientedBox> ObbLista;

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

        //Posicion de los Obb
        List<float> dxObb = new List<float> { 16, -16, -16, 16, 0, 0 };
        List<float> dyObb = new List<float> { -28, 30, -28, 30, 58, -58 };

        //Estado de salto
        private bool falling = false;
        private bool jumping = false;

        //Nro. de Jugador
        private int NroJugador;

        //MediaDir
        private string MediaDir;
        public Vector3 velocidad;
        private Vector3 OriginalPos;

        //Colisiono
        public bool colisiono { get; set; }
        public float ModificadorVida { get; set; }

        //Direccion para seguimiento
        public Vector3 direccionSeguir = new Vector3(0, 0, 0);
        public Vector3 direccionASeguir = new Vector3(0, 0, 0);
        public float desviacion = 0.25f;

        //Humo
        HumoParticula emisorHumo;

        //Choque
        ChoqueParticula emisorChoque;

        //Sonidos
        private string pathSonidoGolpe1;
        private string pathSonidoEncendido;
        private string pathSonidoPowerUpVida;
        private string pathSonidoMotor;
        private TgcStaticSound sound;
        private TgcStaticSound soundMotor;
        private TgcDirectSound DirectSound;

        //Deformacion
        private object vertices;
        public TgcMesh meshColisionado;
        public BoundingBox bB;
        List<KeyValuePair<string, object>> variablesDeInterfaz;
        private bool ColisionAdelanteIzq = false;
        private bool ColisionAdelanteDer = false;
        private bool ColisionAtrasIzq = false;
        private bool ColisionAtrasDer = false;
        private bool ColisionAdelanteIzqHabilitada = true;
        private bool ColisionAdelanteDerHabilitada = true;
        private bool ColisionAtrasIzqHabilitada = true;
        private bool ColisionAtrasDerHabilitada = true;

        public Auto(string MediaDir, int NroJugador, TgcDirectSound directSound)
        {
            this.NroJugador = NroJugador;
            this.MediaDir = MediaDir;
            this.colisiono = false;
            this.pathSonidoGolpe1 = MediaDir + "Sounds\\Golpe1.wav";
            this.pathSonidoEncendido = MediaDir + "Sounds\\Encendido.wav";
            this.pathSonidoPowerUpVida = MediaDir + "Sounds\\PowerUpVida.wav";
            this.pathSonidoMotor = MediaDir + "Sounds\\Motor1.wav";
            this.DirectSound = directSound;
        }

        public void RenderObb()
        {
            this.ObbMesh.render();
            this.ObbArriba.render();
            this.ObbArribaIzq.render();
            this.ObbArribaDer.render();
            this.ObbAbajo.render();
            this.ObbAbajoDer.render();
            this.ObbAbajoIzq.render();
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
            this.emisorHumo = new HumoParticula(this.MediaDir);
            this.emisorChoque = new ChoqueParticula(this.MediaDir);

            /*
            Vector3[] puntosDelAuto = this.Mesh.getVertexPositions();
            this.ObbMesh = TgcBoundingOrientedBox.computeFromPoints(puntosDelAuto);
            */

            /*   this.ObbArriba = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 40)).BoundingBox);
               this.ObbArriba.setRenderColor(System.Drawing.Color.IndianRed);
               this.ObbMedio = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 40)).BoundingBox);
               this.ObbMedio.setRenderColor(System.Drawing.Color.Green);
               this.ObbAbajo = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 40)).BoundingBox);
               this.ObbAbajo.setRenderColor(System.Drawing.Color.Blue);*/

            this.ObbArriba = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 3)).BoundingBox);
            this.ObbArriba.setRenderColor(System.Drawing.Color.IndianRed);

            this.ObbAbajo = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(55, 30, 3)).BoundingBox);
            this.ObbAbajo.setRenderColor(System.Drawing.Color.Blue);
            this.ObbArribaIzq = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(20, 30, 55)).BoundingBox);
            this.ObbArribaIzq.setRenderColor(System.Drawing.Color.Green);
            this.ObbArribaDer = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(20, 30, 55)).BoundingBox);
            this.ObbArribaDer.setRenderColor(System.Drawing.Color.Fuchsia);
            this.ObbAbajoIzq = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(20, 30, 55)).BoundingBox);
            this.ObbAbajoIzq.setRenderColor(System.Drawing.Color.Chocolate);
            this.ObbAbajoDer = TgcBoundingOrientedBox.computeFromAABB(TgcBox.fromSize(this.ObbMesh.Position, new Vector3(20, 30, 55)).BoundingBox);
            this.ObbAbajoDer.setRenderColor(System.Drawing.Color.Crimson);

            ObbLista = new List<TgcBoundingOrientedBox>();
            this.ObbLista.Add(ObbArribaDer);
            this.ObbLista.Add(ObbAbajoDer);
            this.ObbLista.Add(ObbArribaIzq);
            this.ObbLista.Add(ObbAbajoIzq);
            this.ObbLista.Add(ObbArriba);
            this.ObbLista.Add(ObbAbajo);

            //Para deformar el auto
            this.variablesDeInterfaz = new List<KeyValuePair<string, object>>();

            switch (this.Mesh.RenderType)
            {
                case TgcMesh.MeshRenderType.VERTEX_COLOR:
                    TgcSceneLoader.VertexColorVertex[] verts1 = (TgcSceneLoader.VertexColorVertex[])this.Mesh.D3dMesh.LockVertexBuffer(
                        typeof(TgcSceneLoader.VertexColorVertex), LockFlags.ReadOnly, this.Mesh.D3dMesh.NumberVertices);
                    this.Mesh.D3dMesh.UnlockVertexBuffer();
                    vertices = verts1;
                    break;

                case TgcMesh.MeshRenderType.DIFFUSE_MAP:
                    TgcSceneLoader.DiffuseMapVertex[] verts2 = (TgcSceneLoader.DiffuseMapVertex[])this.Mesh.D3dMesh.LockVertexBuffer(
                        typeof(TgcSceneLoader.DiffuseMapVertex), LockFlags.ReadOnly, this.Mesh.D3dMesh.NumberVertices);
                    vertices = verts2;
                    break;

                case TgcMesh.MeshRenderType.DIFFUSE_MAP_AND_LIGHTMAP:
                    TgcSceneLoader.DiffuseMapAndLightmapVertex[] verts3 = (TgcSceneLoader.DiffuseMapAndLightmapVertex[])this.Mesh.D3dMesh.LockVertexBuffer(
                        typeof(TgcSceneLoader.DiffuseMapAndLightmapVertex), LockFlags.ReadOnly, this.Mesh.D3dMesh.NumberVertices);
                    this.Mesh.D3dMesh.UnlockVertexBuffer();
                    vertices = verts3;
                    break;
            }

            //Se crea un boundingBox propio
            this.bB = new BoundingBox(TgcBoundingAxisAlignBox.computeFromPoints(this.Mesh.getVertexPositions()));
            this.variablesDeInterfaz.Add(new KeyValuePair<string, object>("beta", Math.Atan2(this.bB.PMax.Z, this.bB.PMax.X)));
            this.variablesDeInterfaz.Add(new KeyValuePair<string, object>("hip", Math.Sqrt(Math.Pow(this.bB.PMax.X, 2f) + Math.Pow(this.bB.PMax.Z, 2f))));
            this.variablesDeInterfaz.Add(new KeyValuePair<string, object>("radio", this.bB.PMax.Z));
            this.bB.scaleTranslate(this.Mesh.Position, new Vector3(1f, 1f, 1f));//Hay que situar el boundingBox donde esta posicionado la malla
            this.bB.setExtremes(this.bB.PMin, this.bB.PMax); //llevar a valores absolutos
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        public float GetRotationAngle()
        {
            return this.rotAngle;
        }

        public Vector3 GetPosition()
        {
            return this.Mesh.Position;
        }

        public void SetPositionMesh(Vector3 unVector, bool rotar)
        {
            this.Mesh.AutoTransformEnable = true;
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

        private void CalcularPosicionRuedas(bool MoverRuedas)
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
            float ro, alfa_OBB;
            float posicion_x;
            float posicion_y;

            //   this.ObbArribaIzq.Center = (new Vector3(posicion_xArribaIzq, 11.5f, posicion_yArribaIzq) + this.Mesh.Position);

            for (int i = 0; i < 6; i++)
            {
                ro = FastMath.Sqrt(dxObb[i] * dxObb[i] + dyObb[i] * dyObb[i]);

                alfa_OBB = FastMath.Asin(dxObb[i] / ro);

                if (i == 0 || i == 2 || i == 4)
                {
                    alfa_OBB += FastMath.PI;
                }

                posicion_x = FastMath.Sin(alfa_OBB + this.Mesh.Rotation.Y) * ro;
                posicion_y = FastMath.Cos(alfa_OBB + this.Mesh.Rotation.Y) * ro;

                this.ObbLista[i].Center = (new Vector3(posicion_x, 7.5f, posicion_y) + this.Mesh.Position);
                this.ObbLista[i].rotate((new Vector3(0, rotAngle, 0)));

            }

            /*
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
			*/
        }

        private Vector3[] dameRotados()
        {
            List<Vector3> points = new List<Vector3>();
            float rotacion = this.Mesh.Rotation.Y;

            double gama = (double)this.getValorDeVariable("beta") - rotacion;

            float xMax = (float)((double)this.getValorDeVariable("hip") * Math.Cos(gama));
            float zMax = (float)((double)this.getValorDeVariable("hip") * Math.Sin(gama));

            double betaPrima = -Math.PI + (double)this.getValorDeVariable("beta");

            gama = betaPrima - rotacion;

            float xMin = (float)((double)this.getValorDeVariable("hip") * Math.Cos(gama));
            float zMin = (float)((double)this.getValorDeVariable("hip") * Math.Sin(gama));

            points.Add(new Vector3(xMin, this.bB.PMin.Y, zMin));
            points.Add(new Vector3(xMax, this.bB.PMax.Y, zMax));

            return points.ToArray();
        }

        private void rotarBoundingBox()
        {
            Vector3[] rotados = this.dameRotados();

            TgcBoundingAxisAlignBox box = this.bB.Box;

            this.bB = new BoundingBox(rotados[0], rotados[1]);
            this.bB.Box = box;

            this.bB.scaleTranslate(this.Mesh.Position, new Vector3(1f, 1f, 1f));
            this.bB.setExtremes(this.bB.PMin, this.bB.PMax);
        }

        private void moverBoundingBox()
        {
            Vector3 moverse = new Vector3(0f, 0f, 0f);
            this.bB.scaleTranslate(moverse, new Vector3(1f, 1f, 1f));
            this.bB.setExtremes(this.bB.PMin, this.bB.PMax);
        }

        public void MoverAutoConColisiones(bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime)
        {
            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new Vector3(0, 0, 0);
            var moveForward = 0f;
            var rotating = false;
            var posicionAnterior = this.Mesh.Position;

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

            //Para que el auto no tiemble
            if (Math.Abs(moveForward) < 1)
            {
                moveForward = 0;
                this.MOVEMENT_SPEED = 0;
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
            this.OriginalPos = this.Mesh.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movement *= this.MOVEMENT_SPEED * ElapsedTime;

            rotacionVertical -= this.MOVEMENT_SPEED * ElapsedTime / 60;

            this.Mesh.moveOrientedY(moveForward * ElapsedTime);

            if (this.colisiono)
            {
                ReproducirSonidoChoque();
                colisionSimple(ElapsedTime, moveForward);
            }

            this.ColisionesObbObb(GameModel.ListaMeshAutos, ElapsedTime, moveForward);
            this.ObbMesh.Center = this.Mesh.Position;
            this.rotarBoundingBox();
            this.moverBoundingBox();
            
            direccionSeguir = new Vector3((-1) * FastMath.Sin(this.GetMesh().Rotation.Y), 0, (-1) * FastMath.Cos(this.GetMesh().Rotation.Y));
        }

        private void colisionSimple(float elapsedTime, float moveForward)
        {
            int x = 0, z = 0;
            Vector3 unaDiferencia = new Vector3(0, 0, 0);

            this.VerificarDeformacion(this.meshColisionado.BoundingBox);
            this.Mesh.Position = this.OriginalPos;
            this.MOVEMENT_SPEED = (-1) * Math.Sign(this.MOVEMENT_SPEED) * Math.Abs(500f * 0.2f);
            this.Mesh.moveOrientedY((-3.5f) * moveForward * elapsedTime);

            if (TgcCollisionUtils.testObbAABB(this.ObbMesh, this.meshColisionado.BoundingBox))
            {
                if (TgcCollisionUtils.testObbAABB(this.ObbArribaIzq, this.meshColisionado.BoundingBox) ||
                    TgcCollisionUtils.testObbAABB(this.ObbAbajoIzq, this.meshColisionado.BoundingBox)
                    )
                {
                    unaDiferencia = this.ObbArribaDer.Position - this.ObbArribaIzq.Position;
                }

                if (TgcCollisionUtils.testObbAABB(this.ObbArribaDer, this.meshColisionado.BoundingBox) ||
                    TgcCollisionUtils.testObbAABB(this.ObbAbajoDer, this.meshColisionado.BoundingBox)
                    )
                {
                    unaDiferencia = this.ObbArribaIzq.Position - this.ObbArribaDer.Position;
                }

                if (unaDiferencia.X > 0)
                    x = -20;
                else
                    x = 20;

                if (unaDiferencia.Z > 0)
                    z = -20;
                else
                    z = 20;

                unaDiferencia = unaDiferencia + new Vector3(x, 0, z);
                this.Mesh.move(unaDiferencia.X, 0, unaDiferencia.Z);
            }

            this.colisiono = false;
        }

        private void VerificarDeformacion(TgcBoundingAxisAlignBox a)
        {
            this.emisorChoque.SetInicioChoque(DateTime.Now);

            if (TgcCollisionUtils.testObbAABB(this.ObbArribaIzq, a))
            {
                this.ColisionAdelanteIzq = true;
                this.emisorChoque.SetChoqueAdelanteIzquierda();
                this.deformarMalla();
                return;
            }

            if (TgcCollisionUtils.testObbAABB(this.ObbArribaDer, a))
            {
                this.ColisionAdelanteDer = true;
                this.emisorChoque.SetChoqueAdelanteDerecha();
                this.deformarMalla();
                return;
            }

            if (TgcCollisionUtils.testObbAABB(this.ObbAbajoIzq, a))
            {
                this.ColisionAtrasIzq = true;
                this.emisorChoque.SetChoqueAtrasIzquierda();
                this.deformarMalla();
                return;
            }

            if (TgcCollisionUtils.testObbAABB(this.ObbAbajoDer, a))
            {
                this.ColisionAtrasDer = true;
                this.emisorChoque.SetChoqueAtrasDerecha();
                this.deformarMalla();
                return;
            }
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

        private bool ColisionesObbObb(List<Auto> ListaMesh, float elapsedTime, float movefoward)
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
                            //		respuestaColisionAutovsAuto(unMesh, elapsedTime, movefoward);
                            colisionEntreAutos(unMesh, elapsedTime);
                            return true;
                        }
                    }
                }
            }

            //No colisiono nada
            return false;
        }

        private void deformarMalla()
        {
            List<Vector3> corners = this.dameCorners(this.bB);
            Type tipo = this.vertices.GetType();
            int cantidadDeVertices = (int)tipo.GetProperty("Length").GetValue(this.vertices, null);
            System.Reflection.MethodInfo dameValorPorIndice = tipo.GetMethod("GetValue", new Type[] { typeof(int) });
            System.Reflection.MethodInfo insertaValorPorIndice = tipo.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) });

            if (Math.Abs(this.MOVEMENT_SPEED) > VELOCIDAD_DEFORMACION)
            {
                for (int j = 0; j < cantidadDeVertices; j++)
                {
                    object vertice = dameValorPorIndice.Invoke(this.vertices, new object[] { j });

                    Vector3 posicion = (Vector3)vertice.GetType().GetField("Position").GetValue(vertice);

                    if (this.ColisionAtrasIzq && this.ColisionAtrasIzqHabilitada)
                    {
                        if (posicion.Z > 40)
                        {
                            if (posicion.X > 10)
                            {
                                posicion.Z -= 7;
                                vertice.GetType().GetField("Position").SetValue(vertice, posicion);
                                insertaValorPorIndice.Invoke(this.vertices, new object[] { vertice, j });
                            }
                        }

                        continue;
                    }

                    if (this.ColisionAtrasDer && this.ColisionAtrasDerHabilitada)
                    {
                        if (posicion.Z > 40)
                        {
                            if (posicion.X < 5)
                            {
                                posicion.Z -= 12;
                                vertice.GetType().GetField("Position").SetValue(vertice, posicion);
                                insertaValorPorIndice.Invoke(this.vertices, new object[] { vertice, j });
                            }
                        }

                        continue;
                    }

                    if (this.ColisionAdelanteIzq && this.ColisionAdelanteIzqHabilitada)
                    {
                        if (posicion.Z < -30)
                        {
                            if (posicion.X > 10)
                            {
                                posicion.Z += 8;
                                vertice.GetType().GetField("Position").SetValue(vertice, posicion);
                                insertaValorPorIndice.Invoke(this.vertices, new object[] { vertice, j });
                            }
                        }

                        continue;
                    }

                    if (this.ColisionAdelanteDer && this.ColisionAdelanteDerHabilitada)
                    {
                        if (posicion.Z < -30)
                        {
                            if (posicion.X < 5)
                            {
                                posicion.Z += 12;
                                vertice.GetType().GetField("Position").SetValue(vertice, posicion);
                                insertaValorPorIndice.Invoke(this.vertices, new object[] { vertice, j });
                            }
                        }

                        continue;
                    }

                }

                this.Mesh.D3dMesh.SetVertexBufferData(this.vertices, LockFlags.None);

                if (this.ColisionAdelanteDer)
                {
                    this.ColisionAdelanteDer = false;
                    this.ColisionAdelanteDerHabilitada = false;
                }

                if (this.ColisionAdelanteIzq)
                {
                    this.ColisionAdelanteIzq = false;
                    this.ColisionAdelanteIzqHabilitada = false;
                }

                if (this.ColisionAtrasIzq)
                {
                    this.ColisionAtrasIzq = false;
                    this.ColisionAtrasIzqHabilitada = false;
                }

                if (this.ColisionAtrasDer)
                {
                    this.ColisionAtrasDer = false;
                    this.ColisionAtrasDerHabilitada = false;
                }

            }
        }

        private object getValorDeVariable(string key)
        {
            foreach (KeyValuePair<string, object> clave in variablesDeInterfaz)
            {
                if (clave.Key == key)
                    return clave.Value;
            }

            return null;
        }

        private List<Vector3> dameCorners(BoundingBox auto)
        {
            double beta = (double)this.getValorDeVariable("beta");
            double hip = (double)this.getValorDeVariable("hip");
            double rotacion = this.Mesh.Rotation.Y;
            Vector3 position = this.Mesh.Position;
            List<Vector3> corners = new List<Vector3>();

            double gama = -beta - rotacion;

            float xMax = (float)(hip * Math.Cos(gama));
            float zMin = (float)(hip * Math.Sin(gama));

            xMax = xMax + position.X;
            zMin = zMin + position.Z;

            corners.Add(new Vector3(xMax, auto.PMin.Y, zMin));

            gama = Math.PI - beta - rotacion;

            float xMin = (float)(hip * Math.Cos(gama));
            float zMax = (float)(hip * Math.Sin(gama));

            xMin = xMin + position.X;
            zMax = zMax + position.Z;

            corners.Add(new Vector3(xMin, auto.PMin.Y, zMax));

            corners.Add(auto.PMin);
            corners.Add(auto.PMax);

            return corners;
        }
	
		private void colisionSimpleCostados(float elapsedTime, float orientacion)
        {
            this.rotAngle = (80f * Math.Sign(orientacion) * elapsedTime) * (FastMath.PI / 50);
            this.Mesh.rotateY(rotAngle);
            this.ObbMesh.rotate(new Vector3(0, rotAngle, 0));
        }

        private float AnguloEntreVectores(Vector3 vector1, Vector3 vector2)
        {
            var v1x = vector1.X;
            var v1y = vector1.Y;
            var v1z = vector1.Z;
            var v2x = vector1.X;
            var v2y = vector2.Y;
            var v2z = vector2.Z;
            var dotProduct = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
            var modv1xv2 = FastMath.Sqrt(v1x * v1x + v1y * v1y + v1z * v1z) + FastMath.Sqrt(v2x * v2x + v2y * v2y + v2z * v2z);
            var angle = FastMath.Acos(dotProduct / modv1xv2);
            return angle;
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
         /*
             if (this.colisiono || this.ColisionesObbObb(GameModel.ListaMeshAutos))
                return true;
          */
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

        public void Seguir(Auto autoJugador, List<TgcMesh> MeshAABB, float ElapsedTime)
        {
            if (JugadorEncontrado(autoJugador))
            {
                this.AvanzarHastaJugador(MeshAABB, ElapsedTime);
                //UpdateIA(true, true, false, false, false, false, ElapsedTime, 225f); //Si el rayo proyectado impacta con el auto del jugador, el auto de la ia comienza a avanzar hacia su posicion
            }
            else
            {
                UpdateIA(true, true, false, false, true, false, ElapsedTime, 150f); //Si el rayo no impacta con el auto del jugador, el auto comienza a frenar mientras que gira en sentido horario intentando encontrar al auto del jugador
            }

        }

        public bool JugadorEncontrado(Auto autoJugador)
        {
            direccionSeguir = new Vector3((-1) * FastMath.Sin(this.GetMesh().Rotation.Y), 0, (-1) * FastMath.Cos(this.GetMesh().Rotation.Y));

            var origenIA = Mesh.Position;

            Vector3 origenJugador = autoJugador.Mesh.Position;

            direccionASeguir = new Vector3(origenJugador.X - origenIA.X, 0, origenJugador.Z - origenIA.Z);

            direccionASeguir.Normalize();

            direccionSeguir.Normalize();

            var xmin = direccionASeguir.X - desviacion;
            var xmax = direccionASeguir.X + desviacion;
            var zmin = direccionASeguir.Z - desviacion;
            var zmax = direccionASeguir.Z + desviacion;

            if (xmin <= direccionSeguir.X && direccionSeguir.X <= xmax)
            {
                if (zmin <= direccionSeguir.Z && direccionSeguir.Z <= zmax)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            /*var rayo = new TgcRay(origenIA, direccionSeguir);

            var lugarChoque = new Vector3(0, 0, 0);

            var obbJugador = autoJugador.ObbMesh;

            if (TgcCollisionUtils.intersectRayObb(rayo, obbJugador, out lugarChoque))
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }

        public void AvanzarHastaJugador(List<TgcMesh> MeshAABB, float ElapsedTime)
        {
            if (EncontroAABB(MeshAABB))
            {
                UpdateIA(true, true, false, false, true, false, ElapsedTime, 150f);
            }
            else
            {
                UpdateIA(true, true, false, false, false, false, ElapsedTime, 225f);
            }
        }

        private bool EncontroAABB(List<TgcMesh> meshAABB)
        {
            bool e = false;
            foreach (TgcMesh unMesh in meshAABB)
            {
                var posicion = new Vector3(this.Mesh.Position.X, this.Mesh.Position.Y, this.Mesh.Position.Z);
                var rayo = new TgcRay(posicion, direccionSeguir);
                var lugarChoque = new Vector3(0, 0, 0);
                if (TgcCollisionUtils.intersectRayAABB(rayo, unMesh.BoundingBox, out lugarChoque))
                {
                    var distancia = DistanciaEntre2Puntos(this.Mesh.Position, lugarChoque);
                    if (distancia < 100)
                    {
                        e = true;
                    }
                    else
                    {
                        e = false;
                    }
                }
                else
                {
                    e = false;
                }
            }
            return e;
        }

        public double DistanciaEntre2Puntos(Vector3 position, Vector3 lugarChoque)
        {
            float X = position.X - lugarChoque.X;
            float Z = position.Z - lugarChoque.Z;
            var XX = Math.Pow(X, 2);
            var ZZ = Math.Pow(Z, 2);
            var suma = XX + ZZ;
            var distancia = Math.Sqrt(suma);
            return distancia;
        }

        public void ReproducirSonidoMotor()
        {
            soundMotor = new TgcStaticSound();
            soundMotor.loadSound(pathSonidoMotor, DirectSound.DsDevice);
            soundMotor.play(true);
        }

        public void Update(bool MoverRuedas, bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime, float VidaJugador)
        {
            this.emisorChoque.Update(ElapsedTime, this.ObbAbajoDer.Position, this.Mesh.Rotation.Y);

            if (VidaJugador > 0)
            {
                this.MoverAutoConColisiones(Avanzar, Frenar, Izquierda, Derecha, Saltar, ElapsedTime);
            }
            else
            {
                this.MOVEMENT_SPEED = 0;
            }

            this.CalcularPosicionRuedas(MoverRuedas);
            this.CalcularPosObb();
            this.emisorHumo.Update(ElapsedTime, this.ObbAbajoDer.Position, this.Mesh.Rotation.Y);
        }

        public void UpdateIA(bool MoverRuedas, bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime, float aceleraciooon)
        {
            this.emisorChoque.Update(ElapsedTime, this.ObbAbajoDer.Position, this.Mesh.Rotation.Y);

            this.MoverAutoIAConColisiones(Avanzar, Frenar, Izquierda, Derecha, Saltar, ElapsedTime, aceleraciooon);
            this.CalcularPosicionRuedas(MoverRuedas);
            this.CalcularPosObb();
            this.emisorHumo.Update(ElapsedTime, this.ObbAbajoDer.Position, this.Mesh.Rotation.Y);
        }

        public void MoverAutoIAConColisiones(bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime, float aceleraciooon)
        {
            //Declaramos un vector de movimiento inicializado en cero.
            //El movimiento sobre el suelo es sobre el plano XZ.
            //Sobre XZ nos movemos con las flechas del teclado o con las letas WASD.
            var movement = new Vector3(0, 0, 0);
            var moveForward = 0f;
            var rotating = false;
            var posicionAnterior = this.Mesh.Position;
            var ROTATION_SPEED_IA = ROTATION_SPEED * 20f;

            rotate = 0;

            //Movernos de izquierda a derecha, sobre el eje X.
            if (Izquierda)
            {
                rotate = -ROTATION_SPEED_IA;
                rotating = true;
            }
            else if (Derecha)
            {
                rotate = ROTATION_SPEED_IA;
                rotating = true;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (Avanzar)
            {
                moveForward += -this.AcelerarIA(aceleraciooon, ElapsedTime);
            }

            if (Frenar)
            {
                //moveForward += -this.AcelerarIA(-250f, ElapsedTime);
            }

            //El auto dejo de acelerar e ira frenando de apoco 
            if (!Avanzar && !Frenar)
            {
                moveForward = -this.AcelerarIA(0f, ElapsedTime);
            }

            if (rotating)
            {
                if (this.MOVEMENT_SPEED <= (Auto.MAX_SPEEDIA / 2))
                    this.rotAngle = (this.MOVEMENT_SPEED * 0.2f * Math.Sign(rotate) * ElapsedTime) * (FastMath.PI / 50) * Math.Abs(ROTATION_SPEED_IA);
                else
                    this.rotAngle = (this.MOVEMENT_SPEED * 0.2f * Math.Sign(rotate) * ElapsedTime) * (FastMath.PI / 180) * Math.Abs(ROTATION_SPEED_IA);

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
            this.OriginalPos = this.Mesh.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movement *= this.MOVEMENT_SPEED * ElapsedTime;

            rotacionVertical -= this.MOVEMENT_SPEED * ElapsedTime / 60;

            this.Mesh.moveOrientedY(moveForward * ElapsedTime);

            if (this.colisiono)
            {
                colisionSimple(ElapsedTime, moveForward);
            }

            this.ColisionesObbObb(GameModel.ListaMeshAutos, ElapsedTime, moveForward);
            this.ObbMesh.Center = this.Mesh.Position;
            this.rotarBoundingBox();
            this.moverBoundingBox();

            direccionSeguir = new Vector3((-1) * FastMath.Sin(this.GetMesh().Rotation.Y), 0, (-1) * FastMath.Cos(this.GetMesh().Rotation.Y));
        }

        private float AcelerarIA(float aceleracion, float ElapsedTime)
        {
            if ((this.MOVEMENT_SPEED < MAX_SPEEDIA))
            {
                this.MOVEMENT_SPEED = MOVEMENT_SPEED + ((aceleracion + ObtenerRozamiento()) * ElapsedTime);

                if (this.MOVEMENT_SPEED > Math.Abs(MAX_SPEEDIA))
                    this.MOVEMENT_SPEED = MAX_SPEEDIA - 1;

                return this.MOVEMENT_SPEED;
            }
            else return this.MOVEMENT_SPEED;
        }

        private void loadSound(string filePath)
        {
            //Borrar sonido anterior
            if (sound != null)
            {
                sound.dispose();
                sound = null;
            }

            //Cargar sonido
            sound = new TgcStaticSound();
            sound.loadSound(filePath, DirectSound.DsDevice);
        }

        private void ReproducirSonidoChoque()
        {
            if (this.NroJugador == 0)
            {
                if (Math.Abs(this.MOVEMENT_SPEED) < Math.Abs((Auto.MAX_SPEED * 0.5)))
                {
                    loadSound(this.pathSonidoGolpe1);
                    sound.play();
                }

                else
                {
                    loadSound(this.pathSonidoGolpe1);
                    sound.play();
                    //TODO: Reproducir sonido de un golpe fuerte
                }

            }
        }

        public void ReproducirSonidoArranque()
        {
            loadSound(this.pathSonidoEncendido);
            sound.play();
        }

        public void ReproducirSonidoPowerUpVida()
        {
            loadSound(this.pathSonidoPowerUpVida);
            sound.play();
        }

        public void colisionEntreAutos(Auto otroAuto, float elapsedTime)
        {
            int x = 0, z = 0;
            Vector3 unaDiferencia = new Vector3(0, 0, 0);

            if (!TgcCollisionUtils.testObbObb(this.ObbArriba, otroAuto.ObbMesh))
            {
                this.ModificadorVida = -5f;
            }

            this.VerificarDeformacion(otroAuto.Mesh.BoundingBox);

            ReproducirSonidoChoque();

                if (this.MOVEMENT_SPEED >= 0)
                {
                    if (TgcCollisionUtils.testObbObb(this.ObbAbajoIzq, otroAuto.ObbArribaIzq) || TgcCollisionUtils.testObbObb(this.ObbAbajoIzq, otroAuto.ObbArribaDer)
                        || TgcCollisionUtils.testObbObb(this.ObbAbajoDer, otroAuto.ObbArribaIzq) || TgcCollisionUtils.testObbObb(this.ObbAbajoDer, otroAuto.ObbArribaDer))
                    {
                        //auto gira cierto angulo hacia izquierda y sube velocidad
                        if (Math.Truncate(this.MOVEMENT_SPEED) == 0)
                            this.MOVEMENT_SPEED = 300;
                        else
                            this.MOVEMENT_SPEED = Math.Abs(this.MOVEMENT_SPEED * 0.5f);
                        //otroAuto reduce bastante velocidad y se lo traslada un poco hacia atrás para no seguir chocando
                        otroAuto.GetMesh().moveOrientedY(10 * otroAuto.MOVEMENT_SPEED * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                        otroAuto.MOVEMENT_SPEED = -(otroAuto.MOVEMENT_SPEED * 0.3f);

                    }
                    else if (TgcCollisionUtils.testObbObb(otroAuto.ObbAbajoIzq, this.ObbArribaIzq ) || TgcCollisionUtils.testObbObb(otroAuto.ObbAbajoIzq, this.ObbArribaDer)
                        || TgcCollisionUtils.testObbObb(otroAuto.ObbAbajoDer, this.ObbArribaIzq ) || TgcCollisionUtils.testObbObb(otroAuto.ObbAbajoDer, this.ObbArribaDer))
                    {
                        //auto gira cierto angulo hacia derecha y sube velocidad
                        if (Math.Truncate(otroAuto.MOVEMENT_SPEED) == 0)
                            otroAuto.MOVEMENT_SPEED = 300;
                        else
                            otroAuto.MOVEMENT_SPEED = Math.Abs(otroAuto.MOVEMENT_SPEED * 0.5f);
                        //otroAuto reduce bastante velocidad y se lo traslada un poco hacia atrás para no seguir chocando
                        otroAuto.GetMesh().moveOrientedY(10 * this.MOVEMENT_SPEED * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                    }
                    else if (TgcCollisionUtils.testObbObb(this.ObbArribaIzq , otroAuto.ObbArribaIzq) || TgcCollisionUtils.testObbObb(this.ObbArribaDer, otroAuto.ObbArribaIzq) || TgcCollisionUtils.testObbObb( this.ObbArribaIzq , otroAuto.ObbArribaDer) || TgcCollisionUtils.testObbObb(this.ObbArribaDer , otroAuto.ObbArribaDer))
                    {
                        //auto se mueve hacia atrás y cambia de sentido 180° (rebota)
                        if (Math.Truncate(this.MOVEMENT_SPEED) == 0)
                            this.MOVEMENT_SPEED = -300;
                        else
                        {
                            this.GetMesh().moveOrientedY(10 * this.MOVEMENT_SPEED * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                            this.MOVEMENT_SPEED = -(this.MOVEMENT_SPEED * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                        }
                        //otroAuto hace lo mismo que el auto
                        otroAuto.GetMesh().moveOrientedY(10 * otroAuto.MOVEMENT_SPEED * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                        otroAuto.MOVEMENT_SPEED = -(otroAuto.MOVEMENT_SPEED * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                    }
                }
                else
                {
                    if (TgcCollisionUtils.testObbObb(this.ObbAbajoIzq, otroAuto.ObbArribaIzq) || TgcCollisionUtils.testObbObb(this.ObbAbajoIzq, otroAuto.ObbArribaDer) || TgcCollisionUtils.testObbObb(this.ObbAbajoDer, otroAuto.ObbArribaIzq) || TgcCollisionUtils.testObbObb(this.ObbAbajoDer, otroAuto.ObbArribaDer))
                    {
                        //ambos autos rebotan (cambia sentido 180°)
                        if (Math.Truncate(this.MOVEMENT_SPEED) == 0)
                            this.MOVEMENT_SPEED = 300;
                        else
                            this.MOVEMENT_SPEED = Math.Abs(this.MOVEMENT_SPEED * 0.5f);

                        otroAuto.GetMesh().moveOrientedY(10 * otroAuto.MOVEMENT_SPEED * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                        otroAuto.MOVEMENT_SPEED = -(otroAuto.MOVEMENT_SPEED * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                    }
                    else if (TgcCollisionUtils.testObbObb(this.ObbAbajoIzq, otroAuto.ObbAbajoIzq) || TgcCollisionUtils.testObbObb(this.ObbAbajoIzq, otroAuto.ObbAbajoDer) || TgcCollisionUtils.testObbObb(this.ObbAbajoDer, otroAuto.ObbAbajoIzq) || TgcCollisionUtils.testObbObb(this.ObbAbajoDer, otroAuto.ObbAbajoDer))
                    {
                        //rebota auto
                        this.GetMesh().moveOrientedY(10 * this.MOVEMENT_SPEED * elapsedTime); //Lo hago "como que rebote un poco" para no seguir colisionando
                        this.MOVEMENT_SPEED = -(this.MOVEMENT_SPEED * 0.3f); //Lo hago ir atrás un tercio de velocidad de choque
                    }
                }

                if (TgcCollisionUtils.testObbAABB(this.ObbMesh, otroAuto.Mesh.BoundingBox))
                {
                    if (TgcCollisionUtils.testObbAABB(this.ObbArribaIzq, otroAuto.Mesh.BoundingBox) ||
                        TgcCollisionUtils.testObbAABB(this.ObbAbajoIzq, otroAuto.Mesh.BoundingBox)
                        )
                    {
                        unaDiferencia = this.ObbArribaDer.Position - this.ObbArribaIzq.Position;
                    }

                    if (TgcCollisionUtils.testObbAABB(this.ObbArribaDer, otroAuto.Mesh.BoundingBox) ||
                        TgcCollisionUtils.testObbAABB(this.ObbAbajoDer, otroAuto.Mesh.BoundingBox)
                        )
                    {
                        unaDiferencia = this.ObbArribaIzq.Position - this.ObbArribaDer.Position;
                    }

                    if (unaDiferencia.X > 0)
                        x = -20;
                    else
                        x = 20;

                    if (unaDiferencia.Z > 0)
                        z = -20;
                    else
                        z = 20;

                    unaDiferencia = unaDiferencia + new Vector3(x, 0, z);
                    this.Mesh.move(unaDiferencia.X, 0, unaDiferencia.Z);
                }
            //}
        }

        public void Render()
        {
            this.Mesh.render();

            for (int i = 0; i < 4; i++)
            {
                this.RuedasJugador[i].render();
            }

            this.emisorHumo.Render();
            this.emisorChoque.Render();
        }

        public void Dispose()
        {
            foreach (var unaRueda in this.RuedasJugador)
            {
                unaRueda.dispose();
            }

            this.emisorHumo.Dispose();
            this.emisorChoque.Dispose();
        }
    }
}