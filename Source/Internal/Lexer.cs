/*
 * Copyright (c) 2016, Ömercan Yazici <omercan AT pearcoding.eu>
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 *    1. Redistributions of source code must retain the above copyright notice,
 *       this list of conditions and the following disclaimer.
 *
 *    2. Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *
 *    3. Neither the name of the copyright owner may be used
 *       to endorse or promote products derived from this software without
 *       specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE
 */

using System.Text;
namespace DataLisp.Internal
{
    class Lexer
    {
        string _Source;
        Logger _Logger;
        int _Position = 0;

        int _Line = 1;
        public int CurrentLine
        {
            get { return _Line; }
        }

        int _Column = 1;
        public int CurrentColumn
        {
            get { return _Column; }
        }

        public Lexer(string source, Logger logger)
        {
            _Source = source;
            _Logger = logger;
        }

        public Token Next()
        {
            if (_Position >= _Source.Length)
            {
                return new Token(TokenType.EOF);
            }
            //else if (CurrentChar() == '$')
            //{
            //    ++_Position;
            //    ++_Column;

            //    if (_Position < _Source.Length && CurrentChar() == '(')
            //    {
            //        ++_Position;
            //        ++_Column;

            //        return new Token(TokenType.ExpressionParanthese);
            //    }
            //    else if (_Position >= _Source.Length)
            //    {
            //        _Logger.Log(_Line, _Column, LogLevel.Error,
            //             "No '(' after '$'");
            //        return new Token(TokenType.EOF);
            //    }
            //    else
            //    {
            //        _Logger.Log(_Line, _Column, LogLevel.Error,
            //             "Invalid character '" + CurrentChar() + "' after '$'");
            //        ++_Position;
            //        ++_Column;
            //        return Next();
            //    }
            //}
            else if (CurrentChar() == '(')
            {
                ++_Position;
                ++_Column;

                return new Token(TokenType.OpenParanthese);
            }
            else if (CurrentChar() == ')')
            {
                ++_Position;
                ++_Column;

                return new Token(TokenType.CloseParanthese);
            }
            else if (CurrentChar() == '[')
            {
                ++_Position;
                ++_Column;

                return new Token(TokenType.OpenSquareBracket);
            }
            else if (CurrentChar() == ']')
            {
                ++_Position;
                ++_Column;

                return new Token(TokenType.CloseSquareBracket);
            }
            else if (CurrentChar() == ',')
            {
                ++_Position;
                ++_Column;

                return new Token(TokenType.Comma);
            }
            else if (CurrentChar() == ':')
            {
                ++_Position;
                ++_Column;

                return new Token(TokenType.Colon);
            }
            else if (CurrentChar() == ';')//Comment
            {
                while (_Position < _Source.Length && CurrentChar() != '\n')
                {
                    ++_Position;
                    ++_Column;
                }

                return Next();
            }
            else if (CurrentChar() == '"' || CurrentChar() == '\'')//String
            {
                char start = CurrentChar();

                string str = "";
                while (true)
                {
                    ++_Position;
                    ++_Column;

                    if (_Position >= _Source.Length ||
                        CurrentChar() == '\n')
                    {
                        _Logger.Log(_Line, _Column, LogLevel.Error,
                             "The string \"" + str + "\" is not closed");
                        break;
                    }
                    else if (CurrentChar() == '\\')
                    {
                        ++_Position;
                        ++_Column;

                        if (_Position >= _Source.Length)
                        {
                            _Logger.Log(_Line, _Column, LogLevel.Error,
                                "Invalid use of the '\\' operator");
                        }
                        else if (CurrentChar() == '\n')
                        {
                            ++_Line;
                            ++_Position;
                            _Column = 1; 
                        }
                        else
                        {
                            switch (CurrentChar())
                            {
                                case 'n':// New line
                                    str += '\n';
                                    break;
                                case 't':// Tab
                                    str += '\t';
                                    break;
                                case 'r':// Carriage return 
                                    str += '\r';
                                    break;
                                case 'a':// Audible bell 
                                    str += '\a';
                                    break;
                                case 'b':// Backspace
                                    str += '\b';
                                    break;
                                case 'f':// Form feed
                                    str += '\f';
                                    break;
                                case 'v':// Vertical tab
                                    str += '\v';
                                    break;
                                case 'x'://Binary [2]
                                case 'u'://Unicode [4]
                                case 'U'://Unicode [8]
                                    {
                                        int length = CurrentChar() == 'x' ? 2 : (CurrentChar() == 'u' ? 4 : 8);
                                        string uni_val = "";
                                        for (int i = 0; i < length; ++i)
                                        {
                                            ++_Position;
                                            ++_Column;
                                            if (_Position >= _Source.Length || CurrentChar() == '\n')
                                            {
                                                _Logger.Log(_Line, _Column, LogLevel.Error, "Invalid use of Unicode escape sequence.");
                                                break;
                                            }

                                            uni_val += CurrentChar();
                                        }

                                        if (uni_val.Length == length)
                                        {
                                            try
                                            {
                                                uint uni = System.Convert.ToUInt32(uni_val, 16);

                                                if (length != 2)
                                                {
                                                    byte[] tmp = new byte[4];
                                                    int size = 0;
                                                    if (uni <= 0x7F)
                                                    {
                                                        tmp[0] = (byte)uni;
                                                        size = 1;
                                                    }
                                                    else if (uni <= 0x7FF)
                                                    {
                                                        uint d = uni & 0x7FF;
                                                        tmp[0] = (byte)(0xC0 | ((d & 0x7C0) >> 6));
                                                        tmp[1] = (byte)(0x80 | (d & 0x3F));
                                                        size = 2;
                                                    }
                                                    else if (uni <= 0xFFFF)
                                                    {
                                                        uint d = uni & 0xFFFF;
                                                        tmp[0] = (byte)(0xE0 | ((d & 0xF000) >> 12));
                                                        tmp[1] = (byte)(0x80 | ((d & 0xFC0) >> 6));
                                                        tmp[2] = (byte)(0x80 | (d & 0x3F));
                                                        size = 3;
                                                    }
                                                    else if (uni <= 0x10FFFF)
                                                    {
                                                        uint d = uni & 0x10FFFF;
                                                        tmp[0] = (byte)(0xF0 | ((d & 0x1C0000) >> 18));
                                                        tmp[1] = (byte)(0x80 | ((d & 0x3F000) >> 12));
                                                        tmp[2] = (byte)(0x80 | ((d & 0xFC0) >> 6));
                                                        tmp[3] = (byte)(0x80 | (d & 0x3F));
                                                        size = 4;
                                                    }
                                                    else
                                                    {
                                                        _Logger.Log(_Line, _Column, LogLevel.Error, "Invalid Unicode range.");
                                                    }

                                                    if(size != 0)
                                                        str += Encoding.UTF8.GetString(tmp, 0, size);
                                                }
                                                else//Binary
                                                {
                                                    str += (char)uni;
                                                }
                                            }
                                            catch(System.Exception)
                                            {
                                                _Logger.Log(_Line, _Column, LogLevel.Error, "Given Unicode sequence is invalid.");
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    str += CurrentChar();
                                    break;
                            }
                        }
                    }
                    else if (CurrentChar() == start)
                    {
                        ++_Position;
                        ++_Column;
                        break;
                    }
                    else
                    {
                        str += CurrentChar();
                    }
                }

                return new Token(TokenType.String, str);
            }
            else if (IsDigit(CurrentChar()) || CurrentChar() == '-')
            {
                string identifier = "";
                identifier += CurrentChar();

                ++_Position;
                ++_Column;
                while (_Position < _Source.Length)
                {
                    if (IsDigit(CurrentChar()) || CurrentChar() == '.')
                    {
                        identifier += CurrentChar();
                        ++_Position;
                        ++_Column;
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (IsInteger(identifier))
                {
                    return new Token(TokenType.Integer, identifier);
                }
                else if (IsFloat(identifier))
                {
                    return new Token(TokenType.Float, identifier);
                }
                else
                {
                    _Logger.Log(_Line, _Column, LogLevel.Error,
                        "Unknown identifier '" + identifier + "'");
                    return new Token(TokenType.EOF);
                }
            }
            else if (IsAlpha(CurrentChar()))//Identifier
            {
                string identifier = "";
                identifier += CurrentChar();

                ++_Position;
                ++_Column;
                while (_Position < _Source.Length)
                {
                    if (IsAscii(CurrentChar()))
                    {
                        identifier += CurrentChar();
                        ++_Position;
                        ++_Column;
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (identifier == "true")
                {
                    return new Token(TokenType.True);
                }
                else if (identifier == "false")
                {
                    return new Token(TokenType.False);
                }
                else
                {
                    return new Token(TokenType.Identifier, identifier);
                }
            }
            else if (IsWhitespace(CurrentChar()))
            {
                if (CurrentChar() == '\n')
                {
                    ++_Line;
                    _Column = 0;
                }

                ++_Position;
                ++_Column;
                return Next();
            }
            else
            {
                _Logger.Log(_Line, _Column, LogLevel.Error,
                    "Invalid character '" + CurrentChar() + "'");
                ++_Position;
                ++_Column;
                return Next();
            }
        }

        public Token Look()
        {
            int pos = _Position;
            int line = _Line;
            int col = _Column;

            Token t = Next();

            _Position = pos;
            _Line = line;
            _Column = col;

            return t;
        }

        static private bool IsWhitespace(char c)
        {
            if(c == ' ' || c == '\t' || c == '\r' ||
                c == '\n' || c == '\v' || c == '\f')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool IsAscii(char c)
        {
            if (IsDigit(c) || IsAlpha(c))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static private bool IsDigit(char c)
        {
            if (c == '1' || c == '2' || c == '3' ||
            c == '4' || c == '5' || c == '6' ||
            c == '7' || c == '8' || c == '9' ||
            c == '0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool IsAlpha(char c)
        {
            if (c == 'a' || c == 'b' ||
            c == 'c' || c == 'd' || c == 'e' ||
            c == 'f' || c == 'g' || c == 'h' ||
            c == 'i' || c == 'j' || c == 'k' ||
            c == 'l' || c == 'm' || c == 'n' ||
            c == 'o' || c == 'p' || c == 'q' ||
            c == 'r' || c == 's' || c == 't' ||
            c == 'u' || c == 'v' || c == 'w' ||
            c == 'x' || c == 'y' || c == 'z' ||
            c == 'A' || c == 'B' || c == 'C' ||
            c == 'D' || c == 'E' || c == 'F' ||
            c == 'G' || c == 'H' || c == 'I' ||
            c == 'J' || c == 'K' || c == 'L' ||
            c == 'M' || c == 'N' || c == 'O' ||
            c == 'P' || c == 'Q' || c == 'R' ||
            c == 'S' || c == 'T' || c == 'U' ||
            c == 'V' || c == 'W' || c == 'X' ||
            c == 'Y' || c == 'Z' || c == '_')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static private bool IsInteger(string str)
        {
            for(int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if (!IsDigit(c) &&
                c != '-')
                {
                    return false;
                }
                else if (c == '-' && i != 0)
                {
                    return false;
                }
            }
            return true;
        }

        static private bool IsFloat(string str)
        {
            bool point = false;
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];

                if (c != '.' &&
                !IsDigit(c) &&
                c != '-')
                {
                    return false;
                }
                else if (c == '-' && i != 0)
                {
                    return false;
                }
                else if (c == '.')
                {
                    if (point)
                        return false;

                    point = true;
                }
            }

            return true;
        }

        private char CurrentChar()
        {
            return _Source[_Position];
        }
    }
}
