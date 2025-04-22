using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    public GlobalConfig GlobalConfig;
    [SerializeField] private BoardType currentBoardType = BoardType.Size3x3;

    public void SetBoardType(BoardType boardType)
    {
        currentBoardType = boardType;
    }

    public BoardType GetCurrentBoardType() => currentBoardType;
}
