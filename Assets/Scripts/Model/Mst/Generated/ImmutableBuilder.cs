// <auto-generated />
#pragma warning disable CS0105
using MasterMemory.Validation;
using MasterMemory;
using MessagePack;
using System.Collections.Generic;
using System;
using Model.Tables;

namespace Model
{
   public sealed class ImmutableBuilder : ImmutableBuilderBase
   {
        MemoryDatabase memory;

        public ImmutableBuilder(MemoryDatabase memory)
        {
            this.memory = memory;
        }

        public MemoryDatabase Build()
        {
            return memory;
        }

        public void ReplaceAll(System.Collections.Generic.IList<MstBattleSkill> data)
        {
            var newData = CloneAndSortBy(data, x => x.BattleSkillId, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstBattleSkillTable(newData);
            memory = new MemoryDatabase(
                table,
                memory.MstBattleSkillDetailTable,
                memory.MstMonsterTable
            
            );
        }

        public void RemoveMstBattleSkill(int[] keys)
        {
            var data = RemoveCore(memory.MstBattleSkillTable.GetRawDataUnsafe(), keys, x => x.BattleSkillId, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.BattleSkillId, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstBattleSkillTable(newData);
            memory = new MemoryDatabase(
                table,
                memory.MstBattleSkillDetailTable,
                memory.MstMonsterTable
            
            );
        }

        public void Diff(MstBattleSkill[] addOrReplaceData)
        {
            var data = DiffCore(memory.MstBattleSkillTable.GetRawDataUnsafe(), addOrReplaceData, x => x.BattleSkillId, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.BattleSkillId, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstBattleSkillTable(newData);
            memory = new MemoryDatabase(
                table,
                memory.MstBattleSkillDetailTable,
                memory.MstMonsterTable
            
            );
        }

        public void ReplaceAll(System.Collections.Generic.IList<MstBattleSkillDetail> data)
        {
            var newData = CloneAndSortBy(data, x => (x.BattleSkillId, x.Idx), System.Collections.Generic.Comparer<(int BattleSkillId, int Idx)>.Default);
            var table = new MstBattleSkillDetailTable(newData);
            memory = new MemoryDatabase(
                memory.MstBattleSkillTable,
                table,
                memory.MstMonsterTable
            
            );
        }

        public void RemoveMstBattleSkillDetail((int BattleSkillId, int Idx)[] keys)
        {
            var data = RemoveCore(memory.MstBattleSkillDetailTable.GetRawDataUnsafe(), keys, x => (x.BattleSkillId, x.Idx), System.Collections.Generic.Comparer<(int BattleSkillId, int Idx)>.Default);
            var newData = CloneAndSortBy(data, x => (x.BattleSkillId, x.Idx), System.Collections.Generic.Comparer<(int BattleSkillId, int Idx)>.Default);
            var table = new MstBattleSkillDetailTable(newData);
            memory = new MemoryDatabase(
                memory.MstBattleSkillTable,
                table,
                memory.MstMonsterTable
            
            );
        }

        public void Diff(MstBattleSkillDetail[] addOrReplaceData)
        {
            var data = DiffCore(memory.MstBattleSkillDetailTable.GetRawDataUnsafe(), addOrReplaceData, x => (x.BattleSkillId, x.Idx), System.Collections.Generic.Comparer<(int BattleSkillId, int Idx)>.Default);
            var newData = CloneAndSortBy(data, x => (x.BattleSkillId, x.Idx), System.Collections.Generic.Comparer<(int BattleSkillId, int Idx)>.Default);
            var table = new MstBattleSkillDetailTable(newData);
            memory = new MemoryDatabase(
                memory.MstBattleSkillTable,
                table,
                memory.MstMonsterTable
            
            );
        }

        public void ReplaceAll(System.Collections.Generic.IList<MstMonster> data)
        {
            var newData = CloneAndSortBy(data, x => x.MonsterId, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstMonsterTable(newData);
            memory = new MemoryDatabase(
                memory.MstBattleSkillTable,
                memory.MstBattleSkillDetailTable,
                table
            
            );
        }

        public void RemoveMstMonster(int[] keys)
        {
            var data = RemoveCore(memory.MstMonsterTable.GetRawDataUnsafe(), keys, x => x.MonsterId, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.MonsterId, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstMonsterTable(newData);
            memory = new MemoryDatabase(
                memory.MstBattleSkillTable,
                memory.MstBattleSkillDetailTable,
                table
            
            );
        }

        public void Diff(MstMonster[] addOrReplaceData)
        {
            var data = DiffCore(memory.MstMonsterTable.GetRawDataUnsafe(), addOrReplaceData, x => x.MonsterId, System.Collections.Generic.Comparer<int>.Default);
            var newData = CloneAndSortBy(data, x => x.MonsterId, System.Collections.Generic.Comparer<int>.Default);
            var table = new MstMonsterTable(newData);
            memory = new MemoryDatabase(
                memory.MstBattleSkillTable,
                memory.MstBattleSkillDetailTable,
                table
            
            );
        }

    }
}