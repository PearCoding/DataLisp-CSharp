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

namespace DataLisp
{
    class DataContainer
    {
        List<DataGroup> _TopGroups = new List<DataGroup>();

        public void Build(Internal.SyntaxTree tree)
        {
            foreach(Internal.StatementNode n in tree.Nodes)
            {
                DataGroup group = BuildGroup(n);
                _TopGroups.Add(group);
            }
        }

        public List<DataGroup> TopGroups
        {
            get { return _TopGroups; }
        }

        private DataGroup BuildGroup(Internal.StatementNode n)
        {
            DataGroup group = new DataGroup(n.Name);
            foreach (Internal.DataNode d in n.Nodes)
            {
                Data data = BuildData(d);
                if (data != null)
                    group.Add(data);
            }
            return group;
        }

        private Data BuildData(Internal.DataNode n)
        {
            Data data = null;

            switch(n.Value.Type)
            {
                case Internal.ValueNodeType.Statement:
                    data = new Data(n.Key);
                    DataGroup group = BuildGroup(n.Value.Statement);
                    data.Group = group;
                    break;
                case Internal.ValueNodeType.Integer:
                    data = new Data(n.Key);
                    data.Integer = n.Value.Integer;
                    break;
                case Internal.ValueNodeType.Float:
                    data = new Data(n.Key);
                    data.Float = n.Value.Float;
                    break;
                case Internal.ValueNodeType.Bool:
                    data = new Data(n.Key);
                    data.Bool = n.Value.Boolean;
                    break;
                case Internal.ValueNodeType.String:
                    data = new Data(n.Key);
                    data.String = n.Value.String;
                    break;
                case Internal.ValueNodeType.Array:
                    data = new Data(n.Key);
                    data.Array = BuildArray(n.Value.Array);
                    break;
            }

            return data;
        }

        private DataArray BuildArray(Internal.ArrayNode n)
        {
            DataArray array = new DataArray();
            foreach(Internal.ValueNode v in n.Nodes)
            {
                Data data = BuildArrayValue(v);
                if (data != null)
                    array.Add(data);
            }

            return array;
        }

        private Data BuildArrayValue(Internal.ValueNode n)
        {
            Data data = null;

            switch (n.Type)
            {
                case Internal.ValueNodeType.Statement:
                    data = new Data();
                    DataGroup group = BuildGroup(n.Statement);
                    data.Group = group;
                    break;
                case Internal.ValueNodeType.Integer:
                    data = new Data();
                    data.Integer = n.Integer;
                    break;
                case Internal.ValueNodeType.Float:
                    data = new Data();
                    data.Float = n.Float;
                    break;
                case Internal.ValueNodeType.Bool:
                    data = new Data();
                    data.Bool = n.Boolean;
                    break;
                case Internal.ValueNodeType.String:
                    data = new Data();
                    data.String = n.String;
                    break;
                case Internal.ValueNodeType.Array:
                    data = new Data();
                    data.Array = BuildArray(n.Array);
                    break;
            }

            return data;
        }
    }
}
