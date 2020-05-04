// Sorter.cs
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
    using System.Linq;
    using Yarhl.IO;

    public static class Sorter
    {
        public static void UpdateItemsTable(BinaryFormat arm9, ItemList items, uint table)
        {
            var writer = new DataWriter(arm9.Stream);

            var sorted = items.Items.OrderBy(x => x.Name).ToList();
            for (int i = 0; i < sorted.Count; i++) {
                arm9.Stream.Seek(table + (sorted[i].Id * 2));
                writer.Write((ushort)i);
            }
        }
    }
}
