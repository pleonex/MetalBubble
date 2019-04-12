// Program.cs
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
    using System.IO;
    using System.Reflection;
    using Yarhl.IO;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "MetalBubble v{0} -- Item sorter for Metal Max 3 ~~ by pleonex",
                Assembly.GetExecutingAssembly().GetName().Version);

            if (args.Length != 2) {
                Console.WriteLine("Invalid number of arguments");
                Console.WriteLine("USAGE: MetalBubble <xls/pack_data.pak> <arm9.bin>");
                Environment.Exit(-1);
            }

            Console.WriteLine("MetalBubble ~ by pleoNeX");

            string packPath = args[0];
            string arm9Path = args[1];

            using (Node pack = NodeFactory.FromFile(packPath))
            using (Node arm9 = NodeFactory.FromFile(arm9Path)) {
                pack.TransformWith<BinaryPack2Container>();

                var itemListNames = pack.Children["ITEMLIST.NAM"]
                    .GetFormatAs<BinaryFormat>();
                var itemListInfo = pack.Children["ITEMLIST.ARR"]
                    .TransformWith<ArrName2ItemList, BinaryFormat>(itemListNames);

                Sorter.UpdateItemsTable(
                    arm9.GetFormatAs<BinaryFormat>(),
                    itemListInfo.GetFormatAs<ItemList>());
            }

            Console.WriteLine("Done!");
        }
    }
}
