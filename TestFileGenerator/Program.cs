﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestFileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var file = new FileStream("words.txt", FileMode.Create))
            //using (var writer = new StreamWriter(file))
            {
                var words = new WordSet();
                //var words = new HashSet<string>();
                /*
                words.Add("foxes");
                words.Add("fox");
                words.Add("boxes");

                words.Compress();
                var acceptor = words.GetAcceptor();

                Console.WriteLine(acceptor.Contains("fox"));
                Console.WriteLine(acceptor.Contains("foxes"));
                Console.WriteLine(acceptor.Contains("box"));
                Console.WriteLine(acceptor.Contains("boxes"));

                return;
                */
                var stringBuilder = new StringBuilder();
                var size = 0;
                var count = 0;
                var limit = 2 * 1024 * 1024;
                var random = new Random(777);
                var updated = Environment.TickCount;
                var sw = Stopwatch.StartNew();
                var acceptorSize = 0;
                var acceptors = new List<Acceptor>();
                var strings = new List<string>();

                while (size <= limit)
                {
                    var len = random.Next(10) + 3;

                    for (var i = 0; i < len; i++)
                    {
                        stringBuilder.Append((char)(random.Next(128 - 32) + 32));
                    }

                    var word = stringBuilder.ToString();
                    stringBuilder.Clear();

                    if (words.Add(word))
                    {
                        //writer.Write(word);
                        //writer.Write('\n');
                        size += len + 1;
                        acceptorSize += len + 1;
                        strings.Add(word);
                        count++;
                    }

                    if (words.Count % 10000 == 0)
                        words.Compress();

                    //if (acceptorSize >= 24 * 128 * 1024)
                    //{
                    //    acceptorSize = 0;
                    //    words.Compress();
                    //    acceptors.Add(words.GetAcceptor());
                    //    words.Clear();
                    //}

                    if (Environment.TickCount - updated > 500)
                    {
                        updated = Environment.TickCount;
                        Console.SetCursorPosition(0, 0);
                        Console.Write(((float)size / limit).ToString("0.00000"));
                    }
                }

                words.Compress();
                words.Freeze();
                acceptorSize = 0;
                acceptors.Add(words.GetAcceptor());
                words.Clear();

                sw.Stop();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Console.WriteLine();
                Console.WriteLine(sw.Elapsed);
                Console.WriteLine((sw.Elapsed.TotalMilliseconds / (float)count).ToString("F10"));
                //Console.WriteLine("Nodes: " + words.nodes);
                //Console.WriteLine("Merges: " + words.merges);
                //Console.WriteLine("Splits: " + words.splits);
                Console.WriteLine("Lines: " + count);

                sw.Restart();
                for (var wi = 0; wi < strings.Count; wi++)
                {
                    var word = strings[wi];
                    bool found = false;

                    for (var i = 0; i < acceptors.Count; i++)
                    {
                        if (acceptors[i].Contains(word))
                        {
                            found = true;
                            break;
                        }
                    }

                    //found = words.Contains(strings[wi]);

                    if (!found)
                        throw new KeyNotFoundException(word);
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed);
                Console.WriteLine((sw.Elapsed.TotalMilliseconds / (float)count).ToString("F10"));

                strings.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Console.ReadKey();
                Console.WriteLine(words.Count);
                Console.WriteLine(acceptors.Count);
                Console.ReadKey();
            }
        }
    }
}
