﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace View
{
  /// <summary>
  /// Представление кнопки
  /// </summary>
  public class ButtonView : BaseView
  {
    /// <summary>
    /// Экземпляр кнопки
    /// </summary>
    private Button _button;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="parButton">Объект кнопки</param>
    /// <param name="parPlatform">Объект платформы</param>
    public ButtonView(Button parButton, Platform parPlatform) : base(parPlatform)
    {
      _button = parButton;
      _button.PaintEvent += Draw;
    }

    /// <summary>
    /// Отрисовывает кнопку
    /// </summary>
    public override void Draw()
    {
      Platform.PrintTextInRectangle(_button.X1, _button.Y1, _button.X2, _button.Y2, _button.Name, false);
    }
  }
}
