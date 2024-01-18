using System;
using System.IO;

namespace DataTransferApi
{
    class WriteLog
    {
            public static bool WriteLogFile(string strFileName, string strMessage)
            {
                try
                {
                    StreamWriter objStreamWriter;
                    if (!File.Exists(strFileName))
                    {
                        objStreamWriter = new StreamWriter(strFileName);
                    }
                    else
                    {
                        objStreamWriter = File.AppendText(strFileName);
                    }

                    objStreamWriter.WriteLine(strMessage);
                    objStreamWriter.WriteLine();
                    // Close the stream:
                    objStreamWriter.Close();
                    // objFilestream.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
