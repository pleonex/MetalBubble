// ArrNam2ItemList.cs
//
// Copyright (c) 2019 Benito Palacios Sánchez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace MetalBubble
{
    using System;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class ArrName2ItemList :
        IConverter<BinaryFormat, ItemList>, IInitializer<BinaryFormat>
    {
        BinaryFormat binaryNam;

        public void Initialize(BinaryFormat binaryNam)
        {
            if (binaryNam == null)
                throw new ArgumentNullException(nameof(binaryNam));

            this.binaryNam = binaryNam;
        }

        public ItemList Convert(BinaryFormat binaryArr)
        {
            if (binaryArr == null)
                throw new ArgumentNullException(nameof(binaryArr));

            if (binaryNam == null)
                throw new FormatException("Missing initialization");

            var items = new ItemList();

            var infoReader = new DataReader(binaryArr.Stream);
            var namesReader = new DataReader(binaryNam.Stream) {
                DefaultEncoding = Encoding.Unicode,
            };

            int counter = 0;
            while (!binaryArr.Stream.EndOfStream) {
                var item = new Item();

                ushort pointer = infoReader.ReadUInt16();
                item.Unknown0 = infoReader.ReadUInt16();
                item.Unknown1 = infoReader.ReadUInt32();
                item.Unknown2 = infoReader.ReadUInt32();
                item.Id = counter++;

                namesReader.Stream.RunInPosition(
                    () => item.Name = namesReader.ReadString(),
                    pointer);

                items.Items.Add(item);
            }

            return items;
        }
    }
}