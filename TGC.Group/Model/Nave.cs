using TGC.Core.Mathematica;
using TGC.Core.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace TGC.Group.Model
{
    public class Nave : IRenderizable
    {
        private TGCVector3 posicion; 
        private readonly ModeloCompuesto modeloNave;
        private readonly InputDelJugador input;
        private readonly float velocidadBase;
        private float velocidadActual;
        private readonly float aceleracionMovimiento;
        private TGCVector3 rotacionBase;
        private TGCVector3 rotacionActual;
        private readonly float velocidadRotacion;
        private bool estaRolleando;
        private bool estaVivo;
        private TgcText2D textoGameOver;
        private string mediaDir;
        private float segundosDesdeUltimoRoll;
        private float segundosDesdeUltimoDisparo;
        private int cantidadVida;

        public Nave(string mediaDir, TGCVector3 posicionInicial, InputDelJugador input)
        {
            this.mediaDir = mediaDir;
            this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicion);
            this.posicion = posicionInicial;
            this.input = input;
            this.velocidadBase = 5f;
            this.velocidadActual = velocidadBase;
            this.aceleracionMovimiento = 0.01f;
            this.rotacionBase = new TGCVector3(0f, Geometry.DegreeToRadian(180f), 0f);
            this.rotacionActual = rotacionBase;
            this.velocidadRotacion = 0.008f;
            this.estaRolleando = false;
            this.estaVivo = true;
            this.segundosDesdeUltimoRoll = 100;
            this.segundosDesdeUltimoDisparo = 100;
            this.cantidadVida = 100;
        }


        public void Init()
        {
            
            TGCVector3 rotacionInicial = new TGCVector3(0f, 1f, 0f) * Geometry.DegreeToRadian(180f);
            modeloNave.CambiarRotacion(rotacionInicial);
            SetearTextoGameOver();
        }

        public void Update(float elapsedTime)
        {
            if (GameManager.Instance.Pause)
                return;
            segundosDesdeUltimoRoll += elapsedTime;
            segundosDesdeUltimoDisparo += elapsedTime;

            if (cantidadVida <= 0)
            {
                Morir();
            }

            if (input.HayInputDeAceleracion())
            {
                float aceleracionDelInput = input.SentidoDeAceleracionDelInput() * aceleracionMovimiento;
                Acelerar(aceleracionDelInput);
            }
            else
            {
                VolverAVelocidadNormal();
            }

            if (input.HayInputDeRoll())
            {
                EmpezarARollear();
            }

            if (estaRolleando)
            {
                Rollear();
            }
            else
            {
                if (input.HayInputDePonerseVertical())
                {
                    RotarPorPonerseVertical(new TGCVector3(0, 0, -1));
                }
                else if (input.HayInputDeRotacion())
                {
                    RotarPorMoverseEnDireccion(input.RotacionDelInput());
                }
                else
                {
                    VolverARotacionNormal();
                }
            }
            if (input.HayInputDeDisparo())
            {
                Disparar();
            }

            CalcularColision();
            MoverseEnDireccion(input.DireccionDelInput(), elapsedTime);

        }

        private void CalcularColision()
        {
            List<Colisionable> listaColisionables = GameManager.Instance.GetColisionables();
            listaColisionables.ForEach(colisionable => {
                if (colisionable.ColisionaConNave()){
                    colisionable.ColisionarConNave();
                } });

        }

        public void Render()
        {
            string textoControles = "Controles:\nQ: Roll\nE: Voltearse\nShift: Acelerar\nCtrl: Desacelerar";
            if (estaVivo)
            {
                modeloNave.AplicarTransformaciones();
                modeloNave.Render();
            }
            else
            {
                textoGameOver.render();
            }
            new TgcText2D().drawText("Pausa activa:\n" +GameManager.Instance.Pause.ToString(), 5, 60, Color.White);

            /*
            new TgcText2D().drawText("Posicion de la nave:\n" + posicion.ToString(), 5, 60, Color.White);
            new TgcText2D().drawText("Velocidad de la nave:\n" + velocidadActual.ToString(), 5, 20, Color.White);
            new TgcText2D().drawText("Rotacion de la nave:\n" + rotacionActual.ToString(), 5, 130, Color.White);
            */
            //new TgcText2D().drawText(textoControles, 5, 10, Color.White);
            new HUD().Render(cantidadVida, CantidadCombustibleParaRollear());

        }

        public void Dispose()
        {
            modeloNave.Dispose();
        }
        public TGCVector3 GetPosicion()
        {
            return posicion;
        }

        private void MoverseEnDireccion(TGCVector3 versorDirector,float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);

            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime * velocidadActual;

            TGCVector3 nuevaPosiblePosicion = posicion + movimientoDelFrame;

            if(nuevaPosiblePosicion.X < 128 && nuevaPosiblePosicion.X > 90)
            {
                posicion.X = nuevaPosiblePosicion.X;
            }
            
            if (nuevaPosiblePosicion.Y > -30 && nuevaPosiblePosicion.Y < 0)
            {
                posicion.Y = nuevaPosiblePosicion.Y;
            }

            posicion.Z = nuevaPosiblePosicion.Z;


            modeloNave.CambiarPosicion(posicion);
        }
        #region Rotacion

        private void Rotar(TGCVector3 nuevaRotacion)
        {
            rotacionActual = nuevaRotacion;
            modeloNave.CambiarRotacion(rotacionActual);
        }
        private void RotarEnDireccionConLimite(TGCVector3 versorDirector, float limiteEnZ, float limiteEnX, float multiplicadorVelocidad)
        {

            TGCVector3 rotacionASumar = versorDirector * velocidadRotacion* multiplicadorVelocidad;

            float nuevaRotacionZ = NuevoValorDentroDeLimites(rotacionActual.Z, rotacionASumar.Z  , limiteEnZ);
            float nuevaRotacionX = NuevoValorDentroDeLimites(rotacionActual.X, rotacionASumar.X  , limiteEnX);

            Rotar(new TGCVector3(nuevaRotacionX, Geometry.DegreeToRadian(180f), nuevaRotacionZ));
        }

        private void RotarPorMoverseEnDireccion(TGCVector3 versorDirector)
        {
            RotarEnDireccionConLimite(versorDirector, Geometry.DegreeToRadian(20f), Geometry.DegreeToRadian(15f), 1);
        }

        private void RotarPorPonerseVertical(TGCVector3 versorDirector)
        {
            RotarEnDireccionConLimite(versorDirector, Geometry.DegreeToRadian(90f), 0, 4);
        }

        private void VolverARotacionNormal()
        {
            TGCVector3 direccionDeRotacionNecesaria = TGCVector3.Normalize(rotacionBase - rotacionActual);
            RotarEnDireccionConLimite(direccionDeRotacionNecesaria, Geometry.DegreeToRadian(360f), Geometry.DegreeToRadian(360f),1);
        }

        private float NuevoValorDentroDeLimites(float valorActual, float valorASumar, float valorLimite) //Mal nombre aaa
        {
            float rotacionMaxima = valorLimite;
            float rotacionMinima = -valorLimite;
            float nuevaPosibleRotacion = valorActual + valorASumar;

            if (nuevaPosibleRotacion >= rotacionMaxima)
            {
                return valorActual;
            }
            else if (nuevaPosibleRotacion <= rotacionMinima)
            {
                return valorActual;
            }
            else
            {
                return nuevaPosibleRotacion;
            }
        }


        #endregion
        private void Acelerar(float aceleracion)
        {
            float velocidadMaxima = velocidadBase * 4;
            float velocidadMinima = velocidadBase / 2;

            float nuevaVelocidad = aceleracion + velocidadActual;

            if (nuevaVelocidad < velocidadMinima)
            {
                velocidadActual -= aceleracion;
            }
            else if(nuevaVelocidad > velocidadMaxima)
            {
                velocidadActual -= aceleracion;
            }
            else
            {
                velocidadActual = nuevaVelocidad;
            }
        }

        private void VolverAVelocidadNormal()
        {
            float aceleracion = Math.Sign(velocidadBase - velocidadActual) * aceleracionMovimiento;
            Acelerar(aceleracion);
        }


        #region Roll
        private void Rollear()
        {
            Rotar(rotacionActual + new TGCVector3(0, 0, aceleracionMovimiento * 4));

            if (TerminoElRoll())
            {
                rotacionActual = new TGCVector3(0, 0, 0);
                estaRolleando = false;
            }
        }

        private void EmpezarARollear()
        {
            if (SePuedeRollear())
            {
                estaRolleando = true;
                segundosDesdeUltimoRoll = 0;
            }
        }

        private int CantidadCombustibleParaRollear() //Ponele
        {
            return Convert.ToInt32(Math.Min(segundosDesdeUltimoRoll * 20, 100));
        }

        private Boolean SePuedeRollear()
        {
            return segundosDesdeUltimoRoll > 5;
        }

        private bool TerminoElRoll()
        {
            return rotacionActual.Z > Math.PI*2;
        }
        #endregion
        private void SetearTextoGameOver()
        {
            textoGameOver = new TgcText2D
            {
                Text = "GAME\nOVER",
                Color = Color.Black,
                Align = TgcText2D.TextAlign.CENTER,
                Position = new Point(500, 300),
                Size = new Size(400, 200)
            };
            textoGameOver.changeFont(new System.Drawing.Font("TimesNewRoman", 100, FontStyle.Bold | FontStyle.Italic));
        }

        private Boolean SePuedeDisparar()
        {
            return segundosDesdeUltimoDisparo > 0.1f;
        }
        private void Disparar()
        {
            if (SePuedeDisparar())
            {
                TGCVector3 posicionLaser = new TGCVector3(GetPosicion());
                posicionLaser.Z += 10f;
                GameManager.Instance.AgregarRenderizable(new LaserDeJugador(mediaDir + "Xwing\\laserBueno-TgcScene.xml", posicionLaser, new TGCVector3(0, 0, 1)));
                segundosDesdeUltimoDisparo = 0;
            }


        }

        public Boolean ColisionaConColisionable(Colisionable unColisionable)
        {
            return modeloNave.ColisionaConColisionable(unColisionable);
        }

        public void Morir()
        {
            estaVivo = false;
        }

        public void Chocar()
        {
            Morir();
        }

        public void ChocarConLaser()
        {
            if (!estaRolleando)
            {
                PerderVida(1);
            }
        }

        private void PerderVida(int vidaAPerder)
        {
            int nuevaPosibleVida = cantidadVida - vidaAPerder;
            cantidadVida = nuevaPosibleVida > 0 ? nuevaPosibleVida : 0;
        }

        public float GetVelocidad()
        {
            return velocidadActual;
        }
    }
}

