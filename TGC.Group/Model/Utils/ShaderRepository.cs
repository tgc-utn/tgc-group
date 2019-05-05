using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Shaders;

namespace TGC.Group.Model.Utils
{
    class ShaderRepository
    {
        private static string ShadersDir = "./../Shaders";
        public static Effect Shader
        {
            get { return TGCShaders.Instance.LoadEffect(ShadersDir + "TgcKeyFrameMeshShader.fx"); }
        }
    }
}
