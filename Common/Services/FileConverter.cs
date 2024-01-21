using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CzechJudiciaryDecisionsTools.Common.Services
{
    public class FileConverter
    {
        public async static Task ConvertFileEncodingToUtf8Async(string filePath, Encoding sourceEncoding)
        {
            string content = await File.ReadAllTextAsync(filePath, sourceEncoding);
            await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
        }
    }

}
