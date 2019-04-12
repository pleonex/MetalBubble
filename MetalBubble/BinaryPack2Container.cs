// BinaryPack2Container.cs
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
    using System.Collections.Generic;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class BinaryPack2Container : IConverter<BinaryFormat, NodeContainerFormat>
    {
        DataReader reader;

        ushort fatOffset;
        ushort strPoolOffset;

        public NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var container = new NodeContainerFormat();
            reader = new DataReader(source.Stream);

            source.Stream.Position = 0;
            ushort numFiles = reader.ReadUInt16();
            fatOffset = reader.ReadUInt16();
            strPoolOffset = reader.ReadUInt16();
            ushort nodeTableOffset = reader.ReadUInt16();

            source.Stream.Position = nodeTableOffset;
            var queue = new SortedList<ushort, string>();
            queue.Add(0, string.Empty);
            do {
                // We use the SorteList as an sorted queue
                string current = queue.Values[0];
                queue.RemoveAt(0);

                ushort numSegments = reader.ReadUInt16();
                for (int i = 0; i < numSegments; i++) {
                    (string segment, ushort id) = ReadSegment();
                    string name = current + segment;

                    if ((id & 0x01) == 1) {
                        var child = ReadChild(name, id);
                        container.Root.Add(child);
                    } else {
                        queue.Add(id, name);
                    }
                }
            } while (queue.Count > 0);

            return container;
        }

        (string, ushort) ReadSegment()
        {
            ushort segmentOffset = reader.ReadUInt16();
            ushort id = reader.ReadUInt16();

            string segment = null;
            reader.Stream.RunInPosition(
                () => segment = reader.ReadString(),
                strPoolOffset + segmentOffset);

            return (segment, id);
        }

        Node ReadChild(string name, ushort id)
        {
            BinaryFormat binary = null;
            reader.Stream.RunInPosition(
                () => {
                    uint size = reader.ReadUInt32();
                    uint offset = reader.ReadUInt32();
                    binary = new BinaryFormat(reader.Stream, offset, size);
                },
                fatOffset + ((id - 1) * 4));

            return new Node(name, binary);
        }
    }
}