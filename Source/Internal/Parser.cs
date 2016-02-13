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

using System.Collections.Generic;
using System.Globalization;

namespace DataLisp.Internal
{
    class Parser
    {
        Lexer _Lexer;
        Logger _Logger;

        public Parser(string source, Logger logger)
        {
            _Lexer = new Lexer(source, logger);
            _Logger = logger;
        }

        public SyntaxTree Parse()
        {
            return gr_tr_unit();
        }

        // Internal
        SyntaxTree gr_tr_unit()
        {
            SyntaxTree tree = new SyntaxTree();
            while(Lookahead(TokenType.OpenParanthese))
            {
                Match(TokenType.OpenParanthese);
                tree.Nodes.Add(gr_statement());
                Match(TokenType.CloseParanthese);
            }
            return tree;
        }

        StatementNode gr_statement()
        {
            StatementNode node = new StatementNode();
            node.Name = Match(TokenType.Identifier).Value;
            node.Nodes = gr_data_list();
            return node;
        }
        //ExpressionNode gr_expression();

        List<DataNode> gr_data_list()
        {
            List<DataNode> list = new List<DataNode>();
            while (Lookahead(TokenType.Colon) ||
                Lookahead(TokenType.OpenParanthese) ||
                Lookahead(TokenType.OpenSquareBracket) ||
                //Lookahead(TokenType.ExpressionParanthese) ||
                Lookahead(TokenType.Integer) ||
                Lookahead(TokenType.Float) ||
                Lookahead(TokenType.String) ||
                Lookahead(TokenType.True) ||
                Lookahead(TokenType.False))
            {
                list.Add(gr_data());
            }

            return list;
        }

        DataNode gr_data()
        {
            DataNode node = new DataNode();

            if(Lookahead(TokenType.Colon))
            {
                Match(TokenType.Colon);

                Token str;
                if (Lookahead(TokenType.Integer))
                    str = Match(TokenType.Integer);
                else
                    str = Match(TokenType.Identifier);

                node.Key = str.Value;
            }
            else
            {
                node.Key = "";
            }

            node.Value = gr_value();
            return node;
        }

        ArrayNode gr_array()
        {
            ArrayNode node = new ArrayNode();
            node.Nodes = gr_array_list();
            return node;
        }

        List<ValueNode> gr_array_list()
        {
            List<ValueNode> list = new List<ValueNode>();
            while (Lookahead(TokenType.OpenParanthese) ||
                Lookahead(TokenType.OpenSquareBracket) ||
                //Lookahead(TokenType.ExpressionParanthese) ||
                Lookahead(TokenType.Integer) ||
                Lookahead(TokenType.Float) ||
                Lookahead(TokenType.String) ||
                Lookahead(TokenType.True) ||
                Lookahead(TokenType.False))
            {
                list.Add(gr_value());

                if (Lookahead(TokenType.Comma))
                {
                    Match(TokenType.Comma);
                }
                else
                {
                    break;
                }
            }
            return list;
        }

        ValueNode gr_value()
        {
            ValueNode node = new ValueNode();

            if (Lookahead(TokenType.OpenParanthese))
            {
                Match(TokenType.OpenParanthese);

                node.Type = ValueNodeType.Statement;
                node.Statement = gr_statement();

                Match(TokenType.CloseParanthese);
            }
            else if (Lookahead(TokenType.OpenSquareBracket))
            {
                Match(TokenType.OpenSquareBracket);

                node.Type = ValueNodeType.Array;
                node.Array = gr_array();

                Match(TokenType.CloseSquareBracket);
            }
            //else if (Lookahead(TokenType.ExpressionParanthese))
            //{
            //    Match(TokenType.ExpressionParanthese);

            //    node.Type = ValueNodeType.Expression;
            //    node.Expression = gr_expression();

            //    Match(TokenType.CloseParanthese);
            //}
            else if (Lookahead(TokenType.Integer))
            {
                Token str = Match(TokenType.Integer);

                node.Type = ValueNodeType.Integer;
                node.Integer = int.Parse(str.Value);
            }
            else if (Lookahead(TokenType.Float))
            {
                Token str = Match(TokenType.Float);

                node.Type = ValueNodeType.Float;
                node.Float = float.Parse(str.Value, CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (Lookahead(TokenType.String))
            {
                Token str = Match(TokenType.String);

                node.Type = ValueNodeType.String;
                node.String = str.Value;
            }
            else if (Lookahead(TokenType.True))
            {
                Match(TokenType.True);

                node.Type = ValueNodeType.Bool;
                node.Boolean = true;
            }
            else if (Lookahead(TokenType.False))
            {
                Match(TokenType.False);

                node.Type = ValueNodeType.Bool;
                node.Boolean = false;
            }
            else
            {
                node.Type = ValueNodeType.Invalid;
            }
            return node;
        }

        // Utils
        Token Match(TokenType type)
        {
            Token token = _Lexer.Next();
            if (token.Type != type)
            {
                _Logger.Log(_Lexer.CurrentLine, _Lexer.CurrentColumn, LogLevel.Error,
                    "Expected '" + type.ToString() + "' but got '" + token.Type.ToString() + "'");
            }
            return token;
        }

        bool Lookahead(TokenType type)
        {
            Token token = _Lexer.Look();
            return token.Type == type;
        }       
    }
}
