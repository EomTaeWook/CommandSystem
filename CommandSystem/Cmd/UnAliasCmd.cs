﻿using CommandSystem.Attribude;
using CommandSystem.Interface;
using CommandSystem.Models;
using System.Text.Json;

namespace CommandSystem.Cmd
{
    [Cmd("unalias")]
    internal class UnAliasCmd : ICmdProcessor
    {
        private readonly AliasTable _aliasTable;
        public UnAliasCmd(AliasTable aliasTable)
        {
            _aliasTable = aliasTable;
        }

        public async Task InvokeAsync(string[] args)
        {
            foreach (var item in args)
            {
                _aliasTable.RemoveAlias(item);
            }
            var datas = _aliasTable.GetDatas();
            var json = JsonSerializer.Serialize(datas);
            await File.WriteAllTextAsync(AliasTable.Path, json);
        }

        public string Print()
        {
            return $"명령어의 별칭을 삭제합니다.";
        }
    }
}