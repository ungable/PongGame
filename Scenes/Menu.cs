using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Menu : CanvasLayer
{
   [Signal]
   public delegate void StartGameEventHandler();
   [Signal]
   public delegate void RestartGameEventHandler();
   [Signal]
   public delegate void ContinueGameEventHandler();

   public bool IsPauseMenuActive { get; private set; } = false;

   public override void _Ready()
   {
      GetNode<Button>("Panel/VBoxContainer/StartGame").Pressed += () => EmitSignal("StartGame");
      GetNode<Button>("Panel/VBoxContainer/RestartGame").Pressed += () => EmitSignal("RestartGame");
      GetNode<Button>("Panel/VBoxContainer/ContinueGame").Pressed += () => EmitSignal("ContinueGame");

   }

   public void ShowMenu(bool show)
   {
      Visible = show;
   }

   public void SetWinner(string winner)
   {
      var label = GetNode<Label>("Panel/VBoxContainer/WinnerLabel");
      label.Text = winner;
      label.Visible = !string.IsNullOrEmpty(winner);
   }

   public void ShowPauseMenu()
   {
      GetNode<Button>("Panel/VBoxContainer/StartGame").Visible = false;
      GetNode<Button>("Panel/VBoxContainer/ContinueGame").Visible = true;
      GetNode<Button>("Panel/VBoxContainer/RestartGame").Visible = true;
      IsPauseMenuActive = true;
   }

   public void ShowStartMenu()
   {
      GetNode<Button>("Panel/VBoxContainer/StartGame").Visible = true;
      GetNode<Button>("Panel/VBoxContainer/ContinueGame").Visible = false;
      GetNode<Button>("Panel/VBoxContainer/RestartGame").Visible = false;
      IsPauseMenuActive = false;
   }

   public void SetPanelColor(Color color)
   {
      var panel = GetNode<Panel>("Panel");
      var style = panel.GetThemeStylebox("panel") as StyleBoxFlat;
      if (style != null)
         style.BgColor = color;
   }

}
