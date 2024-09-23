using MasterMemory;
using MessagePack;

[MemoryTable("battleSkill"), MessagePackObject(true)]
public class MstBattleSkill
{
    public MstBattleSkill() { }
    [PrimaryKey(0)]
    public int BattleSkillId { get; private set; }
    public string Description { get; private set; }

    public MstBattleSkill(int BattleSkillId, string Description)
    {
        this.BattleSkillId = BattleSkillId;
        this.Description = Description;
    }
}