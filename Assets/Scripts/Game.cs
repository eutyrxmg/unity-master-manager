using System;
using UnityEngine;

public class Game : MonoBehaviour {

    void Awake() {
        MasterService.Init();
        var monster = MasterService.MemoryDatabase.MstMonsterTable.FindByMonsterId(1);
        Debug.Log(monster.Name);
    }
}
