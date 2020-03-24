using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Flashcards // by Radzuka, https://github.com/Radzuka
{
    class Program
    {
        static void Main(string[] args)
        /* This script reads csv's written in the form "question";"answer" located in current directory
         * (C:\Users\current_user\source\repos\Flashcards\Flashcards\bin\Debug\netcoreapp3.1),
         then displays the options, lets you choose which flashcards do you want to use. It asks you
         if your answer was correct and counts your success rate.
         Possible expansions:
         - new Flashcard sets (it's easy to make the csv)
         - implementing the possibility to shuffle wrong answers again and again to the point where 
           user got 100 % score */
        {
            string file_name = Choose_Flashcards();
            Flashcard_mode(Read_csv(file_name));
            //Console.WriteLine("Aplikace doběhla úspěšně.");
        }

        static string Choose_Flashcards()
        {
            List<string> files_list = new List<string>();
            string[] file_paths = Directory.GetFiles(Environment.CurrentDirectory, "*.csv", SearchOption.TopDirectoryOnly);
            foreach (var file_path in file_paths) // Searching for filenames in file paths
            {
                int last_slash = file_path.LastIndexOf('\\');
                string file_name = file_path.Substring(last_slash + 1);
                files_list.Add(file_name);
                //Console.WriteLine(file_name);
            }
            Console.WriteLine("Jsou dostupné tyto Flashcards: \n");
            int option_number = 1;
            foreach (string flashcards_name in files_list) // displaying Flashcards' options
            {
                Console.WriteLine("{0}: {1}", option_number, flashcards_name);
                option_number++;
            }
            while (true) // dealing with user's input
            {
                Console.Write("\nZadejte číslo Flashcards, které chcete používat: ");
                string chosen_flashcards = Console.ReadLine();
                int chosen_flashcards_number;
                if (Int32.TryParse(chosen_flashcards, out chosen_flashcards_number));
                {
                    try
                    {
                        return files_list[chosen_flashcards_number - 1];
                    }
                    catch (ArgumentOutOfRangeException) // if the number is out of list's range
                    {
                        chosen_flashcards = null;
                    }
                }
                Console.WriteLine("Nerozumím.");
            }
        }

        static Dictionary<string, string> Read_csv(string files_name)
        {
            /* Gets csv's name, decodes it and creates a dictionary in a form {"question":"answer"}.
             * Returns dictionary. */
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // reading csv and dealing with encoding
            using (var reader = new StreamReader(files_name, Encoding.GetEncoding(1250)))
            {
                Dictionary<string, string> all_cards_dict = new Dictionary<string, string>();

                while (!reader.EndOfStream) // reading values from csv, separating and creating dictionary
                { 
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    all_cards_dict.Add(values[0], values[1]);
                }
                reader.Close();
                //Console.WriteLine("Csv načteno a úspěšně zavřeno.");
                return all_cards_dict;
            }
        }


        static bool Flashcard_mode(Dictionary<string, string> all_cards_dict)
        /* Gets dictionary with questions and answers. Presents a question, then answer, then asks
         if the answer was correct. Counts success rate. Returns true when finished. */
        {
            var shuffled_questions = Shuffle_questions(all_cards_dict);
            float correct_count = 0;
            float correct_percentage = 0;
            int question_number = 1;
            float all_questions = all_cards_dict.Count();
            foreach (var question in shuffled_questions) 
            {
                Console.Clear();
                Console.WriteLine("KARTIČKA {0}/{1}\nÚspěšnost: {2} %", question_number, all_questions, correct_percentage);
                Console.WriteLine("\n\nPřední strana kartičky: {0}\n", question);
                Console.Write("Pro zobrazení odpovědi stiskněte tlačítko...");
                Console.ReadKey(true);

                string answer = all_cards_dict[question];
                Console.WriteLine("\n\nODPOVĚĎ: {0}", answer);
                char user_char = 'x';
                while (true) // Dealing with user input on question if the answer was correct
                {
                    Console.WriteLine("\n\nByla Vaše odpověď správná? [a/n]");
                    string user_string = Console.ReadLine();
                    try
                    {
                        user_string = user_string.ToLower();
                        user_char = user_string[0];
                    }
                    catch (Exception) // empty string, Enter key etc...
                    {
                        user_string = "x";
                    }
                    if (user_char == 'a') {correct_count++; break;} // breaks when YES
                    if (user_char == 'n') {break;} // breaks when NO
                    Console.WriteLine("Nerozumím.");
                }
                question_number = question_number + 1;
                correct_percentage = (correct_count / all_questions) * 100;
            }
            Console.Clear();
            Console.WriteLine("Vaše úspěšnost byla {0} %.\n\nDěkujeme Vám, že používáte Flashcards by Radzuka", correct_percentage);
            Console.ReadKey();
            return true;
        }
        static IOrderedEnumerable<string> Shuffle_questions(Dictionary<string, string> all_cards_dict)
        {
            /* Gets dictionary with all the cards, shuffles them and returns them as enumerable. */
            var rnd = new Random();
            List<string> questions_list = new List<string>();
            foreach (var key in all_cards_dict.Keys)
            {
                questions_list.Add(key);
            }
            var shuffled_questions = questions_list.OrderBy(item => rnd.Next());
            return shuffled_questions;
        }
    }
}
