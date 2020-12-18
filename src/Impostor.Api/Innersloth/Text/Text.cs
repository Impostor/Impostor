using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Impostor.Api.Innersloth.Text
{
    public class Text
    {
        private readonly List<Text> _children = new List<Text>();

        public Text(string content, Color? color = null, string? link = null)
        {
            Content = content;
            Color = color;
            Link = link;
        }

        public string Content { get; private set; }

        public Color? Color { get; private set; }

        public string? Link { get; private set; }

        public IReadOnlyList<Text> Children => _children.AsReadOnly();

        public static implicit operator Text(string raw) => Parse(raw);

        public static implicit operator string(Text text) => text.ToString();

        public static Text Parse(string raw)
        {
            var root = new Text(string.Empty);
            var current = root;

            var inBracket = false;

            string? color = null;
            string? link = null;

            foreach (var c in raw)
            {
                if (inBracket)
                {
                    if (c == ']')
                    {
                        inBracket = false;

                        if (color != null)
                        {
                            current.Color = System.Drawing.Color.FromArgb(
                                int.Parse(color.Substring(6, 2), NumberStyles.HexNumber),
                                int.Parse(color.Substring(0, 2), NumberStyles.HexNumber),
                                int.Parse(color.Substring(2, 2), NumberStyles.HexNumber),
                                int.Parse(color.Substring(4, 2), NumberStyles.HexNumber));
                            color = null;
                        }
                        else if (link != null)
                        {
                            current.Link = link;
                            link = null;
                        }
                        else
                        {
                            if (!ReferenceEquals(root, current) && current.Content != string.Empty)
                            {
                                root.Append(current);
                            }

                            current = new Text(string.Empty);
                        }
                    }
                    else if (color != null)
                    {
                        color += c.ToString();
                    }
                    else if (link != null)
                    {
                        link += c.ToString();
                    }
                    else if (int.TryParse(c.ToString(), NumberStyles.HexNumber, null, out var number))
                    {
                        color = c.ToString();
                    }
                    else
                    {
                        link = c.ToString();
                    }
                }
                else if (c == '[')
                {
                    if (current.Content != string.Empty)
                    {
                        if (!ReferenceEquals(root, current) && current.Content != string.Empty)
                        {
                            root.Append(current);
                        }

                        current = new Text(string.Empty);
                    }

                    inBracket = true;
                }
                else
                {
                    current.Content += c;
                }
            }

            return root;
        }

        public string ToRawString()
        {
            var builder = new StringBuilder();

            builder.Append(Content);

            foreach (var child in _children)
            {
                builder.Append(child.ToRawString());
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Color != null)
            {
                builder.Append("[");
                builder.Append($"{Color.Value.R:X2}{Color.Value.G:X2}{Color.Value.B:X2}{Color.Value.A:X2}");
                builder.Append("]");
            }

            if (Link != null)
            {
                builder.Append("[");
                builder.Append(Link);
                builder.Append("]");
            }

            builder.Append(Content);

            if (Color != null || Link != null)
            {
                builder.Append("[]");
            }

            foreach (var child in _children)
            {
                builder.Append(child);
            }

            return builder.ToString();
        }

        public Text Append(Text text)
        {
            _children.Add(text);

            return this;
        }

        public Text Append(string content, Color? color = null, string? link = null)
        {
            return Append(new Text(content, color, link));
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Text)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_children, Content, Color, Link);
        }

        protected bool Equals(Text other)
        {
            return Content == other.Content && Color?.ToArgb() == other.Color?.ToArgb() && Link == other.Link && _children.SequenceEqual(other._children);
        }
    }
}
