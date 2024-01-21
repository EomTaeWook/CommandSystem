﻿using CommandSystem.Interface;
using CommandSystem.Models;
using CommandSystem.Net;
using Dignus.Log;

namespace CommandSystem
{
    public class ClientCmdModule : Module
    {
        private readonly ClientModule _clientModule;
        private readonly string _ip;
        private readonly int _port;

        public bool IsRequested { get; set; }

        private CancellationTokenSource _localCancellationToken;
        public ClientCmdModule(string ip, int port, string moduleName = null) : base(moduleName)
        {
            _clientModule = new ClientModule(this);
            _ip = ip;
            _port = port;
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (IsRequested == true)
            {
                e.Cancel = true;
                _clientModule.CacelCommand();
            }
            else
            {
                //Process.GetCurrentProcess().Kill();
            }
        }
        private async Task<bool> ProcessLocalCommandAsync(string command, string[] options, bool isAlias, CancellationToken cancellationToken)
        {
            if (isAlias == false)
            {
                var table = _builder._commandContainer.Resolve<AliasTable>();
                if (table.Alias.TryGetValue(command, out AliasModel alias) == true)
                {
                    return await ProcessLocalCommandAsync(alias.Cmd, options, true, cancellationToken);
                }
            }

            var commandTable = _builder._commandContainer.Resolve<CommandTable>();

            if (!commandTable.IsContainLocalCommand(command))
            {
                return false;
            }
            try
            {
                var cmdProcessor = _builder._commandContainer.Resolve<ICmdProcessor>(command);
                await cmdProcessor.InvokeAsync(options, cancellationToken);
                return true;
            }
            catch (OperationCanceledException operationCanceledException)
            {
                LogHelper.Error($"failed to command : {operationCanceledException.Message}");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"failed to command : {ex.Message}");
            }
            return false;
        }
        protected override void RunCommand(string line)
        {
            if (_localCancellationToken != null)
            {
                LogHelper.Error("the command is currently in progress");
                return;
            }

            var splits = line.Split(" ");

            _localCancellationToken = new CancellationTokenSource();
            var result = ProcessLocalCommandAsync(splits[0], splits[1..], false, _localCancellationToken.Token).Result;

            _localCancellationToken.Dispose();
            _localCancellationToken = null;

            if (result == true)
            {
                Prompt();
                return;
            }

            _clientModule.SendCommand(line);
            IsRequested = true;
        }
        public override void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        private Task RunAsync()
        {
            return Task.Run(() =>
            {
                _clientModule.Run(_ip, _port);

                LogHelper.Info($"*** command client module start ***");
                Prompt();
            });
        }
    }
}