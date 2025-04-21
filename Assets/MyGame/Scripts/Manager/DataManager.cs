using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    public GlobalConfig GlobalConfig;
    [SerializeField] private BoardType _CurrentBoardType;

    public void SetBoardType(BoardType boardType)
    {
        _CurrentBoardType = boardType;
    }

    public BoardType GetCurrentBoardType() => _CurrentBoardType;
}
