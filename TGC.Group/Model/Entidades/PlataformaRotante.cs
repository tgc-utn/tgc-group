using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model {
    // TODO: podría ser un cilindro en vez de un rectángulo
    public class PlataformaRotante : Plataforma, IUpdateable {
        private float vel;

        public PlataformaRotante(TGCVector3 pos, TGCVector3 size, TgcTexture textura, float velAng) 
            : base(pos, size, textura) {
            vel = velAng / 100;

            box.Move(pos);
            box.Transform = TGCMatrix.Translation(box.Position);
        }

        public void Update(float deltaTime) {
            box.RotateY(vel * deltaTime);
            box.Transform = TGCMatrix.RotationY(box.Rotation.Y) * TGCMatrix.Translation(box.Position);
        }

        public override void Render() {
            base.Render();
            box.BoundingBox.Render();
        }

        public TGCVector3 getVelAsVector(TGCVector3 personajePos) {
            var distanceFromCenter = personajePos - box.Position;
            distanceFromCenter.Y = 0;

            var moduloVel = TGCVector3.Length(distanceFromCenter) * vel;

            var versorDireccion = TGCVector3.Normalize(distanceFromCenter);
            var versorRotado = new TGCVector3(versorDireccion.Z, 0, -versorDireccion.X);

            return versorRotado * moduloVel;
        }

        // no es correcto
        // pero para plataformas chicas anda
        // TODO: calcular el arcoseno de la distancia del personaje y la plataforma
        // TODO: investigar que carajo devuelve Math.Asin()
        // lo tenía hecho pero me devolvía cualquier cosa
        // (seguro devuelve lo correcto y yo lo estoy usando mal)
        public float getAngle() {
            return box.Rotation.Y;
        }

    }
}
