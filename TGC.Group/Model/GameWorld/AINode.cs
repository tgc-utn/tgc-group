using Microsoft.DirectX;
using System;
using System.Collections.Generic;



namespace TGC.Group.Model.GameWorld
{
    public class AINode
    {
        Vector3 position;
        List<Vector3> directions;

        public AINode(Vector3 position, List<Vector3> directions)
        {
            this.position = position;
            this.directions = directions;
        }

        public Vector3 Position
        {
            get { return this.position; }
        }

        public List<Vector3> Directions
        {
            get { return this.directions; }
        }

        public Vector3 Direction
        {
            get
            {
                Random rnd = new Random();
                int position = rnd.Next(0, this.directions.Count - 1);
                return this.directions[position];
            }
        }

        public Boolean isInThePoint(Vector3 position)
        {
            return this.position.Equals(position);
        }


    }
}