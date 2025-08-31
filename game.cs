using Godot;
using System;

public partial class Game : Node2D
{
   Vector2 screenSize;
   Vector2 padSize;
   Vector2 direction = new Vector2(1, 1);

   const int InitialBallSpeed = 80;
   double ballSpeed = InitialBallSpeed;
   int padSpeed = 150;
   int leftScore = 0;
   int rightScore = 0;
   public override void _Ready()
   {
      screenSize = GetViewportRect().Size;
      padSize = GetNode<Sprite2D>("left").Texture.GetSize();

      SetProcess(true);
   }

   public override void _Process(double delta)

   {
      var hud = GetNode<hud>("HUD");

      var time = 0.0;
      time += delta;

      var ballPos = GetNode<Sprite2D>("ball").Position;
      var leftRect = new Rect2(GetNode<Sprite2D>("left").Position - padSize / 2, padSize);
      var rightRect = new Rect2(GetNode<Sprite2D>("right").Position - padSize / 2, padSize);

      // Integrate new ball position
      ballPos += direction * (float)ballSpeed * (float)delta;

      // Flip when touching roof	or floor
      if ((ballPos.Y < 0 && direction.Y < 0) || (ballPos.Y > screenSize.Y && direction.Y > 0))
      {
         direction.Y = -direction.Y;
      }

      // Flip, change direction and increace speed when touching pads
      if ((leftRect.HasPoint(ballPos) && direction.X < 0) || (rightRect.HasPoint(ballPos) && direction.X > 0))
      {
         direction.X = -direction.X;
         direction.Y = GD.Randf() * 2 - 1;
         direction = direction.Normalized();
         ballSpeed *= 1.1;
      }

      if (ballPos.X > screenSize.X)
      {
         leftScore++;
         hud.UpdateScoreLeft(leftScore);
      }

      if (ballPos.X < 0)
      {
         rightScore++;
         hud.UpdateScoreRight(rightScore);
      }

      // Check gameover
      if (ballPos.X < 0 || ballPos.X > screenSize.X)
      {
         ballPos = screenSize / 2;
         ballSpeed = InitialBallSpeed;
         direction = new Vector2(-1, -1);
      }


      GetNode<Sprite2D>("ball").Position = ballPos;

      // Move left pad
      var leftPos = GetNode<Sprite2D>("left").Position;

      if (leftPos.Y > 0 && Input.IsActionPressed("leftMoveUp"))
      {
         leftPos.Y += -padSpeed * (float)delta;
      }
      if (leftPos.Y < screenSize.Y && Input.IsActionPressed("leftMoveDown"))
      {
         leftPos.Y += padSpeed * (float)delta;
      }

      GetNode<Sprite2D>("left").Position = leftPos;

      // Move right pad
      var rightPos = GetNode<Sprite2D>("right").Position;

      if (rightPos.Y > 0 && Input.IsActionPressed("rightMoveUp"))
      {
         rightPos.Y += -padSpeed * (float)delta;
      }
      if (rightPos.Y < screenSize.Y && Input.IsActionPressed("rightMoveDown"))
      {
         rightPos.Y += padSpeed * (float)delta;
      }

      GetNode<Sprite2D>("right").Position = rightPos;
   }
}
