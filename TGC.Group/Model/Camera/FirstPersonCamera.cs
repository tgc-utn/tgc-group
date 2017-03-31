using System;
using Microsoft.DirectX;
using System.Windows.Forms;

namespace TGC.Group.Model.Camera
{
    public class FirstPersonCamera : TgcCamera
    {
        public void update(EntityPlayer player)
        {
            base.SetCamera(player.HeadPosition, player.LookAt);
        }
    }
}
