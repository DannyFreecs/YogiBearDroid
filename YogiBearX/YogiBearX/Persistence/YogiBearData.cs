using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;

namespace YogiBearX.Persistence
{
    //Fájl betöltés típusa
    public class YogiBearData
    {
        //Pálya fájlból történő beolvasáa, majd továbbadása
        public async Task<List<List<Int32>>> LoadFromFileAsync()
        {
            try
            {
                //Pálya kiválasztása
                Int32 chooseMap;
            
                var maps = new string[] { "Első szint", "Második szint", "Harmadik szint" };
                var choice = await App.Current.MainPage.DisplayActionSheet("Pálya választás", "Mégse", null, maps);
                if (string.IsNullOrEmpty(choice))
                    chooseMap = -1;
                else if (choice == maps[0])
                    chooseMap = 0;
                else if (choice == maps[1])
                    chooseMap = 1;
                else if (choice == maps[2])
                    chooseMap = 2;
                else
                    chooseMap = 3;

                if (chooseMap == 3)
                    return null;

                return LoadMap(chooseMap);
            }
            catch
            {
                throw new YogiBearDataException();
            }

        }

        private List<List<Int32>> LoadMap(int selectedMapName)
        {
            var assem = typeof(YogiBearData).GetTypeInfo().Assembly;
            Stream fileStream;
            switch (selectedMapName)
            {
                case 0:
                    fileStream = assem.GetManifestResourceStream("YogiBearX.Resources.Map.level1.map");
                    break;
                case 1:
                    fileStream = assem.GetManifestResourceStream(string.Format("YogiBearX.Resources.Map.level2.map"));
                    break;
                case 2:
                    fileStream = assem.GetManifestResourceStream(string.Format("YogiBearX.Resources.Map.level3.map"));
                    break;
                default:
                    return null;
            }

            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine(); // Soronként olvasunk
                Int32 size = Int32.Parse(line); // Fájl első paramétere a pályaméret
                List<List<Int32>> map = new List<List<Int32>>(size); //pálya tárolására alkalmas konténer

                for (Int32 i = 0; i < size; i++)
                {
                    line = reader.ReadLine();
                    string[] numbers = line.Split(' ');
                    List<Int32> tmp = new List<Int32>(size);

                    for (Int32 j = 0; j < size; j++)
                        tmp.Add(Int32.Parse(numbers[j]));

                    map.Add(tmp);
                }

                return map;
            }
        }

        public List<List<Int32>> LoadFirstLevel()
        {
            return LoadMap(0);
        }

    }
}
