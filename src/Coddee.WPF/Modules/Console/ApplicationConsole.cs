﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coddee.Loggers;
using Coddee.WPF.Commands;
using Coddee.WPF.Modules;
using Coddee.WPF.Modules.Console;
using System.Diagnostics;
using System.Reflection;

namespace Coddee.WPF.Console
{
    /// <summary>
    /// The ViewModel of the ApplicationConsole
    /// </summary>
    public class ApplicationConsole : ViewModelBase<ApplicationConsoleView>, IApplicationConsole
    {
        public ApplicationConsole()
        {
            _logger = new StringLogger();
            _logger.AppendString += LoggerAppendString;
            _parser = new ConsoleCommandParser();
            _defaultCommandHandlers = new Dictionary<string, List<EventHandler<ConsoleCommandArgs>>>();
            _commandHandlers = new Dictionary<string, List<EventHandler<ConsoleCommandArgs>>>();
            _commands = new List<ConsoleCommand>();
            _executedCommands = new List<string>();
        }

        /// <summary>
        /// The default commands handlers
        /// </summary>
        private readonly Dictionary<string, List<EventHandler<ConsoleCommandArgs>>> _defaultCommandHandlers;

        /// <summary>
        /// The custom command handlers
        /// </summary>
        private readonly Dictionary<string, List<EventHandler<ConsoleCommandArgs>>> _commandHandlers;

        private readonly List<ConsoleCommand> _commands;
        private readonly List<string> _executedCommands;
        private readonly ConsoleCommandParser _parser;

        private int _executedCommandIndex;
        /// <summary>
        /// The condition to toggle the console window
        /// </summary>
        private Func<KeyEventArgs, bool> _toggleCondition;

        /// <summary>
        /// StringLogger to append the ConsoleContent property
        /// </summary>
        private readonly StringLogger _logger;

        /// <summary>
        /// Called when a new log record added
        /// </summary>
        /// <param name="obj"></param>
        private void LoggerAppendString(string obj)
        {
            WriteToConsole(obj);
        }

        /// <summary>
        /// Specify the console visibility 
        /// </summary>
        private bool _showConsole;
        public bool ShowConsole
        {
            get { return _showConsole; }
            set { SetProperty(ref _showConsole, value); }
        }

        private string _consoleContent;
        public string ConsoleContent
        {
            get { return _consoleContent; }
            set { SetProperty(ref _consoleContent, value); }
        }


        private string _currentCommand;
        public string CurrentCommand
        {
            get { return _currentCommand; }
            set { SetProperty(ref _currentCommand, value); }
        }

        public ICommand ExecuteCommand => new RelayCommand(Execute);

        /// <summary>
        /// Executed when a key is pressed on the command TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void KeyDown(object sender, KeyEventArgs arg)
        {
            if (arg.Key == Key.Tab)
            {
                arg.Handled = true;
                var command = _commands.FirstOrDefault(e => e.Name.StartsWith(CurrentCommand));
                if (command != null)
                {
                    CurrentCommand = command.Name;
                    _view.commandBox.CaretIndex = Int32.MaxValue;
                }
                arg.Handled = true;
            }
            else if (arg.Key == Key.Up)
            {
                _executedCommandIndex--;
                if (_executedCommandIndex < 0)
                    _executedCommandIndex = _executedCommands.Count - 1;

                CurrentCommand = _executedCommands[_executedCommandIndex];
                _view.commandBox.CaretIndex = Int32.MaxValue;
                arg.Handled = true;
            }
            else if (arg.Key == Key.Down)
            {
                _executedCommandIndex++;
                if (_executedCommandIndex > _executedCommands.Count - 1)
                    _executedCommandIndex = 0;

                CurrentCommand = _executedCommands[_executedCommandIndex];
                _view.commandBox.CaretIndex = Int32.MaxValue;
                arg.Handled = true;
            }
        }

        /// <summary>
        /// Returns the string logger responsible for adding the log messages to the console
        /// </summary>
        /// <returns></returns>
        public ILogger GetLogger()
        {
            return _logger;
        }

        /// <summary>
        /// Initialize the console
        /// </summary>
        /// <param name="shell"></param>
        public void Initialize(IShell shell)
        {
            var shellWindow = (Window) shell;

            //Check if the root element of the shell is grid
            //if not a grid will be created and the original content added to it
            var grid = shellWindow.Content as Grid;
            if (grid == null)
            {
                grid = new Grid();
                var oldContent = shellWindow.Content;
                shellWindow.Content = null;
                grid.Children.Add((UIElement) oldContent);
                shellWindow.Content = grid;
            }
            grid.Children.Add(GetView());

            //Sets the Shell KeyDown event handler to toggle the console visibility
            //when Ctrl+F12 are pressed
            shellWindow.KeyDown += (sender, args) =>
            {
                if (_toggleCondition(args))
                    ToggleConsole();
            };

            _view.commandBox.PreviewKeyDown += KeyDown;
            AddDefaultCommands();
            AddDefaultCommandHandlers();
        }

        /// <summary>
        /// Set the condition on which the console visibility will be toggled
        /// </summary>
        /// <param name="toggleCondition"></param>
        public void SetToggleCondition(Func<KeyEventArgs, bool> toggleCondition)
        {
            _toggleCondition = toggleCondition;
        }


        private void AddDefaultCommands()
        {
            AddCommands(DefaultCommands.RestartCommand,
                        DefaultCommands.HelpCommand,
                        DefaultCommands.ShowGlobalsCommand,
                        DefaultCommands.ClearCommand,
                        DefaultCommands.CMDCommand,
                        DefaultCommands.ExitCommand);
        }

        private void AddDefaultCommandHandlers()
        {
            _defaultCommandHandlers[DefaultCommands.RestartCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> {OnRestartCommand};

            _defaultCommandHandlers[DefaultCommands.HelpCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> {OnHelpCommand};

            _defaultCommandHandlers[DefaultCommands.ExitCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> {OnExitCommand};

            _defaultCommandHandlers[DefaultCommands.ShowGlobalsCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> {OnShowGlobalsCommand};

            _defaultCommandHandlers[DefaultCommands.CMDCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> {OnCMDCommand};

            _defaultCommandHandlers[DefaultCommands.ClearCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> {OnClearCommand};
        }


        /// <summary>
        /// Add additional commands to the console.
        /// Use must call the <see cref="AddCommandHandler"/> to provide a handler for the command
        /// </summary>
        /// <param name="commands">The console command object</param>
        public void AddCommands(params ConsoleCommand[] commands)
        {
            _parser.RegisterCommands(commands);
            _commands.AddRange(commands);
        }

        /// <summary>
        /// Provide a handler for a command
        /// </summary>
        /// <param name="command">The console command name</param>
        /// <param name="handler">The command handler</param>
        public void AddCommandHandler(string command, EventHandler<ConsoleCommandArgs> handler)
        {
            command = command.ToLower();
            if (!_commandHandlers.ContainsKey(command) || _commandHandlers[command] == null)
                _commandHandlers[command] = new List<EventHandler<ConsoleCommandArgs>>();
            _commandHandlers[command].Add(handler);
        }

        /// <summary>
        /// Toggle the console visibility
        /// </summary>
        public void ToggleConsole()
        {
            ShowConsole = !ShowConsole;
            _view.commandBox.Focus();
            Keyboard.Focus(_view.commandBox);
        }

        /// <summary>
        /// Execute a command
        /// </summary>
        public void Execute(string commandString)
        {
            if (string.IsNullOrEmpty(commandString))
                return;

            _executedCommands.Add(commandString);
            _executedCommandIndex = _executedCommands.Count;

            WriteToConsole(commandString);
            WriteToConsole(Environment.NewLine);
            CommandParseResult command;
            try
            {
                command = _parser.ParseCommand(commandString);
            }
            catch (CommandParseException e)
            {
                WriteToConsole($"An error occurred while parsing command: {e.Message}\n");
                return;
            }
            if (command == null)
            {
                WriteToConsole($"Invalid command '{commandString}'.\n");
                return;
            }
            var commandName = command.Command.Name.ToLower();
            var args = new ConsoleCommandArgs
            {
                Arguments = command.Arguments,
                Command = command.Command,
                Result = new List<string>(),
            };


            var handlers = _commandHandlers.ContainsKey(commandName) ? _commandHandlers[commandName] : null;
            var defaultHandlers = _defaultCommandHandlers.ContainsKey(commandName)
                ? _defaultCommandHandlers[commandName]
                : null;

            if (handlers == null && defaultHandlers == null)
            {
                WriteToConsole($"Command '{commandString}' is not supported.\n");
                return;
            }

            if (handlers != null)
                foreach (var handler in handlers)
                {
                    if (args.Handled)
                        break;
                    handler?.Invoke(this, args);
                }
            if (!args.Handled)
            {
                if (defaultHandlers != null)
                    foreach (var handler in defaultHandlers)
                    {
                        if (args.Handled)
                            break;
                        handler?.Invoke(this, args);
                    }
            }
            if (args.Result != null)
                foreach (var result in args.Result)
                {
                    WriteToConsole(result);
                    WriteToConsole(Environment.NewLine);
                }
        }

        /// <summary>
        /// Execute the command that is currently in the TextBox
        /// </summary>
        private void Execute()
        {
            Execute(CurrentCommand);
            CurrentCommand = string.Empty;
        }

        private void WriteToConsole(string content)
        {
            ConsoleContent += content;
            ExecuteOnUIContext(_view.consoleTextBox.ScrollToEnd);
        }


        //Handlers
        private void OnRestartCommand(object sender, ConsoleCommandArgs e)
        {
            Process.Start(Assembly.GetEntryAssembly().GetName().CodeBase);
            _app.Shutdown();
        }

        private void OnHelpCommand(object sender, ConsoleCommandArgs e)
        {
            var maxName = _commands.Select(c => c.Name.Length).Max();
            var builder = new StringBuilder();
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            foreach (var command in _commands)
            {
                builder.Append('-', 70);
                builder.Append(Environment.NewLine);
                builder.Append(command.Name);
                for (int i = 0; i < maxName - command.Name.Length; i++)
                {
                    builder.Append(" ");
                }
                builder.Append("\t");
                builder.Append(command.Description);
                builder.Append(Environment.NewLine);
                if (command.SupportedArguments == null || command.SupportedArguments.Count == 0)
                {
                    builder.Append("Doesn't have any arguments.");
                }
                else
                {
                    builder.Append("Arguments: ");
                    builder.Append(Environment.NewLine);
                    var maxArg = command.SupportedArguments.Select(a => a.Key.Length).Max();
                    foreach (var arg in command.SupportedArguments)
                    {
                        builder.Append($"{arg.Key}");
                        for (int i = 0; i < maxArg - command.Name.Length; i++)
                        {
                            builder.Append(" ");
                        }
                        builder.Append("\t");
                        builder.Append(arg.Value);
                    }
                }
                builder.Append(Environment.NewLine);
            }
            builder.Append('-', 70);

            e.Result = new List<string>
            {
                builder.ToString()
            };
        }

        private void OnShowGlobalsCommand(object sender, ConsoleCommandArgs e)
        {
            var variables = _globalVariables.GetAllGlobals();
            var builder = new StringBuilder();
            var maxName = variables.Select(c => c.Key.Length).Max();

            builder.Append(Environment.NewLine);
            foreach (var variable in variables)
            {
                builder.Append(variable.Key);
                for (int i = 0; i < maxName - variable.Key.Length; i++)
                {
                    builder.Append(" ");
                }
                builder.Append(" : ");
                builder.Append(variable.Value);
                builder.Append(Environment.NewLine);
            }
            e.Result.Add(builder.ToString());
            builder.Append(Environment.NewLine);
        }

        private void OnExitCommand(object sender, ConsoleCommandArgs e)
        {
            _app.Shutdown();
        }

        private void OnCMDCommand(object sender, ConsoleCommandArgs e)
        {
            if (e.Arguments == null || !e.Arguments.ContainsKey("/c"))
            {
                e.Result.Add($"The cmd command must have the /c argument, use the help command for more information.");
                return;
            }
            var command = e.Arguments["/c"];
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit();
            e.Result.Add($"CMD output: {process.StandardOutput.ReadToEnd()}");
        }

        private void OnClearCommand(object sender, ConsoleCommandArgs e)
        {
            ConsoleContent = string.Empty;
        }
    }
}