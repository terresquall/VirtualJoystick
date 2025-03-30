using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCombo", menuName = "FightingGame/Combo", order = 0)]
public class InputData : ScriptableObject
{
    public string comboName;

    [Header("Combo input")]
    public List<Direction> comboSequence;
}

//stores the avaliable directions
public enum Direction
{
    Neutral,
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}





