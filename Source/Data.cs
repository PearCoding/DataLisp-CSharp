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
    enum DataType
    {
        Group,
        Integer,
        Float,
        Bool,
        String,
        Array
    }

    class Data
    {
        public string Key { get; set; }

        DataType _Type;
        public DataType Type { get { return this._Type; } }

        //Internal
        DataGroup _Group = null;
        public DataGroup Group
        {
            get { return this._Group; }
            set { _Type = DataType.Group; _Group = value; }
        }

        DataArray _Array = null;
        public DataArray Array
        {
            get { return _Array; }
            set { _Type = DataType.Array; _Array = value; }
        }

        int _Integer;
        public int Integer
        {
            get { return _Integer; }
            set { _Type = DataType.Integer; _Integer = value; }
        }

        float _Float;
        public float Float
        {
            get { return _Float; }
            set { _Type = DataType.Float; _Float = value; }
        }

        bool _Bool;
        public bool Bool
        {
            get { return _Bool; }
            set { _Type = DataType.Bool; _Bool = value; }
        }

        string _String;
        public string String
        {
            get { return _String; }
            set { _Type = DataType.String; _String = value; }
        }

        public float FloatConverted
        {
            get
            {
                if (_Type == DataType.Integer)
                    return (float)_Integer;
                else
                    return _Float;
            }
        }

        public bool IsNumber
        {
            get { return _Type == DataType.Float || _Type == DataType.Integer; }
        }
        
        public Data(string key)
        {
            Key = key;
        }

        public Data()
        {
            Key = "";
        }
    }
}
