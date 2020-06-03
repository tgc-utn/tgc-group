using TGC.Core.Terrain;
using TGC.Core.Mathematica;
using TGC.Examples.Camara;
using TGC.Core.Direct3D;
using System;

namespace TGC.Group.Model
{
	public class Skybox : IRenderizable
	{
		private readonly string mediaDir;
		private TgcSkyBox skyBox;
		private TgcThirdPersonCamera camara;


		public Skybox(string mediaDir, TgcThirdPersonCamera camara)
		{
			this.mediaDir = mediaDir;
			this.camara = camara;
		}

		public void Init()
		{
			skyBox = new TgcSkyBox
			{
				Center = TGCVector3.Empty,
				Size = new TGCVector3(10000, 10000, 10000)
			};
			
			string texturesPath = mediaDir + "\\Skybox\\";

			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "phobos_up.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "phobos_dn.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "phobos_lf.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "phobos_rt.jpg");

			
			//Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "phobos_bk.jpg");
			skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "phobos_ft.jpg");
			

			skyBox.SkyEpsilon = 25f;
			skyBox.Init();

		}

		public void Update(float elapsedTime)
		{

			//Se cambia el valor por defecto del farplane para evitar cliping de farplane. [Copiado del ejemplo]
			D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio,
					D3DDevice.Instance.ZNearPlaneDistance, D3DDevice.Instance.ZFarPlaneDistance * 2f).ToMatrix();

			
			skyBox.Center = camara.Position;
		}

		public void Render()
		{
			skyBox.Render();
		}
		public void Dispose()
		{
			skyBox.Dispose();
		}
	}
}

