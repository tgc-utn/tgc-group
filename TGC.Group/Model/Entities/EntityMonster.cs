using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Utils;
using TGC.Group.Model.GameWorld;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;


namespace TGC.Group.Model.Entities
{
	public class EntityMonster : EntityUpdatable
	{
		protected TgcSkeletalMesh enemy;
		private const float MOVEMENT_SPEED = 200f;
		private Vector3 move = new Vector3(0, 0, -1);
		private Boolean rotating = false;
		private float rotationAngle = 0;
		private List<AINode> walkingNodes;
		private Vector3[] intersectionPoints;
		private TgcBoundingCylinderFixedY playerColliderCylinder;
		private Boolean chaseMode = false;
		private Vector3 playerPosition;
		private TgcBoundingAxisAlignBox enemySight;

		public EntityMonster(TgcSkeletalLoader loader, String mediaPath)
		{
			this.walkingNodes = new List<AINode>();
			enemy = loader.loadMeshAndAnimationsFromFile(mediaPath + "/Monster-TgcSkeletalMesh.xml", new string[]
				{
					mediaPath + "/Run-TgcSkeletalAnim.xml",
					mediaPath + "/Walk-TgcSkeletalAnim.xml",
				});
			enemy.playAnimation("Run");
			enemy.Position = new Vector3(330f, 0f, 330f);
			enemy.Scale = new Vector3(2.5f, 2.5f, 2.5f);
		}

		public List<AINode> WalkingNodes
		{
			get { return this.walkingNodes; }
			set { this.walkingNodes = value; }
		}

		public Vector3[] IntersectionPoints
		{
			get { return this.intersectionPoints; }
			set { this.intersectionPoints = value; }
		}

		public void playerColider(TgcBoundingCylinderFixedY colliderCylinder)
		{
			this.playerColliderCylinder = colliderCylinder;
		}


		public override void update(float elapsedTime)
		{
			if (!chaseMode)
				patrol(elapsedTime);
			else
				chase(elapsedTime);
		}

		private void chase(float elapsedTime)
		{
			if (!this.rotating)
			{
				var originalMove = new Vector3(this.move.X, this.move.Y, this.move.Z);
				if (this.playerPosition.Equals(this.enemy.Position))
				{
					if (isInSight(new Vector3(1, 0, 0)))
					{
						this.move = new Vector3(1, 0, 0);
						setRotationAngle(originalMove);
					}
					else if (isInSight(new Vector3(-1, 0, 0)))
					{
						this.move = new Vector3(-1, 0, 0);
						setRotationAngle(originalMove);
					}
					else if (isInSight(new Vector3(0, 0, 1)))
					{
						this.move = new Vector3(0, 0, 1);
						setRotationAngle(originalMove);
					}
					else if (isInSight(new Vector3(0, 0, -1)))
					{
						this.move = new Vector3(0, 0, -1);
						setRotationAngle(originalMove);
					}
					else
						chaseMode = false;

				}
			}


			if (this.rotating)
			{
				enemy.playAnimation("Walk");
				var rotAngle = 1f / 180f;
				this.enemy.rotateY(rotAngle);
				this.rotationAngle = this.rotationAngle - rotAngle;

				if (this.rotationAngle < 0)
				{ rotating = false; enemy.playAnimation("Run"); }
			}

			this.enemy.UpdateMeshTransform();
			this.enemy.updateAnimation(elapsedTime);
			if (!rotating && chaseMode)
			{
				enemy.move(this.move * 1f);
				this.rotationAngle = 0;
			}

			if (TgcCollisionUtils.testAABBCylinder(enemy.BoundingBox, playerColliderCylinder))
			{
				int i = 0;
				i++;
			}
		}
		private void patrol(float elapsedTime)
		{
			if (!this.rotating)
			{
				var originalMove = new Vector3(this.move.X, this.move.Y, this.move.Z);

				for (int i = 0; i < this.walkingNodes.Count; i++)
				{
					if (this.walkingNodes[i].isInThePoint(this.enemy.Position))
					{
						this.move = this.walkingNodes[i].Direction;

						setRotationAngle(originalMove);
						break;
					}
				}
			}

			if (rotating)
			{
				enemy.playAnimation("Walk");
				var rotAngle = 1f / 180f;
				this.enemy.rotateY(rotAngle);
				this.rotationAngle = this.rotationAngle - rotAngle;

				if (this.rotationAngle < 0)
				{ rotating = false; enemy.playAnimation("Run"); }
			}

			this.enemy.UpdateMeshTransform();
			this.enemy.updateAnimation(elapsedTime);
			if (!rotating)
			{
				chaseMode = isInSight(this.move);
				enemy.move(this.move * 1f);

				this.rotationAngle = 0;
			}

			if (TgcCollisionUtils.testAABBCylinder(enemy.BoundingBox, playerColliderCylinder))
			{
				int i = 0;
				i++;
			}
		}

		private void setRotationAngle(Vector3 originalMove)
		{
			var newMove = this.move;

			if ((originalMove.Z == -1 && newMove.X == 1) ||
				(originalMove.X == -1 && newMove.Z == -1) ||
				(originalMove.X == 1 && newMove.Z == 1) ||
				(originalMove.Z == 1 && newMove.X == -1))
				this.rotationAngle = FastMath.Acos(Vector3.Dot(Vector3.Normalize(newMove), Vector3.Normalize(originalMove))) + FastMath.PI;
			else
				this.rotationAngle = FastMath.Acos(Vector3.Dot(Vector3.Normalize(newMove), Vector3.Normalize(originalMove)));

			this.rotating = true;
		}

		private Boolean isInSight(Vector3 sight)
		{
			this.enemySight =
				TgcBoundingAxisAlignBox.computeFromPoints(new[]
				{this.enemy.Position +( sight * 10000000f) +new Vector3 (120,20,120)
				, this.enemy.Position + (sight * 10000000f) +new Vector3 (-120,20,-120)
				, this.enemy.Position + new Vector3 (120,20,120)
				,this.enemy.Position  + new Vector3(-120, 20, -120)});

			if (TgcCollisionUtils.testAABBCylinder(this.enemySight, this.playerColliderCylinder))
			{
				this.playerPosition = getClosestPoint(this.playerColliderCylinder.Center);
				return true;
			}
			return false;
		}

		protected Vector3 getClosestPoint(Vector3 point)
		{
			Vector3 returnPoint = new Vector3();

			float distance = 0f;
			returnPoint = TgcCollisionUtils.closestPoint(point, this.intersectionPoints, out distance);
			return returnPoint;
		}


		public override void render()
		{
			this.enemy.render();
		}

		public override void dispose()
		{
			this.enemy.dispose();
		}


	}
}
