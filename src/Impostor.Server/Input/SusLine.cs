using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Impostor.Server.Input
{
    /// <summary>
    /// GNU Readline-like utility for collecting user input.
    /// </summary>
    internal class SusLine : IDisposable
    {
        private readonly StringBuilder _input = new StringBuilder();
        private readonly SusLineWriter _writer;

        private readonly List<string> _history = new List<string>();
        private int _historyPosition;

        public SusLine()
        {
            _writer = new SusLineWriter(Console.Out, _input);
            Console.SetOut(_writer);
        }

        /// <summary>
        /// Custom <see cref="Console.ReadLine"/> with support for being used alongside <see cref="Console.SetCursorPosition"/> calls and history support.
        /// </summary>
        /// <param name="stoppingToken"><see cref="CancellationToken"/> for a while loop.</param>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public string? ReadLine(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Console.KeyAvailable)
                {
                    continue;
                }

                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                    {
                        var line = _input.ToString();
                        Clear();

                        AppendToHistory(line);
                        return line;
                    }

                    case ConsoleKey.LeftArrow:
                        MoveCursorLeft();
                        break;

                    case ConsoleKey.RightArrow:
                        MoveCursorRight();
                        break;

                    case ConsoleKey.Home:
                        MoveCursorToBeginning();
                        break;

                    case ConsoleKey.End:
                        MoveCursorToEnd();
                        break;

                    case ConsoleKey.Backspace:
                        Backspace();
                        break;

                    case ConsoleKey.Delete:
                        Delete();
                        break;

                    case ConsoleKey.UpArrow:
                    {
                        if (_historyPosition > 0)
                        {
                            _historyPosition--;
                            SetInput(GetHistoryElement());
                        }

                        break;
                    }

                    case ConsoleKey.DownArrow:
                    {
                        if (_historyPosition < _history.Count)
                        {
                            _historyPosition++;
                            SetInput(GetHistoryElement());
                        }

                        break;
                    }

                    default:
                    {
                        if (keyInfo.Key == ConsoleKey.Clear || keyInfo.KeyChar == (char)ConsoleKey.Clear)
                        {
                            Console.Clear();
                            break;
                        }

                        if (keyInfo.KeyChar == default)
                        {
                            continue;
                        }

                        _input.Insert(_writer.Position, keyInfo.KeyChar);
                        _writer.Position++;
                        break;
                    }
                }

                _writer.WritePrompt();
            }

            Clear();
            return null;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Update()
        {
            _writer.WritePrompt();
        }

        public void MoveCursorLeft()
        {
            if (_writer.Position > 0)
            {
                _writer.Position -= 1;
            }
        }

        public void MoveCursorRight()
        {
            if (_writer.Position < _input.Length)
            {
                _writer.Position += 1;
            }
        }

        public void MoveCursorToBeginning()
        {
            _writer.Position = 0;
        }

        public void MoveCursorToEnd()
        {
            _writer.Position = _input.Length;
        }

        public void Backspace()
        {
            if (_writer.Position > 0 && _writer.Position <= _input.Length)
            {
                _input.Remove(_writer.Position - 1, 1);
                MoveCursorLeft();
            }
        }

        public void Delete()
        {
            if (_input.Length > _writer.Position)
            {
                _input.Remove(_writer.Position, 1);
            }
        }

        public void AppendToHistory(string line)
        {
            _history.Add(line);
            _historyPosition++;
        }

        public void SetInput(string? line)
        {
            _input.Clear();

            if (line != null)
            {
                _input.Append(line);
                MoveCursorToEnd();
            }
            else
            {
                MoveCursorToBeginning();
            }
        }

        private string? GetHistoryElement()
        {
            return _historyPosition != _history.Count ? _history[_historyPosition] : null;
        }

        private void Clear()
        {
            _writer.Position = 0;
            _input.Clear();
        }

        /// <summary>
        /// Console.Out wrapper used for displaying prompts.
        /// </summary>
        private class SusLineWriter : TextWriter
        {
            private const string Prefix = "> ";

            private readonly TextWriter _out;
            private readonly StringBuilder _input;

            private int? _lastLength;
            private int? _lastPosition;

            public SusLineWriter(TextWriter @out, StringBuilder input)
            {
                _out = @out;
                _input = input;

                Encoding = _out.Encoding;
            }

            public override Encoding Encoding { get; }

            public int Position { get; set; }

            public override void Write(char value)
            {
                lock (this)
                {
                    ClearPrompt();
                    _out.Write(value);
                }
            }

            public override void WriteLine()
            {
                base.WriteLine();
                WritePrompt();
            }

            public void WritePrompt()
            {
                lock (this)
                {
                    ClearPrompt();

                    var input = Prefix + _input;

                    Console.SetCursorPosition(0, Console.CursorTop);

                    _out.Write(input);

                    var position = Prefix.Length + Position;
                    var (lines, line) = CalculateLines(input.Length, position);

                    Console.SetCursorPosition(position % Console.WindowWidth, Console.CursorTop + (lines - line));

                    if (Console.CursorLeft == 0 && lines > 3)
                    {
                        Console.CursorTop++;
                    }

                    _lastPosition = position;
                    _lastLength = input.Length;
                }
            }

            protected override void Dispose(bool disposing)
            {
                lock (this)
                {
                    ClearPrompt();
                    Console.SetOut(_out);
                }

                base.Dispose(disposing);
            }

            private void ClearPrompt()
            {
                if (_lastLength == null || _lastPosition == null)
                {
                    return;
                }

                if (Console.CursorLeft != 0 || Console.CursorTop != 0)
                {
                    Console.CursorVisible = false;

                    var (lines, line) = CalculateLines(_lastLength.Value, _lastPosition.Value);
                    var start = Console.CursorTop - lines;

                    for (var i = 0; i <= lines; i++)
                    {
                        ClearLine(start + i);
                    }

                    Console.SetCursorPosition(0, start);

                    Console.CursorVisible = true;
                }

                _lastLength = null;
                _lastPosition = null;
            }

            private (int Lines, int Line) CalculateLines(int length, int position)
            {
                return (length / Console.WindowWidth, position / Console.WindowWidth);
            }

            private void ClearLine(int line)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.SetCursorPosition(Console.WindowWidth - 1, line);

                    for (var i = 0; i < Console.WindowWidth; i++)
                    {
                        WriteBackspace();
                    }
                }
                else
                {
                    Console.SetCursorPosition(0, line);
                    _out.Write("\u001b[2K");
                }
            }

            private void WriteBackspace()
            {
                _out.Write("\b \b");
            }
        }
    }
}
