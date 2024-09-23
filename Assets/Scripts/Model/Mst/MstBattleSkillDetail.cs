using MasterMemory;
using MessagePack;

[MemoryTable("battleSkillDetail"), MessagePackObject(true)]
public class MstBattleSkillDetail
{
    public MstBattleSkillDetail() { }
    [PrimaryKey(0)]
    public int BattleSkillId { get; private set; }
    [PrimaryKey(1)]
    public int Idx { get; private set; }
    public int SkillType { get; private set; }
    public int Param1 { get; private set; }
    public int Param2 { get; private set; }
    public int Param3 { get; private set; }

    public MstBattleSkillDetail(int BattleSkillId, int Idx, int SkillType, int Param1, int Param2, int Param3)
    {
        this.BattleSkillId = BattleSkillId;
        this.Idx = Idx;
        this.SkillType = SkillType;
        this.Param1 = Param1;
        this.Param2 = Param2;
        this.Param3 = Param3;
    }
}