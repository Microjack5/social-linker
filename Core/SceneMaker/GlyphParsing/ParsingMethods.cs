using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialLinker.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SocialLinker.Core.SceneMaker.GlyphParsing
{
    class ParsingMethods
    {
        private static List<ParsingFields> glyphs;

        public static ParsingFields Get_P1_PS1_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P1-PS1//Font";
            string font_data = "p1-ps1_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }

            return null;
        }

        public static ParsingFields Get_P2EP_PS1_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P2EP-PS1//Font";
            string font_data = "p2ep-ps1_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }

            return null;
        }

        public static ParsingFields Get_P3F_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3F//Font";
            string font_data = "p3f_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }
            else
            {
                Create_Font_Data_Sheet();
            }

            return null;
        }

        public static ParsingFields Get_P3P_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P3P//Font";
            string font_data = "p3p_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }
            else
            {
                Create_Font_Data_Sheet();
            }

            return null;
        }

        public static ParsingFields Get_P4_PS2_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4-PS2//Font";
            string font_data = "p4-ps2_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }

            return null;
        }

        public static ParsingFields Get_P4G_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Font";
            string font_data = "p4g_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }

            return null;
        }

        public static ParsingFields Get_P5R_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5R//Font";
            string font_data = "p5r_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }
            else
            {
                Create_Font_Data_Sheet();
            }

            return null;
        }

        public static ParsingFields Get_P5S_Glyph(char character_to_render)
        {
            string font_folder = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Font";
            string font_data = "p5s_font_data.json";

            // If the file folder doesn't exist, create it.
            if (!Directory.Exists(font_folder))
            {
                Directory.CreateDirectory(font_folder);
            }

            // If the file exists, load its contents.
            if (File.Exists(font_folder + "/" + font_data))
            {
                glyphs = LoadGlyphList(font_folder + "/" + font_data).ToList();
                return GetGlyph(character_to_render);
            }
            else
            {
                Create_Font_Data_Sheet();
            }

            return null;
        }

        public static IEnumerable<ParsingFields> LoadGlyphList(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<ParsingFields>>(json);
        }

        public static ParsingFields GetGlyph(char input)
        {
            foreach (ParsingFields s in glyphs)
            {
                string[] options = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(s.SupportedChars);

                for (int i = 0; i < options.Length; i++)
                {
                    if (Convert.ToChar(options[i]) == input)
                    {
                        return s;
                    }
                }
            }

            return null;
        }

        public static List<ParsingFields> Create_Font_Data_Sheet()
        {
            var new_list = new List<ParsingFields>();

            try
            {
                string converted_xml_to_json = $@"C://Users//Microjack5//Downloads//converted_p5r_xml.json";
                string base_data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P4G//Font//p4g_font_data.json";
                string new_data_path = $@"{AssetDirectoryConfig.assetDirectory.assetFolderPath}//SceneMaker//Templates//P5S//Font//p5s_font_data.json";

                string current_line = "";
                string current_glyph = "";

                string glyph = "";
                string text = File.ReadAllText(converted_xml_to_json);
                string left_cut = "";
                string right_cut = "";
                string input = "";

                var json = JObject.Parse(text);

                var glyphs = LoadGlyphList(base_data_path).ToList();

                for (int line_count = 1; line_count <= 32; line_count++)
                {
                    current_line = $"Line_{line_count}";

                    for (int glyph_count = 1; glyph_count <= 16; glyph_count++)
                    {
                        current_glyph = $"Glyph_{glyph_count}";

                        glyph = current_glyph; // Or wherever you get that from

                        var glyph_path = json[current_line][glyph];

                        Console.WriteLine($"Current: {current_line}, {current_glyph}");

                        left_cut = (string)glyph_path["LeftCut"];
                        right_cut = (string)glyph_path["RightCut"];

                        foreach (var s in glyphs)
                        {
                            if (s.Row == line_count - 1 && s.Column == glyph_count - 1)
                            {
                                input = s.SupportedChars;
                            }
                        }

                        var new_parsing_data = new ParsingFields()
                        {
                            Row = line_count - 1,
                            Column = glyph_count - 1,
                            LeftCut = Int32.Parse(left_cut),
                            RightCut = Int32.Parse(right_cut),
                            SupportedChars = input
                        };
                        new_list.Add(new_parsing_data);
                    }
                }

                string new_json = JsonConvert.SerializeObject(new_list, Formatting.Indented);
                File.WriteAllText(new_data_path, new_json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("All done!");
            return new_list;
        }
    }
}
