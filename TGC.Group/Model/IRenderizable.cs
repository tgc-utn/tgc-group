using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    public interface IRenderizable
    {
        void Init();

        void Update(float elapsedTime);

        void Render();

        void Dispose();
    }
}
