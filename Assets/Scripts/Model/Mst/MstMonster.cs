using MasterMemory;
using MessagePack;

[MemoryTable("monster"), MessagePackObject(true)]
public class MstMonster
{
    public MstMonster() { }
    [PrimaryKey(0)]
    public int MonsterId { get; private set; }
    [SecondaryKey(1, 0), NonUnique]
    public string Name { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }
    public int Magic { get; private set; }
    public int Speed { get; private set; }
    public int BattleSkillId { get; private set; }

    public MstMonster(int MonsterId, string Name, int Atk, int Def, int Magic, int Speed, int BattleSkillId)
    {
        this.MonsterId = MonsterId;
        this.Name = Name;
        this.Atk = Atk;
        this.Def = Def;
        this.Magic = Magic;
        this.Speed = Speed;
        this.BattleSkillId = BattleSkillId;
    }
}