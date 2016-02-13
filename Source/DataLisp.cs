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

namespace DataLisp
{
    class DataLisp
    {
        Logger _Logger;
        Internal.SyntaxTree _Tree;

        public DataLisp(Logger logger)
        {
            _Logger = logger;
        }

        public DataLisp()
        {
            _Logger = new SourceLogger();
        }

        public void Parse(string source)
        {
            Internal.Parser parser = new Internal.Parser(source, _Logger);
            _Tree = parser.Parse();
        }

        public void Build(DataContainer container)
        {
            container.Build(_Tree);
        }

        public static string Generate(DataContainer container)
        {
            string output = "";

            foreach(DataGroup group in container.TopGroups)
            {
                output += GenerateDataGroup(group, 0) + "\n";
            }

            return output;
        }

        public string Dump()
        {
            if (_Tree == null)
                return "";
            else
            {
                string output = "";

                foreach (Internal.StatementNode n in _Tree.Nodes)
                {
                    output += DumpNode(n, 1);
                }

                return output;
            }
        }

        static string DumpNode(Internal.StatementNode node, int depth)
        {
            string white = "";
            for (int i = 0; i < depth; ++i)
                white += " ";

            string str = white + "Statement {" + node.Name + "\n";
            foreach(Internal.DataNode n in node.Nodes)
            {
                str += DumpNode(n, depth + 1);
            }
            str += white + ")\n";
            return str;
        }

        static string DumpNode(Internal.DataNode node, int depth)
        {
            string white = "";
            for (int i = 0; i < depth; ++i)
                white += " ";

            string str = white + "Data {" + node.Key + ":\n";
            str += DumpNode(node.Value, depth + 1);
            str += white + ")\n";
            return str;
        }

        static string DumpNode(Internal.ValueNode node, int depth)
        {
            string white = "";
            for (int i = 0; i < depth; ++i)
                white += " ";

            string str = "";
            switch(node.Type)
            {
                case Internal.ValueNodeType.Statement:
                    str += DumpNode(node.Statement, depth + 1);
                    break;
                case Internal.ValueNodeType.Integer:
                    str += node.Integer;
                    break;
                case Internal.ValueNodeType.Float:
                    str += node.Float;
                    break;
                case Internal.ValueNodeType.String:
                    str += "\"" + node.String + "\"";
                    break;
                case Internal.ValueNodeType.Bool:
                    if (node.Boolean)
                        str += "true";
                    else
                        str += "false";
                    break;
                case Internal.ValueNodeType.Array:
                    foreach(Internal.ValueNode n in node.Array.Nodes)
                    {
                        str += DumpNode(n, depth + 1);
                    }
                    break;
                case Internal.ValueNodeType.Invalid:
                    str += "INVALID";
                    break;
            }
            return str;
        }
        //static string DumpNode(Internal.ExpressionNode node, int depth);

        static string GenerateDataGroup(DataGroup d, int depth)
        {
            string white = "";
            for (int i = 0; i < depth; ++i)
                white += " ";

            string str = white + "(" + d.ID + "\n";
            foreach(Data data in d.IndexedData)
            {
                str += GenerateData(data, depth + 1) + "\n";
            }
            foreach(Data data in d.NamedData)
            {
                str += GenerateData(data, depth + 1) + "\n";
            }
            return str + white + ")\n";
        }

        static string GenerateData(Data d, int depth)
        {
            string white = "";
            for (int i = 0; i < depth; ++i)
                white += " ";

            string str;
            if (d.Key == "")
                str = GenerateValue(d, depth + 1);
            else
                str = ":" + d.Key + " " + GenerateValue(d, depth + 1);

            return white + str;
        }

        static string GenerateArray(DataArray d, int depth)
        {
            string white = "";
            for (int i = 0; i < depth; ++i)
                white += " ";

            if(d.Size <= 0)
            {
                return "[]";
            }
            else
            {
                string str = "[" + GenerateValue(d.At(0), depth + 1) + ",\n";
                for(int i = 1; i < d.Size; ++i)
                {
                    str += white + GenerateValue(d.At(i), depth + 1) + ",\n";
                }
                return str + white + "]";
            }
        }

        static string GenerateValue(Data d, int depth)
        {
            switch(d.Type)
            {
                case DataType.Group:
                    return GenerateDataGroup(d.Group, depth);
                case DataType.Array:
                    return GenerateArray(d.Array, depth);
                case DataType.Bool:
                    if (d.Bool)
                        return "true";
                    else
                        return "false";
                case DataType.Float:
                    return d.Float.ToString();
                case DataType.Integer:
                    return d.Integer.ToString();
                case DataType.String:
                    return "\"" + d.String + "\"";
            }
            return "INVALID";
        }
    }
}
