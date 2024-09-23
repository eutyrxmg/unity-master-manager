using System;
using System.Collections.Generic;
using System.Reflection;

public static class MasterHelper {
    public delegate object Constructor();
    public static Constructor GetConstructor(string tableName) {
        return tableName switch {
        
            "monster" => () => new MstMonster(),
        
            "battleSkill" => () => new MstBattleSkill(),
        
            "battleSkillDetail" => () => new MstBattleSkillDetail(),
            
            _ => throw new ArgumentOutOfRangeException(nameof(tableName), tableName, null)
        };
    }   
}