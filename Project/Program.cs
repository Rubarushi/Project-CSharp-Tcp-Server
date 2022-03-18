using Common;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public static bool m_bStopped = false;
        public static Acceptor Acceptor;
        static void Main(string[] args)
        {
            WorkerThread.StartWork();
            Log.StartLogger();

            Acceptor = new Acceptor(14009);
            Acceptor.StartWork();

            //디비


            //디비

            Task Commander = Task.Factory.StartNew(() =>
            {
                while (m_bStopped == false)
                {
                    string cmd = Console.ReadLine();
                    string[] cmds = cmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (cmds.Length < 1 || !cmds[0].StartsWith("/"))
                        continue;

                    cmds[0] = cmds[0].Replace("/", "");

                    switch (cmds[0])
                    {
                        case "stop":
                            {
                                m_bStopped = true;
                            }
                            break;
                        case "list":
                            {

                            }
                            break;
                        case "info":
                            {
                                if (cmds.Length < 2) // /info 124124-14-1-4-5
                                {
                                    Log.Warning("Missing Parameters: {0}", cmd);
                                    break;
                                }
                            }
                            break;
                        default:
                            Log.Warning("Unknown Command: {0}", cmd);
                            break;
                    }
                }
                //Console.Title = "";
            });

            Commander.Wait();
        }
    }
}
