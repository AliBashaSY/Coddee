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
using Coddee.WPF;
using Coddee.WPF.Commands;
using System.Diagnostics;
using System.Reflection;

namespace Coddee.Services.ApplicationConsole
{
    /// <summary>
    /// The ViewModel of the ApplicationConsole
    /// </summary>
    public class ApplicationConsoleService : ViewModelBase<ApplicationConsoleView>, IApplicationConsole
    {
        /// <param name="parser"><see cref="IConsoleCommandParser"/> service</param>
        public ApplicationConsoleService(IConsoleCommandParser parser)
        {
            _logger = new StringLogger();
            _logger.AppendString += LoggerAppendString;
            _parser = parser;
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
        private readonly IConsoleCommandParser _parser;

        private int _executedCommandIndex;
        /// <summary>
        /// The condition to toggle the console window
        /// </summary>
        private Func<KeyEventArgs, bool> _toggleCondition;

        /// <summary>
        /// StringLogger to append the ConsoleContent property
        /// </summary>
        private new readonly StringLogger _logger;

        /// <summary>
        /// Called when a new log record added
        /// </summary>
        /// <param name="obj"></param>
        private void LoggerAppendString(string obj)
        {
            WriteToConsole(obj);
        }

        
        private bool _showConsole;
        /// <summary>
        /// Specify the console visibility 
        /// </summary>
        public bool ShowConsole
        {
            get { return _showConsole; }
            set { SetProperty(ref _showConsole, value); }
        }

        private string _consoleContent;
        /// <summary>
        /// The content displayed in the console.
        /// </summary>
        public string ConsoleContent
        {
            get { return _consoleContent; }
            set { SetProperty(ref _consoleContent, value); }
        }


        private string _currentCommand;
        /// <summary>
        /// The current command that the user is writing.
        /// </summary>
        public string CurrentCommand
        {
            get { return _currentCommand; }
            set { SetProperty(ref _currentCommand, value); }
        }

        /// <summary>
        /// Execute the current command
        /// </summary>
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
                    View.commandBox.CaretIndex = Int32.MaxValue;
                }
                arg.Handled = true;
            }
            else if (arg.Key == Key.Up)
            {
                _executedCommandIndex--;
                if (_executedCommandIndex < 0)
                    _executedCommandIndex = _executedCommands.Count - 1;

                CurrentCommand = _executedCommands[_executedCommandIndex];
                View.commandBox.CaretIndex = Int32.MaxValue;
                arg.Handled = true;
            }
            else if (arg.Key == Key.Down)
            {
                _executedCommandIndex++;
                if (_executedCommandIndex > _executedCommands.Count - 1)
                    _executedCommandIndex = 0;

                CurrentCommand = _executedCommands[_executedCommandIndex];
                View.commandBox.CaretIndex = Int32.MaxValue;
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
        public void Initialize(ContentControl shell, LogRecordTypes logLevel)
        {
            _logger.Initialize(logLevel);

            
            //Check if the root element of the shell is grid
            //if not a grid will be created and the original content added to it
            var grid = shell.Content as Grid;
            if (grid == null)
            {
                grid = new Grid();
                var oldContent = shell.Content;
                shell.Content = null;
                grid.Children.Add((UIElement)oldContent);
                shell.Content = grid;
            }
            grid.Children.Add((UIElement)GetView());

            //Sets the Shell KeyDown event handler to toggle the console visibility
            shell.KeyDown += (sender, args) =>
            {
                if (_toggleCondition(args))
                    ToggleConsole();
            };

            View.commandBox.PreviewKeyDown += KeyDown;
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
            AddCommands(DefaultCommands.AllCommands);
        }

        private void AddDefaultCommandHandlers()
        {
            _defaultCommandHandlers[DefaultCommands.RestartCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnRestartCommand };

            _defaultCommandHandlers[DefaultCommands.HelpCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnHelpCommand };

            _defaultCommandHandlers[DefaultCommands.ExitCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnExitCommand };

       
            _defaultCommandHandlers[DefaultCommands.CMDCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnCMDCommand };

            _defaultCommandHandlers[DefaultCommands.ClearCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnClearCommand };

            _defaultCommandHandlers[DefaultCommands.SetScreenCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnSetScreenCommand };

            _defaultCommandHandlers[DefaultCommands.SetLanguageCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnSetLanguageCommand };

            _defaultCommandHandlers[DefaultCommands.SetResolutionCommand.Name] =
                new List<EventHandler<ConsoleCommandArgs>> { OnSetResolutionCommand };


        }

        private void OnSetResolutionCommand(object sender, ConsoleCommandArgs e)
        {
            UISynchronizationContext.ExecuteOnUIContext(() =>
            {
                var window = Resolve<IShell>() as Window;
                if (!e.Arguments.ContainsKey("/r"))
                {
                    e.Result.Add("You must specify the /r argument.");
                    e.Handled = false;
                    return;
                }
                var res = e.Arguments["/r"];
                if (res.ToLower() == "fullscreen")
                {
                    window.WindowState = WindowState.Maximized;
                }
                else
                {
                    var resWH = res.Split('x');
                    window.WindowState = WindowState.Normal;
                    window.Height = double.Parse(resWH[0]);
                    window.Width = double.Parse(resWH[1]);
                }
            });
            e.Handled = true;
        }

        private void OnSetLanguageCommand(object sender, ConsoleCommandArgs e)
        {
            if (!e.Arguments.ContainsKey("/l"))
            {
                e.Result.Add("You must specify the /l argument.");
                e.Handled = false;
                return;
            }

            LocalizationManager.DefaultLocalizationManager.SetCulture(e.Arguments["/l"]);
        }

        private void OnSetScreenCommand(object sender, ConsoleCommandArgs e)
        {
            if (!e.Arguments.ContainsKey("/i"))
            {
                e.Result.Add("You must specify the /i argument.");
                e.Handled = false;
                return;
            }
            var indexStr = e.Arguments["/i"];
            if (!int.TryParse(indexStr, out var index))
            {
                e.Result
                    .Add($"The value '{indexStr}' is invalid for the /i argument, you need to specify an integer value.");
                e.Handled = false;
                return;
            }
            var temp = System.Windows.Forms.Screen.AllScreens.ElementAtOrDefault(index);
            if (temp == null)
            {
                e.Result.Add($"The value '{indexStr}' is invalid for the /i argument.");
                e.Handled = false;
                return;
            }
            var shell = Resolve<IShell>() as Window;
            if (shell == null)
            {
                e.Result.Add("Shell not found.");
                e.Handled = false;
                return;
            }

            var oldState = shell.WindowState;
            shell.WindowState = WindowState.Normal;
            shell.Left = temp.WorkingArea.Left;
            shell.WindowState = oldState;

            e.Handled = true;
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
            View.commandBox.Focus();
            Keyboard.Focus(View.commandBox);
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

        /// <inheritdoc/>
        public void Execute(ConsoleCommand command)
        {
            Execute(command.Name);
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
            ExecuteOnUIContext(View.consoleTextBox.ScrollToEnd);
        }


        //Handlers
        private void OnRestartCommand(object sender, ConsoleCommandArgs e)
        {
            Process.Start(Assembly.GetEntryAssembly().GetName().CodeBase);
            ((WPFApplication)_app).GetSystemApplication().Shutdown();
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


        private void OnExitCommand(object sender, ConsoleCommandArgs e)
        {
            ((WPFApplication)_app).GetSystemApplication().Shutdown();
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