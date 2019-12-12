using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHandlePlayerSought
{
    void OnPlayerSought(Player player);
    void OnPlayerNotSeeing(Player player);
}
