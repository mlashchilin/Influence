﻿namespace ConsoleView
{
  /// <summary>
  /// Делегат события нажатия клавиши в консоле
  /// </summary>
  /// <param name="parSender">Источник события</param>
  /// <param name="parE">Параметры события</param>
  public delegate void dConsoleKeyboardEventHandler(object parSender, ConsoleKeyboardEventArgs parE);
}