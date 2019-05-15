/***************************************************************************
 *   Copyright (C) 2008 by Peter L Jones                                   *
 *   pljones@users.sf.net                                                  *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using SimPe.Interfaces.Plugin;

namespace pjHoodTool
{
    public class pfSDSC : pjse.ExtendedWrapper<pfSDSCItem, pfSDSC>, IDictionary<String, pfSDSCItem>
    {
        static Dictionary<string, UInt16> keyMap = new Dictionary<string, ushort>();
        static pfSDSC()
        {
            KeyValuePair<string, UInt16>[] akv = new KeyValuePair<string, ushort>[] {
                new KeyValuePair<string, ushort>("", 0)
            };
            foreach (KeyValuePair<string, UInt16> kv in akv) keyMap.Add(kv.Key, kv.Value);
        }


        private byte[] filename = null;
        private byte[] endName = null;
        public string Filename
        {
            get { return SimPe.Helper.ToString(filename); }
            set
            {
                if (!SimPe.Helper.ToString(filename).Equals(value))
                {
                    filename = SimPe.Helper.ToBytes(value, 0x40);
                    OnWrapperChanged(this, new EventArgs());
                }
            }
        }

        protected override void Unserialize(System.IO.BinaryReader reader)
        {
            filename = null;
            endName = null;
            items = new List<pfSDSCItem>();

            filename = reader.ReadBytes(0x40);

            long limit = reader.BaseStream.Length - reader.BaseStream.Position - Filename.Length;
            while (items.Count * 2 < limit - 1)
                items.Add(reader.ReadUInt16());

            endName = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            if (!Filename.Equals(SimPe.Helper.ToString(endName)))
                throw new InvalidOperationException("Trailing filename (\"" + SimPe.Helper.ToString(endName) +
                    "\") not equal to Filename (\"" + Filename + "\")");
        }

        protected override void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write(filename);
            foreach (pfSDSCItem i in items) writer.Write((ushort)i);
            writer.Write(endName);
        }

        protected override IPackedFileUI CreateDefaultUIHandler() { throw new Exception("The method or operation is not implemented."); }

        public override void Add(pfSDSCItem item) { throw new NotSupportedException("The method or operation is not supported."); }
        public new void AddRange(IEnumerable<pfSDSCItem> collection) { throw new NotSupportedException("The method or operation is not supported."); }
        public new void Clear() { throw new NotSupportedException("The method or operation is not supported."); }
        public new void Insert(int index, pfSDSCItem item) { throw new NotSupportedException("The method or operation is not supported."); }
        public new void InsertRange(int index, IEnumerable<pfSDSCItem> collection) { throw new NotSupportedException("The method or operation is not supported."); }
        public new bool Remove(pfSDSCItem item) { throw new NotSupportedException("The method or operation is not supported."); }
        public new int RemoveAll(Predicate<pfSDSCItem> match) { throw new NotSupportedException("The method or operation is not supported."); }
        public new void RemoveRange(int index, int count) { throw new NotSupportedException("The method or operation is not supported."); }


        #region IDictionary<string,pfSDSCItem> Members

        public void Add(string key, pfSDSCItem value) { throw new NotSupportedException("The method or operation is not supported."); }

        public bool ContainsKey(string key) { return keyMap.ContainsKey(key); }

        public ICollection<string> Keys { get { return keyMap.Keys; } }

        public bool Remove(string key) { throw new NotSupportedException("The method or operation is not supported."); }

        public bool TryGetValue(string key, out pfSDSCItem value)
        {
            value = keyMap.ContainsKey(key) ? this[keyMap[key]] : new pfSDSCItem(0);
            return keyMap.ContainsKey(key);
        }

        public ICollection<pfSDSCItem> Values
        {
            get
            {
                List<pfSDSCItem> values = new List<pfSDSCItem>();
                foreach (int i in keyMap.Values) values.Add(this[i]);
                return values;
            }
        }

        public pfSDSCItem this[string key]
        {
            get { return this[keyMap[key]]; }
            set { this[keyMap[key]] = value; }
        }

        #endregion

        #region ICollection<KeyValuePair<string,pfSDSCItem>> Members

        public void Add(KeyValuePair<string, pfSDSCItem> item) { throw new NotSupportedException("The method or operation is not supported."); }

        public bool Contains(KeyValuePair<string, pfSDSCItem> item) { return this[item.Key] == item.Value; }

        public void CopyTo(KeyValuePair<string, pfSDSCItem>[] array, int arrayIndex)
        {
            foreach (string key in keyMap.Keys)
                array[arrayIndex++] = new KeyValuePair<string, pfSDSCItem>(key, this[key]);
        }

        public bool Remove(KeyValuePair<string, pfSDSCItem> item) { throw new NotSupportedException("The method or operation is not supported."); }

        #endregion

        #region IEnumerable<KeyValuePair<string,pfSDSCItem>> Members

        public new IEnumerator<KeyValuePair<string, pfSDSCItem>> GetEnumerator() { throw new NotSupportedException("The method or operation is not supported."); }

        #endregion
    }

    public class pfSDSCItem : pjse.ExtendedWrapperItem<pfSDSC, pfSDSCItem>
        , IComparable<ushort>, IEquatable<ushort>, IComparable<pfSDSCItem>
    {
        private UInt16 value;
        public pfSDSCItem(UInt16 value) { this.value = value; }

        #region Conversions
        public static explicit operator byte(pfSDSCItem i) { return (byte)i.value; }
        public static explicit operator short(pfSDSCItem i) { return (short)i.value; }
        public static implicit operator ushort(pfSDSCItem i) { return i.value; }
        public static explicit operator pfSDSCItem(short i) { return new pfSDSCItem((ushort)i); }
        public static implicit operator pfSDSCItem(ushort i) { return new pfSDSCItem(i); }

        public override string ToString() { return value.ToString(); }
        #endregion

        public override bool Equals(pfSDSCItem other) { return value.Equals(other.value); }

        #region IComparable<ushort> Members

        public int CompareTo(ushort other) { return value.CompareTo(other); }

        #endregion

        #region IEquatable<ushort> Members

        public bool Equals(ushort other) { return value.Equals(other); }

        #endregion

        #region IComparable<pfOBJDItem> Members

        public int CompareTo(pfSDSCItem other) { return value.CompareTo(other.value); }

        #endregion
    }
}
