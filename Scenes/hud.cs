using Godot;
using System;

public partial class hud : CanvasLayer
{
   public void UpdateScoreLeft(int score)
   {
      GetNode<Label>("Left").Text = score.ToString();
   }
   public void UpdateScoreRight(int score)
   {
      GetNode<Label>("Right").Text = score.ToString();
   }
}
