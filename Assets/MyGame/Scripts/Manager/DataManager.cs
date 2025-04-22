using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    public GlobalConfig GlobalConfig;
    [SerializeField] private BoardType currentBoardType;
    private void Start()
    {
        if (GlobalConfig != null)
        {
            currentBoardType = GlobalConfig.defaultBoardType;
        }
        else
        {
            Debug.LogWarning("GlobalConfig is not assigned in DataManager.");
        }
    }

    public void SetBoardType(BoardType boardType)
    {
        currentBoardType = boardType;
    }

    public BoardType GetCurrentBoardType() => currentBoardType;
}
