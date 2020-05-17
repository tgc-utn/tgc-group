namespace TGC.Group.Model
{
    interface IObjetoJuego
    {
        void Update(float elapsedTime);
        void Render();
        void Dispose();
    }
}
