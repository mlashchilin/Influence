﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model
{
  /// <summary>
  /// Класс игрового поля
  /// </summary>
  public class GameField
  {
    /// <summary>
    /// Событие перерисовки игрового поля
    /// </summary>
    public event dPaintHandler PaintEvent;

    /// <summary>
    /// Объект класса, который отвечает за логику
    /// выполнения ходов
    /// </summary>
    private MoveRunner _moveRunner;

    /// <summary>
    /// Текущие игроки
    /// </summary>
    private List<Player> _players;

    /// <summary>
    /// Номер текущего игрока
    /// </summary>
    private int _currentPlayer;

    /// <summary>
    /// Текущее состояние игры
    /// </summary>
    private GameState _currentGameState;

    /// <summary>
    /// Кнопка переключения состояний игры
    /// </summary>
    private Button _button;

    /// <summary>
    /// Ячейки игрового поля
    /// </summary>
    public Cell[,] Cells
    {
      get
      {
        return _moveRunner.Cells;
      }
    }

    /// <summary>
    /// Кнопка переключения состояний игры
    /// </summary>
    public Button Button
    {
      get
      {
        return _button;
      }
      set
      {
        _button = value;
      }
    }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="parVerticalSize">Вертикальный размер поля</param>
    /// <param name="parHorizontalSize">Горизонтальный размер поля</param>
    /// <param name="parPlayers">Список игроков</param>
    public GameField(int parVerticalSize, int parHorizontalSize, List<Player> parPlayers)
    {
      _moveRunner = new MoveRunner(new MapBuilder().BuildMap(parVerticalSize, parHorizontalSize, parPlayers));
      _currentGameState = GameState.Select;
      _players = parPlayers;
      _currentPlayer = 0;
      _button = new Button(35.0f, 5.0f, 60.0f, 15.0f, "Complete atack");
    }

    /// <summary>
    /// Инициализирует игровое поле
    /// </summary>
    public void Initialize()
    {
      PaintEvent?.Invoke();
    }

    /// <summary>
    /// Выбирает ячейку
    /// </summary>
    private void SelectCell()
    {
      Cell clickedCell = GetFocusedCell();

      if (clickedCell != null)
      {
        UnselectAllCells();
        if (clickedCell?.Owner == GetActivePlayer())
        {
          clickedCell.ActiveCell();
          _currentGameState = GameState.Atack;
        }
      }

      _button.CallPaintEvent();
    }

    /// <summary>
    /// Производит атаку на ячейку
    /// </summary>
    private void AtackCell()
    {
      Cell selectedCell = GetSelectedCell();
      Cell clickedCell = GetFocusedCell();

      if (selectedCell != null && clickedCell != null)
      {
        _moveRunner.Move(selectedCell, clickedCell, GetActivePlayer());
      }
      else
      {
        _currentGameState = GameState.Select;
      }

      PaintEvent?.Invoke();
      _button.CallPaintEvent();
    }

    /// <summary>
    /// Раздает очки
    /// </summary>
    private void DistributeScore()
    {
      Cell clickedCellCoords = GetFocusedCell();

      if (null != clickedCellCoords)
      {
        if (clickedCellCoords.Owner == GetActivePlayer())
        {
          if (GetActivePlayer().Score > 0)
          {
            GetActivePlayer().Score -= 1;
            clickedCellCoords.Score += 1;
          }
        }
        PaintEvent?.Invoke();
        _button.CallPaintEvent();
      }
    }

    /// <summary>
    /// Снимает выделение со всех ячеек
    /// </summary>
    private void UnselectAllCells()
    {
      int rows = Cells.GetLength(0);
      int colomns = Cells.GetLength(1);

      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < colomns; j++)
        {
          if (Cells[i, j] != null)
          {
            Cells[i, j].DisactiveCell();
          }
        }
      }
    }

    /// <summary>
    /// Передает ход следующему игроку
    /// </summary>
    private void PassMove()
    {
      if (_currentPlayer >= _players.Count - 1)
      {
        _currentPlayer = 0;
      }
      else
      {
        _currentPlayer++;
      }

      _currentGameState = GameState.Select;
      _button.CallPaintEvent();
    }

    /// <summary>
    /// Получает ячейку, на которую
    /// наведен курсор
    /// </summary>
    /// <returns>Объект ячейки</returns>
    private Cell GetFocusedCell()
    {
      Cursor cursor = Cursor.GetInstance();
      int rows = Cells.GetUpperBound(0) + 1;
      int colomns = Cells.GetUpperBound(1) + 1;
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < colomns; j++)
        {
          if (null != Cells[i, j])
          {
            if ((cursor.X >= Cells[i, j].X - 1.5f) && (cursor.Y >= Cells[i, j].Y - 1.5f)
                && (cursor.X <= Cells[i, j].X + 1.5f) && (cursor.Y <= Cells[i, j].Y + 1.5f))
            {
              return Cells[i, j];
            }
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Получает выбранную ячейку
    /// </summary>
    /// <returns>Объект ячейки</returns>
    private Cell GetSelectedCell()
    {
      int rows = Cells.GetLength(0);
      int colomns = Cells.GetLength(1);
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < colomns; j++)
        {
          if (null != Cells[i, j])
          {
            if (Cells[i, j].CellStatus == CellStatuses.Active)
            {
              return Cells[i, j];
            }
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Получает число ячеек игрока
    /// </summary>
    /// <param name="parPlayer">Объект игрока</param>
    /// <returns>Число ячеек</returns>
    private int GetPlayerNumOfCells(Player parPlayer)
    {
      int result = 0;
      int rows = Cells.GetLength(0);
      int colomns = Cells.GetLength(1);
      for (int i = 0; i < rows; i++)
      {
        for (int j = 0; j < colomns; j++)
        {
          if (null != Cells[i, j])
          {
            if (Cells[i, j].Owner == parPlayer)
            {
              result++;
            }
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Завершает атаку или передает ход
    /// </summary>
    public void CompleteAtackOrPassMove()
    {
      switch (_currentGameState)
      {
        case GameState.Select:
        case GameState.Atack:
          {
            GetActivePlayer().Score = CalculateScorePlayer();
            _currentGameState = GameState.ScoreDistributing;
            _button.Name = "Pass move";
            break;
          }
        case GameState.ScoreDistributing:
          {
            PassMove();
            _button.Name = "Complete atack";
            break;
          }
      }
    }

    /// <summary>
    /// Выполняет одно из игровых действий
    /// </summary>
    public void PerformGameAction()
    {
      switch (_currentGameState)
      {
        case GameState.Select:
          SelectCell();
          break;
        case GameState.Atack:
          AtackCell();
          break;
        case GameState.ScoreDistributing:
          DistributeScore();
          break;
      }
    }

    /// <summary>
    /// Вычисляет счет текущего игрока
    /// </summary>
    /// <returns>Счет текущего игрока</returns>
    private int CalculateScorePlayer()
    {
      int scorePlayer = GetPlayerNumOfCells(GetActivePlayer());
      return scorePlayer;
    }

    /// <summary>
    /// Возвращает текущего игрока
    /// </summary>
    /// <returns>Текущий игрок</returns>
    public Player GetActivePlayer()
    {
      return _players[_currentPlayer];
    }
  }
}
