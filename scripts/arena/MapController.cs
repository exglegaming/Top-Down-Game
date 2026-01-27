using System.Collections.Generic;
using Godot;
using TopDownGame.scripts.ui.map_cell;

namespace TopDownGame.scripts.arena;

public partial class MapController : Control
{
    private static readonly PackedScene MapCellScene = GD.Load<PackedScene>("uid://b081fl6ails64");
    
    private readonly Dictionary<Vector2I, MapCell> _minimapCells = new();
    private Vector2I _playerCoord = Vector2I.MaxValue;
    private Vector2 _cellSize;

    public void UpdateOnRoomEnter(Vector2I newRoomCoord)
    {
        if (newRoomCoord == _playerCoord) return;

        if (_minimapCells.TryGetValue(_playerCoord, out var value))
        {
            value.SetPlayerActive(false);
        }
        
        if (_minimapCells.TryGetValue(newRoomCoord, out var newCell))
        {
            newCell = _minimapCells[newRoomCoord];
        }
        else
        {
            newCell = CreateMapCell(newRoomCoord);
        }
        
        _playerCoord = newRoomCoord;
        newCell.SetPlayerActive(true);
    }
    
    public void Reset()
    {
        foreach (var mapCell in _minimapCells.Values)
        {
            mapCell.QueueFree();
        }
        
        _minimapCells.Clear();
        _playerCoord = Vector2I.MaxValue;
        _cellSize = Vector2.Zero;
    }

    private MapCell CreateMapCell(Vector2I coord)
    {
        var newCell = (MapCell)MapCellScene.Instantiate();
        
        _minimapCells[coord] = newCell;
        AddChild(newCell);

        if (_cellSize == Vector2.Zero)
        {
            _cellSize = newCell.Size;
        }

        var relativePosition = new Vector2(coord.X * _cellSize.X, coord.Y * _cellSize.Y);
        newCell.Position = (Size / 2.0f) + relativePosition - (_cellSize / 2.0f);
        return newCell;
    }
}