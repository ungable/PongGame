using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Game : Node2D
{
   private Menu menu;
   Vector2 screenSize;
   Vector2 padSize;
   Vector2 direction = new Vector2(1, 1);


   const int InitialBallSpeed = 2200;
   double ballSpeed = InitialBallSpeed;
   int padSpeed = 150;
   int leftScore = 0;
   int rightScore = 0;

   Vector2 leftStartPos;
   Vector2 rightStartPos;

   private bool firstBounce = true;
   int serveDirX = 0;
   private bool isPaused = false;
   public override void _Ready()
   {
      screenSize = GetViewportRect().Size;
      padSize = GetNode<Sprite2D>("left").Texture.GetSize();
      menu = GetNode<Menu>("Menu");
      menu.ShowMenu(true);
      menu.SetPanelColor(new Color(0.2f, 0.4f, 0.8f));
      menu.SetWinner("");

      leftStartPos = GetNode<Sprite2D>("left").Position;
      rightStartPos = GetNode<Sprite2D>("right").Position;

      SetProcess(false);

      menu.Connect("StartGame", new Callable(this, nameof(OnStartGame)));
      menu.Connect("RestartGame", new Callable(this, nameof(OnRestartGame)));
      menu.Connect("ContinueGame", new Callable(this, nameof(OnContinueGame)));

   }

   private void OnStartGame()
   {
      ResetGame();
      menu.ShowStartMenu();
      menu.SetPanelColor(new Color(0.2f, 0.4f, 0.8f));
      menu.ShowMenu(false);
      SetProcess(true);
   }

   private void OnRestartGame()
   {
      ResetGame();
      menu.ShowPauseMenu();
      menu.ShowMenu(false);
      SetProcess(true);
   }

   private void OnContinueGame()
   {
      menu.ShowMenu(false);
      SetProcess(true);

   }

   private void ResetGame()
   {
      leftScore = 0;
      rightScore = 0;
      ballSpeed = InitialBallSpeed;
      firstBounce = true;
      GetNode<Sprite2D>("ball").Position = screenSize / 2;
      direction = new Vector2(1, 0);
      menu.SetWinner("");

      //Сброс позиций ракеток
      GetNode<Sprite2D>("left").Position = leftStartPos;
      GetNode<Sprite2D>("right").Position = rightStartPos;

      // Сброс очков
      var hud = GetNode<hud>("HUD");
      hud.UpdateScoreLeft(leftScore);
      hud.UpdateScoreRight(rightScore);

   }

   public override void _Process(double delta)

   {
      var hud = GetNode<hud>("HUD");

      var ballPos = GetNode<Sprite2D>("ball").Position;
      var leftRect = new Rect2(GetNode<Sprite2D>("left").Position - padSize / 2, padSize);
      var rightRect = new Rect2(GetNode<Sprite2D>("right").Position - padSize / 2, padSize);

      if (Input.IsActionJustPressed("ui_cancel"))
      {
         if (menu.Visible && menu.IsPauseMenuActive)
         {
            menu.ShowMenu(false);
            isPaused = false;
         }
         else
         {
            menu.ShowPauseMenu();
            menu.SetPanelColor(new Color(1.0f, 0.6f, 0.2f));
            menu.ShowMenu(true);
            isPaused = true;
         }
      }

      if (isPaused)
         return;

      // Integrate new ball position
      ballPos += direction * (float)ballSpeed * (float)delta;

      // Flip when touching roof	or floor
      if (ballPos.Y < 0 || ballPos.Y > screenSize.Y)
      {
         direction.Y = -direction.Y;
      }

      // Flip, change direction and increace speed when touching pads
      if ((leftRect.HasPoint(ballPos) && direction.X < 0) || (rightRect.HasPoint(ballPos) && direction.X > 0))
      {
         bool hitLeft = leftRect.HasPoint(ballPos) && direction.X < 0;
         direction.X = -direction.X;

         Sprite2D pad = hitLeft ? GetNode<Sprite2D>("left") : GetNode<Sprite2D>("right");
         float padCenterY = pad.Position.Y;
         float padHeight = padSize.Y;

         float relativeY = (ballPos.Y - padCenterY) / (padHeight / 2); // -1 (верх), 0 (центр), 1 (низ)
         relativeY = Mathf.Clamp(relativeY, -1, 1);

         if (Mathf.Abs(relativeY) < 0.2)
         {
            direction.Y = 0;
         }
         else if (relativeY < 0)
         {
            direction.Y = -1;
         }
         else
         {
            direction.Y = 1;
         }

         // direction.Y = relativeY;
         direction = direction.Normalized();
         ballSpeed *= 1.1;

         if (firstBounce)
         {
            ballSpeed *= 1.2;
            firstBounce = false;
         }


      }

      if (ballPos.X > screenSize.X)
      {
         leftScore++;
         hud.UpdateScoreLeft(leftScore);
         serveDirX = -1;
      }

      else if (ballPos.X < 0)
      {
         rightScore++;
         hud.UpdateScoreRight(rightScore);
         serveDirX = 1;
      }

      // Check gameover
      if (ballPos.X < 0 || ballPos.X > screenSize.X)
      {
         ballPos = screenSize / 2;
         ballSpeed = InitialBallSpeed;
         float randY = (float)GD.RandRange(-1.0, 1.0);
         direction = new Vector2(serveDirX, randY).Normalized();
         firstBounce = true;
         serveDirX = 0;
      }


      GetNode<Sprite2D>("ball").Position = ballPos;

      // Move left pad
      var leftPos = GetNode<Sprite2D>("left").Position;

      if (leftPos.Y > padSize.Y / 2 && Input.IsActionPressed("leftMoveUp"))
      {
         leftPos.Y += -padSpeed * (float)delta;
      }
      if (leftPos.Y < screenSize.Y - padSize.Y / 2 && Input.IsActionPressed("leftMoveDown"))
      {
         leftPos.Y += padSpeed * (float)delta;
      }

      GetNode<Sprite2D>("left").Position = leftPos;

      // Move right pad
      var rightPos = GetNode<Sprite2D>("right").Position;

      if (rightPos.Y > padSize.Y / 2 && Input.IsActionPressed("rightMoveUp"))
      {
         rightPos.Y += -padSpeed * (float)delta;
      }
      if (rightPos.Y < screenSize.Y - padSize.Y / 2 && Input.IsActionPressed("rightMoveDown"))
      {
         rightPos.Y += padSpeed * (float)delta;
      }

      GetNode<Sprite2D>("right").Position = rightPos;

      if (leftScore >= 10 || rightScore >= 10)
      {
         string winner = leftScore >= 10 ? "Left Player Wins!" : "Right Player Wins!";
         menu.SetWinner(winner);
         menu.ShowStartMenu();
         menu.SetPanelColor(new Color(0.2f, 0.4f, 0.8f));
         menu.ShowMenu(true);
         SetProcess(false);
      }
   }
}
