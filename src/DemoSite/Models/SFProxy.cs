﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DemoSite.Models;

namespace ConsoleApp1
{
    public class SFProxy : IDisposable
    {
        private const string SFPath = @"C:\Users\bc0618\Desktop\simon\chess\stockfish-7-win\Windows\stockfish 7 x64.exe";
        private readonly Process _proc;
        public SFProxy()
        {
            string path = @"C:\Users\bc0618\Desktop\simon\chess\stockfish-7-win\Windows\stockfish 7 x64.exe";
            _proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = "",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };
            _proc.Start();
        }

        public EvaluatedMove Go(string moveSequence, int depth, string specificMove = "")
        {
            EvaluatedMove evalMove = new EvaluatedMove();
            string beforeFen = FenAfterMoves(moveSequence);
            string moves = moveSequence + " " + specificMove;
            bool reverse = !string.IsNullOrWhiteSpace(specificMove);
            _proc.StandardInput.WriteLine("position startpos moves " + moves);
            _proc.StandardInput.WriteLine("go depth " + depth);
            if (reverse) evalMove.Fen = FenAfterMoves(moves);
            string bestLine = "";
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = _proc.StandardOutput.ReadLine();
                Debug.WriteLine("constructedLine = " + line);
                if (line.IndexOf("info depth ", StringComparison.Ordinal) == 0)
                {
                    bestLine = line;
                }
                else if (line.IndexOf("bestmove", StringComparison.Ordinal) == 0)
                {
                    //debug
                    if(line.IndexOf("bestmove f4g3")==0)
                    {
                        int k = 8;
                    }
                    Dictionary<string, int> info = new Dictionary<string, int>();
                    int pvIndex = bestLine.IndexOf(" pv ", StringComparison.Ordinal);
                    string[] q = bestLine.Substring(0, pvIndex).Split(new char[] {' '});
                    for (int i = 1; i < q.Length; i += 2)
                    {
                        if (q[i] == "score")
                        {
                            info[q[i]+"_"+q[i+1]] = reverse? -int.Parse(q[i + 2]): int.Parse(q[i + 2]);
                            i++;
                        }
                        else
                            info[q[i]] = int.Parse(q[i + 1]);
                    }
                    string pv = bestLine.Substring(pvIndex + 4);
                    if (reverse)
                        pv = specificMove + " " + pv;
                    if (line == "bestmove (none)")
                    {
                        // mate or draw
                    }
                    else
                    {
                        evalMove.MoveLan = (pv+" ").Substring(0, 5).Trim();
                        if (!reverse) evalMove.Fen = FenAfterMoves(moves + " " + evalMove.MoveLan);
                        Board b = new Board();
                        b.Setup(beforeFen);
                        var m = b.GenerateMoves();
                        b.PopulateSan(m);
                        if (info.ContainsKey("score_mate"))
                            evalMove.Value = info["score_mate"] < 0 ? - 30000 : 30000;
                        else
                            evalMove.Value = info["score_cp"];
                        evalMove.MoveSan = m.Single(x => x.Text == evalMove.MoveLan).San;
                        evalMove.DeltaToBest = 0;
                        if (string.IsNullOrWhiteSpace(specificMove))
                        {
                            // set principal variation
                            string[] qqq = pv.Split(new char[] { ' ' });
                            List<EvaluatedMove> qq = new List<EvaluatedMove>();
                            string ff = "";
                            foreach (var move in qqq)
                            {
                                if (FenAfterMoves(moves + ff) != b.DumpFen())
                                {
                                    int k = 8;
                                }
                                ff += " " + move;
                                var xx = b.GenerateMoves();
                                b.PopulateSan(xx);
                                EvaluatedMove ee = new EvaluatedMove();
                                ee.Value = evalMove.Value;
                                ee.MoveLan = move;
                                ee.MoveSan = xx.Single(x => x.Text == move).San;
                                ee.Fen = FenAfterMoves(moves + ff);
                                qq.Add(ee);
                                b.DoMove(b.MoveFromText(move));
                            }
                            evalMove.PrincipalVariation = qq.ToArray();
                        }
                        else
                            evalMove.PrincipalVariation = new EvaluatedMove[0];
                    }
                    return evalMove;
                }
            }
            throw new Exception("error! no bestmove found");
        }

        public string FenAfterMoves(string moves)
        {
            _proc.StandardInput.WriteLine("position startpos moves " + moves);
            _proc.StandardInput.WriteLine("d");
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = _proc.StandardOutput.ReadLine();
                if (line.IndexOf("Fen:") == 0)
                {
                    return line.Substring(5);
                }
            }
            return "";
        }

        public string FenAfterMoves(string fen, string moves)
        {
            _proc.StandardInput.WriteLine("position fen "+fen+" moves " + moves);
            _proc.StandardInput.WriteLine("d");
            while (!_proc.StandardOutput.EndOfStream)
            {
                string line = _proc.StandardOutput.ReadLine();
                if (line.IndexOf("Fen:") == 0)
                {
                    return line.Substring(5);
                }
            }
            return "";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _proc.StandardInput.WriteLine("quit");
            if (!_proc.WaitForExit(5000))
            {
                _proc.Kill();
            }
            _disposed = true;
        }
    }
}
