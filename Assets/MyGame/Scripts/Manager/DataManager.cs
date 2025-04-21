using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    public GlobalConfig GlobalConfig;
    [SerializeField] private BoardType m_CurrentBoardType;

    public void SetBoardType(BoardType boardType)
    {
        m_CurrentBoardType = boardType;
    }

    public BoardType GetCurrentBoardType() => m_CurrentBoardType;
}
