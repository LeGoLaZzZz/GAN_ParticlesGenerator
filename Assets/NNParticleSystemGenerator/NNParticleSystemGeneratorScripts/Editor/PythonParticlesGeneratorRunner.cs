using System;
using System.Diagnostics;
using System.IO;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using Debug = UnityEngine.Debug;

namespace NNParticleSystemGenerator.Editor
{
    public class PythonParticlesGeneratorRunner
    {
        public static void RunPython(int epoch, ParticleTags tags, string pythonFileToExecute)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python"; // path to python.exe 
                // "C:\\Users\\user\\AppData\\Local\\Programs\\Python\\Python311\\python.exe"; 
            start.Arguments =
                string.Format("{0} {1} {2} {3} {4}", pythonFileToExecute, epoch, (int)tags.form, (int)tags.element,
                    (int)tags.colorGroup); // Pass two numbers as arguments
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true; // Enable input redirection

            using (Process process = Process.Start(start))
            {
                using (StreamReader outputReader = process.StandardOutput)
                {
                    using (StreamReader errorReader = process.StandardError)
                    {
                        string output = outputReader.ReadToEnd();
                        string errors = errorReader.ReadToEnd();

                        if (!string.IsNullOrEmpty(errors))
                        {
                            Debug.LogError(errors);
                            throw new Exception(errors);
                        }

                        Debug.Log(output);
                    }
                }

                process.WaitForExit();
            }
        }
    }
}