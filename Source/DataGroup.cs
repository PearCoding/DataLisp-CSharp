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
    public class DataGroup
    {
        string _ID;
        List<Data> _IndexedData = new List<Data>();
        List<Data> _NamedData = new List<Data>();

        public string ID
        {
            get { return _ID; }
        }

        public void Add(Data data)
        {
            if(data.Key != "")
                _NamedData.Add(data);
            else
                _IndexedData.Add(data);
        }

        public Data At(int i)
        {
            return _IndexedData[i];
        }

        public int IndexedDataCount
        {
            get { return _IndexedData.Count; }
        }

        public Data FromKey(string key)
        {
            foreach(Data data in _NamedData)
            {
                if(data.Key == key)
                {
                    return data;
                }
            }
            return null;
        }

        public List<Data> FromKeyAll(string key)
        {
            List<Data> ret = new List<Data>();
            foreach (Data data in _NamedData)
            {
                if (data.Key == key)
                {
                    ret.Add(data);
                }
            }

            return ret;
        }

        public bool HasKey(string key)
        {
            foreach (Data data in _NamedData)
            {
                if (data.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        public List<Data> IndexedData
        {
            get { return _IndexedData; }
        }

        public List<Data> NamedData
        {
            get { return _NamedData; }
        }

        public DataGroup(string id)
        {
            _ID = id;
        }
    }
}
