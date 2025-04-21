using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    public GlobalConfig GlobalConfig;
    [SerializeField] private BoardType currentBoardType;

    public void SetBoardType(BoardType boardType)
    {
        currentBoardType = boardType;
    }

    public BoardType GetCurrentBoardType() => currentBoardType;
}
