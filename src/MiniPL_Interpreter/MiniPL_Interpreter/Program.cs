using System;
using System.IO;

namespace MiniPL_Interpreter
{
    /// <summary>
    /// Main program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method for the interpreter
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            while (true)
            {
                string code;

                // Read a file name
                // If it exists, then read the code
                // Else ask new file name
                while (true)
                {
                    Console.Write("\nEnter the filename where the code is: ");
                    string fileName = Console.In.ReadLine();
                    if (File.Exists(fileName))
                    {
                        code = File.ReadAllText(fileName);
                        break;
                    }
                    Console.Write("\nFile doesn't exist\n");
                }

                try
                {
                    LexAnalyzer lex = new LexAnalyzer(code);
                    Parser parser = new Parser(lex);

                    SemanticChecker semanticChecker = new SemanticChecker(parser);
                    semanticChecker.Analyze();

                    Interpreter interpreter = new Interpreter(semanticChecker);

                    // Check if there are semantic errors
                    // If there are, print them out and stop
                    // If no errors, then execute the program
                    if (!semanticChecker.HasErrors()) interpreter.Interpret();
                    else semanticChecker.PrintAllErrors();
                }
                catch (Exception e)
                {
                    Console.Write("\n" + e.Message);
                }
            }

                
        }
    }
}
